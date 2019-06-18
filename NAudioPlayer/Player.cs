using AudioPlayer.TimeLine;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Threading;

namespace NAudioPlayer
{
    public class Player : IWaveformPlayer, IDisposable
    {
        #region Fields
        private static Player instance;
        private readonly DispatcherTimer positionTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
        private readonly BackgroundWorker waveformGenerateWorker = new BackgroundWorker();
        private readonly int fftDataSize = 256;
        private bool disposed;
        private bool inChannelTimerUpdate;

        private WaveOut waveOutDevice;
        private WaveStream activeStream;
        private WaveChannel32 inputStream;

        private SampleAggregator sampleAggregator;
        private SampleAggregator waveformAggregator;
        private string pendingWaveformPath;
        private float[] fullLevelData;
        private float[] waveformData;
        #endregion

        #region Constants
        private const int waveformCompressedPointCount = 5000;
        #endregion

        #region Singleton Pattern
        public static Player Instance {
            get {
                if (instance == null)
                    instance = new Player();
                return instance;
            }
        }
        #endregion

        #region Constructor
        private Player()
        {
            positionTimer.Interval = TimeSpan.FromMilliseconds(50);
            positionTimer.Tick += positionTimer_Tick;
            waveformGenerateWorker.DoWork += waveformGenerateWorker_DoWork;
            waveformGenerateWorker.RunWorkerCompleted += waveformGenerateWorker_RunWorkerCompleted;
            waveformGenerateWorker.WorkerSupportsCancellation = true;
        }

        //ВРЕМЕННО
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
            if (!disposed) {
                if (disposing) {
                    StopAndCloseStream();
                }

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
                double oldValue = channelLength;
                SetField(ref channelLength, value, () => ChannelLength);
            }
        }

