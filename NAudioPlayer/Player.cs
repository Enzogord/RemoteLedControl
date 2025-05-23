﻿using AudioPlayer.TimeLine;
using NAudio.Wave;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Windows.Threading;

namespace NAudioPlayer
{
    public class Player : IWaveformPlayer, IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region Fields
        private readonly DispatcherTimer positionTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
        private readonly BackgroundWorker waveformGenerateWorker = new BackgroundWorker();
        private readonly int fftDataSize = 256;
        private bool disposed;
        private bool inChannelTimerUpdate;

        private WaveOut waveOutDevice;
        private WaveStream activeStream;
        private WaveChannel32 inputStream;
        private AudioFormat audioFormat;

        private SampleAggregator sampleAggregator;
        private SampleAggregator waveformAggregator;
        private float[] waveformData;

        private byte[] audioData = Array.Empty<byte>();
        private byte[] newAudioDataBuffer = Array.Empty<byte>();
        #endregion

        #region Constants
        private const int waveformCompressedPointCount = 5000;
        #endregion

        #region Constructor
        public Player()
        {
            positionTimer.Interval = TimeSpan.FromMilliseconds(50);
            positionTimer.Tick += positionTimer_Tick;
            waveformGenerateWorker.DoWork += waveformGenerateWorker_DoWork;
            waveformGenerateWorker.RunWorkerCompleted += waveformGenerateWorker_RunWorkerCompleted;
            waveformGenerateWorker.WorkerSupportsCancellation = true;
        }

        Dispatcher dispatcher;

        public void Init(Dispatcher mainThreadDispatcher)
        {
            dispatcher = mainThreadDispatcher;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!disposed) {
                if(disposing) {
                    StopAndCloseStream();
                }
                waveformGenerateWorker.Dispose();
                disposed = true;
            }
        }
        #endregion

        #region IWaveformPlayer

        public float[] WaveformData {
            get { return waveformData; }
            protected set {
                float[] oldValue = waveformData;
                SetField(ref waveformData, value, () => WaveformData);
            }
        }

        private double channelLength;
        public double ChannelLength {
            get { return channelLength; }
            protected set {
                SetField(ref channelLength, value, () => ChannelLength);
            }
        }

        private bool inChannelSet;
        private double channelPosition;
        public virtual double ChannelPosition {
            get { return channelPosition; }
            set {
                if(!inChannelSet && !audioFileEndedInvoked) {
                    inChannelSet = true; // Avoid recursion
                    double position = Math.Max(0, Math.Min(value, ChannelLength));
                    if(!inChannelTimerUpdate && ActiveStream != null) {
                        ActiveStream.Position = (long)((position / ActiveStream.TotalTime.TotalSeconds) * ActiveStream.Length);
                    }
                    if(Math.Abs(position - ChannelLength) <= 0.000001) {
                        RaiseAudioFileEnded();
                        Stop();
                        ActiveStream.Position = 0;
                        OnPropertyChanged(() => CurrentTime);
                    }
                    if(SetField(ref channelPosition, position, () => ChannelPosition)) {
                        OnPropertyChanged(() => CurrentTime);
                    }
                    inChannelSet = false;
                }
            }
        }

        public event EventHandler AudioFileEnded;
        public event EventHandler ChannelPositionUserChanged;

        private bool audioFileEndedInvoked;
        private void RaiseAudioFileEnded()
        {
            audioFileEndedInvoked = true;
            AudioFileEnded?.Invoke(this, EventArgs.Empty);
            audioFileEndedInvoked = false;
        }

        public void ChangePosition(double position)
        {
            ChannelPosition = position;
            ChannelPositionUserChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged<T>(Expression<Func<T>> selectorExpression)
        {
            if(selectorExpression == null) {
                throw new ArgumentNullException("selectorExpression should not be null");
            }
            MemberExpression body = selectorExpression.Body as MemberExpression;
            if(body == null) {
                throw new ArgumentException("The body must be a member expression");
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(body.Member.Name));
        }

        protected bool SetField<T>(ref T field, T value, Expression<Func<T>> selectorExpression)
        {
            if(EqualityComparer<T>.Default.Equals(field, value)) {
                return false;
            }
            field = value;
            OnPropertyChanged(selectorExpression);
            return true;
        }
        #endregion

        #region Waveform Generation
        private class WaveformGenerationParams
        {
            public WaveformGenerationParams(int points)
            {
                Points = points;
            }

            public int Points { get; protected set; }
        }

        private void GenerateWaveformData()
        {
            if(waveformGenerateWorker.IsBusy) {
                waveformGenerateWorker.CancelAsync();
                return;
            }

            if(!waveformGenerateWorker.IsBusy && waveformCompressedPointCount != 0)
                waveformGenerateWorker.RunWorkerAsync();
        }

        private void waveformGenerateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Cancelled) {
                if(!waveformGenerateWorker.IsBusy && waveformCompressedPointCount != 0)
                    waveformGenerateWorker.RunWorkerAsync();
            }
        }

        private void waveformGenerateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using(WaveStream waveStream = CreateWaveStream())
            using(WaveChannel32 waveformInputStream = new WaveChannel32(waveStream)) {
                waveformInputStream.Sample += waveStream_Sample;

                int frameCount = (int)((double)waveformInputStream.Length / (double)fftDataSize);
                int waveformLength = frameCount * 2;
                byte[] readBuffer = new byte[fftDataSize];
                waveformAggregator = new SampleAggregator(fftDataSize);

                float maxLeftPointLevel = float.MinValue;
                float maxRightPointLevel = float.MinValue;
                int currentPointIndex = 0;
                float[] waveformCompressedPoints = new float[waveformCompressedPointCount];
                List<float> waveformData = new List<float>();
                List<int> waveMaxPointIndexes = new List<int>();

                for(int i = 1; i <= waveformCompressedPointCount; i++) {
                    waveMaxPointIndexes.Add((int)Math.Round(waveformLength * ((double)i / (double)waveformCompressedPointCount), 0));
                }
                int readCount = 0;
                while(currentPointIndex * 2 < waveformCompressedPointCount) {
                    waveformInputStream.Read(readBuffer, 0, readBuffer.Length);

                    waveformData.Add(waveformAggregator.LeftMaxVolume);
                    waveformData.Add(waveformAggregator.RightMaxVolume);

                    if(waveformAggregator.LeftMaxVolume > maxLeftPointLevel)
                        maxLeftPointLevel = waveformAggregator.LeftMaxVolume;
                    if(waveformAggregator.RightMaxVolume > maxRightPointLevel)
                        maxRightPointLevel = waveformAggregator.RightMaxVolume;

                    if(readCount > waveMaxPointIndexes[currentPointIndex]) {
                        waveformCompressedPoints[(currentPointIndex * 2)] = maxLeftPointLevel;
                        waveformCompressedPoints[(currentPointIndex * 2) + 1] = maxRightPointLevel;
                        maxLeftPointLevel = float.MinValue;
                        maxRightPointLevel = float.MinValue;
                        currentPointIndex++;
                    }
                    if(readCount % 3000 == 0) {
                        float[] clonedData = (float[])waveformCompressedPoints.Clone();
                        dispatcher.Invoke(new Action(() =>
                        {
                            WaveformData = clonedData;
                        }));
                    }

                    if(waveformGenerateWorker.CancellationPending) {
                        e.Cancel = true;
                        break;
                    }
                    readCount++;
                }

                float[] finalClonedData = (float[])waveformCompressedPoints.Clone();
                dispatcher.Invoke(new Action(() =>
                {
                    WaveformData = finalClonedData;
                }));
            }
        }
        #endregion

        #region Private Utility Methods
        private void StopAndCloseStream()
        {
            if(waveOutDevice != null) {
                waveOutDevice.Stop();
            }
            if(activeStream != null) {
                inputStream.Close();
                inputStream = null;
                ActiveStream.Close();
                ActiveStream = null;
            }
            if(waveOutDevice != null) {
                waveOutDevice.Dispose();
                waveOutDevice = null;
            }
            IsInitialized = false;
        }
        #endregion

        #region Public Methods
        public virtual void Stop()
        {
            if(waveOutDevice != null) {
                waveOutDevice.Stop();
            }
            if(ActiveStream != null) {
                ChannelPosition = 0;
            }
            IsPlaying = false;
            CanStop = false;
            CanPlay = true;
            CanPause = false;
        }

        public virtual void Pause()
        {
            if(IsPlaying && CanPause) {
                waveOutDevice.Pause();
                IsPlaying = false;
                CanPlay = true;
                CanPause = false;
            }
        }

        public virtual void Play()
        {
            if(CanPlay) {
                waveOutDevice.Play();
                IsPlaying = true;
                CanPause = true;
                CanPlay = false;
                CanStop = true;
            }
        }

        private void Start()
        {
            Stop();

            if(ActiveStream != null) {
                ChannelPosition = 0;
            }

            StopAndCloseStream();

            try {
                WaveCallbackInfo waveCallbackInfo = WaveCallbackInfo.FunctionCallback();
                waveOutDevice = new WaveOut(waveCallbackInfo)
                {
                    DesiredLatency = 100
                };
                audioData = newAudioDataBuffer;
                ActiveStream = CreateWaveStream();
                inputStream = new WaveChannel32(ActiveStream);
                sampleAggregator = new SampleAggregator(fftDataSize);
                inputStream.Sample += inputStream_Sample;
                waveOutDevice.Init(inputStream);
                ChannelLength = inputStream.TotalTime.TotalSeconds;
                GenerateWaveformData();
                CanPlay = true;
                IsInitialized = true;
            }
            catch {
                ActiveStream = null;
                CanPlay = false;
                IsInitialized = false;
            }
        }

        private WaveStream CreateWaveStream()
        {
            var memoryStream = new MemoryStream(audioData);
            switch(audioFormat) {
                case AudioFormat.WAV:
                    return new WaveFileReader(memoryStream);
                case AudioFormat.MP3:
                default:
                    return new Mp3FileReader(memoryStream);
            }
    }

        public void Open(string filePath)
        {
            var file = new FileInfo(filePath);
            if(!file.Exists) {
                return;
            }

            AudioFormat format;

            switch(file.Extension.Trim('.').Trim()) {
                case "wav":
                    format = AudioFormat.WAV;
                    break;
                case "mp3":
                    format = AudioFormat.MP3;
                    break;
                default:
                    throw new FileFormatException($"Неподдерживаемый формат файла \"{file.Extension}\"");
            }

            using(var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
                Open(stream, format);
            }
        }

        public void Open(Stream stream, AudioFormat audioFormat)
        {
            this.audioFormat = audioFormat;
            byte[] buffer;
            try {
                buffer = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(buffer, 0, (int)stream.Length);
            }
            catch(Exception) {
                return;
            }

            newAudioDataBuffer = buffer;
            Start();
        }


        public void Close()
        {
            StopAndCloseStream();
        }

        #endregion

        #region Public Properties

        private bool isEnabled;
        public bool IsEnabled {
            get => isEnabled && IsInitialized;
            set => SetField(ref isEnabled, value, () => IsEnabled);
        }

        private bool isInitialized;
        public bool IsInitialized {
            get => isInitialized;
            set {
                if(SetField(ref isInitialized, value, () => IsInitialized)) {
                    OnPropertyChanged(() => IsEnabled);
                }
            }
        }

        public WaveStream ActiveStream {
            get { return activeStream; }
            protected set {
                SetField(ref activeStream, value, () => ActiveStream);
            }
        }

        public TimeSpan TotalTime => ActiveStream.TotalTime;

        public TimeSpan CurrentTime { 
            get => ActiveStream.CurrentTime; 
            set => ActiveStream.CurrentTime = value; 
        }

        public float Volume {
            get {
                if(waveOutDevice == null) {
                    return 0f;
                }
                return waveOutDevice.Volume;
            }
            set {
                if(waveOutDevice == null) {
                    return;
                }
                float volume = value;
                if(value < 0) {
                    volume = 0;
                }
                if(value > 1) {
                    volume = 1;
                }
                waveOutDevice.Volume = volume;
                OnPropertyChanged(() => Volume);
            }
        }

        private bool canPlay;
        public bool CanPlay {
            get { return canPlay; }
            protected set {
                SetField(ref canPlay, value, () => CanPlay);
            }
        }

        private bool canPause;
        public bool CanPause {
            get { return canPause; }
            protected set {
                SetField(ref canPause, value, () => CanPause);
            }
        }

        private bool canStop;
        public bool CanStop {
            get { return canStop; }
            protected set {
                SetField(ref canStop, value, () => CanStop);
            }
        }

        private bool isPlaying;
        public bool IsPlaying {
            get { return isPlaying; }
            protected set {
                SetField(ref isPlaying, value, () => IsPlaying);
                positionTimer.IsEnabled = value;
            }
        }

        #endregion

        #region Event Handlers
        private void inputStream_Sample(object sender, SampleEventArgs e)
        {
            sampleAggregator.Add(e.Left, e.Right);
        }

        void waveStream_Sample(object sender, SampleEventArgs e)
        {
            waveformAggregator.Add(e.Left, e.Right);
        }

        void positionTimer_Tick(object sender, EventArgs e)
        {
            inChannelTimerUpdate = true;
            ChannelPosition = ((double)ActiveStream.Position / (double)ActiveStream.Length) * ActiveStream.TotalTime.TotalSeconds;
            inChannelTimerUpdate = false;
        }
        #endregion
    }

    public enum AudioFormat
    {
        MP3,
        WAV
    }
}