        private bool inChannelSet;
        private double channelPosition;
        public double ChannelPosition {
            get { return channelPosition; }
            set {
                if (!inChannelSet) {
                    inChannelSet = true; // Avoid recursion                    
                    double oldValue = channelPosition;
                    double position = Math.Max(0, Math.Min(value, ChannelLength));
                    if(!inChannelTimerUpdate && ActiveStream != null) {
                        ActiveStream.Position = (long)((position / ActiveStream.TotalTime.TotalSeconds) * ActiveStream.Length);
                    }
                    SetField(ref channelPosition, position, () => ChannelPosition);
                    inChannelSet = false;
                }
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged<T>(Expression<Func<T>> selectorExpression)
        {
            if (selectorExpression == null) {
                throw new ArgumentNullException("selectorExpression should not be null");
            }
            MemberExpression body = selectorExpression.Body as MemberExpression;
            if (body == null) {
                throw new ArgumentException("The body must be a member expression");
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(body.Member.Name));
        }

        protected bool SetField<T>(ref T field, T value, Expression<Func<T>> selectorExpression)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) {
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
            public WaveformGenerationParams(int points, string path)
            {
                Points = points;
                Path = path;
            }

            public int Points { get; protected set; }
            public string Path { get; protected set; }
        }

        private void GenerateWaveformData(string path)
        {
            if (waveformGenerateWorker.IsBusy) {
                pendingWaveformPath = path;
                waveformGenerateWorker.CancelAsync();
                return;
            }

            if (!waveformGenerateWorker.IsBusy && waveformCompressedPointCount != 0)
                waveformGenerateWorker.RunWorkerAsync(new WaveformGenerationParams(waveformCompressedPointCount, path));
        }

        private void waveformGenerateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) {
                if (!waveformGenerateWorker.IsBusy && waveformCompressedPointCount != 0)
                    waveformGenerateWorker.RunWorkerAsync(new WaveformGenerationParams(waveformCompressedPointCount, pendingWaveformPath));
            }
        }

        private void waveformGenerateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            WaveformGenerationParams waveformParams = e.Argument as WaveformGenerationParams;
            Mp3FileReader waveformMp3Stream = new Mp3FileReader(waveformParams.Path);
            WaveChannel32 waveformInputStream = new WaveChannel32(waveformMp3Stream);
            waveformInputStream.Sample += waveStream_Sample;

            int frameCount = (int)((double)waveformInputStream.Length / (double)fftDataSize);
            int waveformLength = frameCount * 2;
            byte[] readBuffer = new byte[fftDataSize];
            waveformAggregator = new SampleAggregator(fftDataSize);

            float maxLeftPointLevel = float.MinValue;
            float maxRightPointLevel = float.MinValue;
            int currentPointIndex = 0;
            float[] waveformCompressedPoints = new float[waveformParams.Points];
            List<float> waveformData = new List<float>();
            List<int> waveMaxPointIndexes = new List<int>();

            for (int i = 1; i <= waveformParams.Points; i++) {
                waveMaxPointIndexes.Add((int)Math.Round(waveformLength * ((double)i / (double)waveformParams.Points), 0));
            }
            int readCount = 0;
            while (currentPointIndex * 2 < waveformParams.Points) {
                waveformInputStream.Read(readBuffer, 0, readBuffer.Length);

                waveformData.Add(waveformAggregator.LeftMaxVolume);
                waveformData.Add(waveformAggregator.RightMaxVolume);

                if (waveformAggregator.LeftMaxVolume > maxLeftPointLevel)
                    maxLeftPointLevel = waveformAggregator.LeftMaxVolume;
                if (waveformAggregator.RightMaxVolume > maxRightPointLevel)
                    maxRightPointLevel = waveformAggregator.RightMaxVolume;

                if (readCount > waveMaxPointIndexes[currentPointIndex]) {
                    waveformCompressedPoints[(currentPointIndex * 2)] = maxLeftPointLevel;
                    waveformCompressedPoints[(currentPointIndex * 2) + 1] = maxRightPointLevel;
                    maxLeftPointLevel = float.MinValue;
                    maxRightPointLevel = float.MinValue;
                    currentPointIndex++;
                }
                if (readCount % 3000 == 0) {
                    float[] clonedData = (float[])waveformCompressedPoints.Clone();
                    dispatcher.Invoke(new Action(() =>
                    {
                        WaveformData = clonedData;
                    }));
                }

                if (waveformGenerateWorker.CancellationPending) {
                    e.Cancel = true;
                    break;
                }
                readCount++;
            }

            float[] finalClonedData = (float[])waveformCompressedPoints.Clone();
            dispatcher.Invoke(new Action(() =>
            {
                fullLevelData = waveformData.ToArray();
                WaveformData = finalClonedData;
            }));
            waveformInputStream.Close();
            waveformInputStream.Dispose();
            waveformInputStream = null;
            waveformMp3Stream.Close();
            waveformMp3Stream.Dispose();
            waveformMp3Stream = null;
        }
        #endregion

        #region Private Utility Methods
        private void StopAndCloseStream()
        {
            if (waveOutDevice != null) {
                waveOutDevice.Stop();
            }
            if (activeStream != null) {
                inputStream.Close();
                inputStream = null;
                ActiveStream.Close();
                ActiveStream = null;
            }
            if (waveOutDevice != null) {
                waveOutDevice.Dispose();
                waveOutDevice = null;
            }
            IsInitialized = false;
        }
        #endregion

        #region Public Methods
        public void Stop()
        {
            if (waveOutDevice != null) {
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

        public void Pause()
        {
            if (IsPlaying && CanPause) {
                waveOutDevice.Pause();
                IsPlaying = false;
                CanPlay = true;
                CanPause = false;
            }
        }

        public void Play()
        {
            if (CanPlay) {
                waveOutDevice.Play();
                IsPlaying = true;
                CanPause = true;
                CanPlay = false;
                CanStop = true;
            }
        }

        public void OpenFile(string path)
        {
            Stop();

            if (ActiveStream != null) {
                ChannelPosition = 0;
            }

            StopAndCloseStream();

            if (System.IO.File.Exists(path)) {
                try {
                    WaveCallbackInfo waveCallbackInfo = WaveCallbackInfo.FunctionCallback();
                    waveOutDevice = new WaveOut(waveCallbackInfo)
                    {
                        DesiredLatency = 100
                    };
                    ActiveStream = new Mp3FileReader(path);
                    inputStream = new WaveChannel32(ActiveStream);
                    sampleAggregator = new SampleAggregator(fftDataSize);
                    inputStream.Sample += inputStream_Sample;
                    waveOutDevice.Init(inputStream);
                    ChannelLength = inputStream.TotalTime.TotalSeconds;
                    GenerateWaveformData(path);
                    CanPlay = true;
                    IsInitialized = true;
                }
                catch {
                    ActiveStream = null;
                    CanPlay = false;
                    IsInitialized = false;
                }
            }
        }
        #endregion

        #region Public Properties

        private bool isInitialized;
        public bool IsInitialized {
            get => isInitialized;
            set => SetField(ref isInitialized, value, () => IsInitialized);
        }


        public WaveStream ActiveStream {
            get { return activeStream; }
            protected set {
                SetField(ref activeStream, value, () => ActiveStream);
            }
        }

        public float Volume {
            get {
                if (waveOutDevice == null) {
                    return 0f;
                }
                return waveOutDevice.Volume;
            }
            set {
                if (waveOutDevice == null) {
                    return;
                }
                float volume = 0f;
                if (SetField(ref volume, value, () => Volume)) {
                    waveOutDevice.Volume = volume;
                }
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
}
