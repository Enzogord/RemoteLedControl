using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using CSCore.Streams;

namespace RLCPlayer
{
    public partial class RLCPlayer : UserControl
    {
        public Point PlayerTrackPosition;
        public Size PlayerTrackSize;
        public int PlayerTrack_TopBottomMargin;
        public int PlayerTrack_LeftRightMargin;
        public int PlayerTrackBorderSize;
        public Color PlayerTrackBackColor { get; set; }
        public Color PlayerTrackBorderColor { get; set; }
        public Color PlayerTrackBorderHoverColor { get; set; }
        public Color PlayerTrackTrackColor { get; set; }
        public int PlayerTrackTrackProgressWidth;
        public bool PlayerTrackMouseOver;
        public bool MouseInPlayerTrack(Point point)
        {
            bool result = false;
            if ((point.X >= PlayerTrackPosition.X) && (point.X < PlayerTrackPosition.X + PlayerTrackSize.Width) && (point.Y >= PlayerTrackPosition.Y) && (point.Y < PlayerTrackPosition.Y + PlayerTrackSize.Height))
            {
                result = true;
            }
            PlayerTrackMouseOver = result;
            return result;
        }
        private double PercentX(Point point)
        {
            double result;

            if (point.X == PlayerTrackPosition.X)
            {
                result = 0D;
            }
            else if (point.X == PlayerTrackPosition.X + PlayerTrackSize.Width - 1)
            {
                result = 1D;
            }
            else
            {
                result = Math.Round(((double)(point.X - PlayerTrackPosition.X) / (double)PlayerTrackSize.Width), 5);
            }

            return result;
        }

        // Fields
        //      Common
        public bool CanClickPlayerTrack = true;
        private readonly CSMusicPlayer _musicPlayer = new CSMusicPlayer();
        private MMDevice FDevice;
        public readonly ObservableCollection<MMDevice> _devices = new ObservableCollection<MMDevice>();
        private Timer Timer;
        private StringFormat StringFormat;
        private StringFormat StringFormatLeft;
        private Point FMouseCoordinate;
        private double Percent = 0D;
        private bool FDragingFile;

        //      Times
        private string FCurrentTimeString;
        private Size FCurrentTimeStringSize;
        private Point FCurrentTimeStringPosition;
        private TimeSpan FHoverTime;
        private string FHoverTimeString;
        private string FProgressTimeString;
        private Size FProgressTimeStringSize;

        //      Labels
        private int TopLabelHeight;
        private Point TopLabelPosition;
        private int BottomLabelHeight;
        private Point BottomLabelPosition;
        private int FileLabelHeight;
        private Point FileLabelPosition;
        private int FLabelsVertMargin;
        private string FFileLabelString;
        private int FLabelStringHorizMargin;

        //      PlayerTrack
        private int FPlayerTrackTopBottomMargin;
        private int FPlayerTrackLeftRightMargin;
        private int FPlayerTrackHeight;

        // Properties
        public MMDevice Device
        {
            get { return FDevice; }
            set
            {
                FDevice = value;
                ChangeDevice();
            }
        }
        private bool DragingFile
        {
            get { return FDragingFile; }
            set
            {
                if (FDragingFile != value)
                {
                    FDragingFile = value;
                    Invalidate();
                }
            }
        }
        private Size CurrentTimeStringSize
        {
            get { return FCurrentTimeStringSize; }
            set { FCurrentTimeStringSize = value; }
        }
        private Point CurrentTimeStringPosition
        {
            get { return FCurrentTimeStringPosition; }
            set { FCurrentTimeStringPosition = value; }
        }
        public TimeSpan TotalTime
        {
            get
            {
                if (_musicPlayer != null)
                {
                    return _musicPlayer.Length;
                }
                else
                {
                    return TimeSpan.Zero;
                }

            }
        }
        public TimeSpan CurrentTime
        {
            get
            {
                if (_musicPlayer != null)
                {
                    return _musicPlayer.Position;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
            set
            {
                if (_musicPlayer != null)
                {
                    _musicPlayer.Position = value;
                    GenerateCurrentTimeString();
                    SetProgressTimeStringPosition();
                }
            }
        }
        public string CurrentTimeString
        {
            get
            {
                return FCurrentTimeString;
            }
            set
            {
                FCurrentTimeString = value;
            }
        }
        private Point MouseCoordinate
        {
            get
            {
                return FMouseCoordinate;
            }
            set
            {
                FMouseCoordinate = value;
                MouseInPlayerTrack(FMouseCoordinate);
            }
        }
        public int LabelsVertMargin
        {
            get
            {
                return FLabelsVertMargin;
            }
            set
            {
                FLabelsVertMargin = value;
                GenerateElementsPositions();
            }
        }
        private string FileLabelString
        {
            get { return FFileLabelString; }
            set { FFileLabelString = value; }
        }
        public string CurrentAudioFile
        {
            get { return FFileLabelString; }
        }
        private int LabelStringHorizMargin
        {
            get { return FLabelStringHorizMargin; }
            set { FLabelStringHorizMargin = value; }
        }
        public int PlayerTrackTopBottomMargin
        {
            get { return FPlayerTrackTopBottomMargin; }
            set { FPlayerTrackTopBottomMargin = value; }
        }
        public int PlayerTrackLeftRightMargin
        {
            get { return FPlayerTrackLeftRightMargin; }
            set { FPlayerTrackLeftRightMargin = value; }
        }
        public int PlayerTrackHeight
        {
            get { return FPlayerTrackHeight; }
            set { FPlayerTrackHeight = value; }
        }

        public delegate void DMouseSetTime();
        public event DMouseSetTime OnMouseSetTime;
        public delegate void DInitializedPlayer(string FilePath);
        public event DInitializedPlayer OnInitializedPlayer;

        // Methods

        /// <summary>
        /// Расчитывает расположение элементов
        /// </summary>
        private void GenerateElementsPositions()
        {
            TopLabelHeight = Font.Height;
            TopLabelPosition = new Point(0, LabelsVertMargin);
            PlayerTrack_TopBottomMargin = PlayerTrackTopBottomMargin;
            PlayerTrack_LeftRightMargin = PlayerTrackLeftRightMargin;
            PlayerTrackPosition = new Point(PlayerTrack_LeftRightMargin + PlayerTrackBorderSize, TopLabelHeight + (LabelsVertMargin * 2) + PlayerTrack_TopBottomMargin + PlayerTrackBorderSize);
            PlayerTrackSize = new Size(Width - (PlayerTrack_LeftRightMargin * 2) - (PlayerTrackBorderSize * 2), PlayerTrackHeight - (PlayerTrackBorderSize * 2));
            BottomLabelHeight = Font.Height;
            BottomLabelPosition = new Point(0, PlayerTrackPosition.Y + PlayerTrackSize.Height + PlayerTrack_TopBottomMargin + LabelsVertMargin + PlayerTrackBorderSize);
            //SetCurrentTimeStringPosition();
            SetProgressTimeStringPosition();
            FileLabelHeight = Font.Height;
            FileLabelPosition = new Point(PlayerTrackPosition.X - 2, BottomLabelPosition.Y);
            SetTrackProgressWidth();
            Invalidate();
        }

        private void GenerateCurrentTimeString()
        {
            if (_musicPlayer != null)
            {
                FProgressTimeString = String.Format("{0,2}:{1,2}", _musicPlayer.Position.Minutes.ToString("D2"), _musicPlayer.Position.Seconds.ToString("D2")) + " / " +
                        String.Format("{0,2}:{1,2}", _musicPlayer.Length.Minutes.ToString("D2"), _musicPlayer.Length.Seconds.ToString("D2"));
            }
        }
        
        /// <summary>
        /// Устанавливает время по курсору на трекбаре
        /// </summary>
        private void SetTimeFromMouse(Point point)
        {
            if (MouseInPlayerTrack(point))
            {
                Percent = PercentX(point);
                SetTrackProgressWidth();
                CurrentTime = TimeSpan.FromMilliseconds(TotalTime.TotalMilliseconds * Percent);
            }
        }

        private void SetTrackProgressWidth()
        {
            if (Percent == 1D)
            {
                PlayerTrackTrackProgressWidth = PlayerTrackSize.Width;
            }
            else
            {
                PlayerTrackTrackProgressWidth = (int)Math.Ceiling(Math.Round((PlayerTrackSize.Width + 1) * Percent, 2));
            }
        }

        /// <summary>
        /// Расчитывает расположение строки текущего времени
        /// </summary>
        private void SetCurrentTimeStringPosition()
        {
            int k = PlayerTrackPosition.X + PlayerTrackTrackProgressWidth - 1;
            CurrentTimeStringSize = TextRenderer.MeasureText(FCurrentTimeString, this.Font);
            float fMin = k - (CurrentTimeStringSize.Width / 2);
            float fMax = k + (CurrentTimeStringSize.Width / 2);
            int PlayerTrackEnd = PlayerTrackPosition.X + PlayerTrackSize.Width + PlayerTrackBorderSize;
            if (fMin <= PlayerTrackPosition.X - 3)
            {
                BottomLabelPosition = new Point(PlayerTrackPosition.X + (int)(CurrentTimeStringSize.Width / 2) - 3, BottomLabelPosition.Y);
            }
            else if (fMax >= PlayerTrackEnd)
            {
                BottomLabelPosition = new Point(PlayerTrackEnd - (int)(CurrentTimeStringSize.Width / 2) + 2, BottomLabelPosition.Y);
            }
            else
            {
                BottomLabelPosition = new Point(k + 1, BottomLabelPosition.Y);
            }
        }

        private void SetProgressTimeStringPosition()
        {
            int k = PlayerTrackPosition.X + PlayerTrackTrackProgressWidth - 1;
            FProgressTimeStringSize = TextRenderer.MeasureText(FProgressTimeString, this.Font);
            int PlayerTrackEnd = PlayerTrackPosition.X + PlayerTrackSize.Width;
            BottomLabelPosition = new Point(PlayerTrackEnd - FProgressTimeStringSize.Width + 7, BottomLabelPosition.Y);
        }

        /// <summary>
        /// Показывает время при наведении мыши на трекбар
        /// </summary>
        private void TimeOnMouse(Point point)
        {
            if (MouseInPlayerTrack(point))
            {
                FHoverTime = TimeSpan.FromMilliseconds(TotalTime.TotalMilliseconds * PercentX(point));
                FHoverTimeString = String.Format("{0,2}:{1,2}", FHoverTime.Minutes.ToString("D2"), FHoverTime.Seconds.ToString("D2"));
                Size size = TextRenderer.MeasureText(FHoverTimeString, this.Font);
                float fMin = MouseCoordinate.X - (size.Width / 2);
                float fMax = MouseCoordinate.X + (size.Width / 2);
                int PlayerTrackEnd = PlayerTrackPosition.X + PlayerTrackSize.Width + PlayerTrackBorderSize;
                if (fMin <= PlayerTrackPosition.X - 3)
                {
                    TopLabelPosition = new Point(PlayerTrackPosition.X + (int)(size.Width / 2) - 3, TopLabelPosition.Y);
                }
                else if (fMax >= PlayerTrackEnd)
                {
                    TopLabelPosition = new Point(PlayerTrackEnd - (int)(size.Width / 2) + 2, TopLabelPosition.Y);
                }
                else
                {
                    TopLabelPosition = new Point((int)(MouseCoordinate.X + 1), TopLabelPosition.Y);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GenerateElementsPositions();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            GenerateElementsPositions();
            base.OnFontChanged(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            MouseCoordinate = e.Location;
            TimeOnMouse(e.Location);
            Invalidate();
            //if (e.Button == MouseButtons.Left)
            //{
            //    SetTimeFromMouse(e.Location);
            //}
            base.OnMouseMove(e);
        }

        /// <summary>
        /// <see cref="Control.OnMouseDown"/>
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (CanClickPlayerTrack)
            {
                SetTimeFromMouse(e.Location);
                Invalidate();
                OnMouseSetTime?.Invoke();
            }
            base.OnMouseDown(e);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            DragingFile = false;
            base.OnDragLeave(e);
        }

        /// <summary>
        /// <see cref="Control.OnPaint"/>
        /// </summary>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Заливка компонента
            pe.Graphics.FillRectangle(new SolidBrush(BackColor), 0, 0, this.Width, this.Height);
            // Граница трекбара
            if (PlayerTrackMouseOver)
            {
                pe.Graphics.DrawRectangle(new Pen(PlayerTrackBorderHoverColor, PlayerTrackBorderSize), PlayerTrackPosition.X, PlayerTrackPosition.Y, PlayerTrackSize.Width, PlayerTrackSize.Height);
            }
            else
            {
                pe.Graphics.DrawRectangle(new Pen(PlayerTrackBorderColor, PlayerTrackBorderSize), PlayerTrackPosition.X, PlayerTrackPosition.Y, PlayerTrackSize.Width, PlayerTrackSize.Height);
            }
            // Заливка трекбара 
            pe.Graphics.FillRectangle(new SolidBrush(PlayerTrackBackColor), PlayerTrackPosition.X, PlayerTrackPosition.Y, PlayerTrackSize.Width, PlayerTrackSize.Height);

            // Заливка прогресса трекабара
            pe.Graphics.FillRectangle(new SolidBrush(PlayerTrackTrackColor), PlayerTrackPosition.X, PlayerTrackPosition.Y, PlayerTrackTrackProgressWidth, PlayerTrackSize.Height);

            if (_musicPlayer != null)
            {
                // Отображение времени на курсоре и курсора
                if (PlayerTrackMouseOver)
                {
                    pe.Graphics.DrawString(FHoverTimeString, this.Font, Brushes.Black, TopLabelPosition, StringFormat);
                    pe.Graphics.DrawLine(new Pen(Color.Black, 1), new Point(MouseCoordinate.X, PlayerTrackPosition.Y - PlayerTrack_TopBottomMargin), new Point(MouseCoordinate.X, PlayerTrackPosition.Y + PlayerTrackSize.Height + PlayerTrack_TopBottomMargin));
                }

                // Отображение времени воспроизведения
                pe.Graphics.DrawString(FProgressTimeString, this.Font, Brushes.Black, BottomLabelPosition, StringFormatLeft);

                // Отображение названия файла
                pe.Graphics.DrawString(FFileLabelString, this.Font, Brushes.Black, new RectangleF(FileLabelPosition, new SizeF(PlayerTrackSize.Width - FProgressTimeStringSize.Width, this.Font.Height)), StringFormatLeft);
            }

            if (DragingFile)
            {
                pe.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Silver)), 0, 0, this.Width, this.Height);
            }
        }

        #region Mp3Player

        public int Volume
        {
            get
            {
                if (_musicPlayer != null)
                {
                    return _musicPlayer.Volume;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (_musicPlayer != null)
                {
                    _musicPlayer.Volume = value;
                }
            }
        }

        private void MP3DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                   ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move))
            {
                string[] objects = null;
                objects = (string[])e.Data.GetData(DataFormats.FileDrop);
                if ((objects.Length == 1) & (string.Equals(Path.GetExtension(objects[0]), ".mp3", StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (CodecFactory.Instance.GetSupportedFileExtensions().Contains(Path.GetExtension(objects[0]).Substring(1)))
                    {
                        e.Effect = DragDropEffects.Move;
                        (sender as Control).DragDrop -= MP3DragDrop;
                        (sender as Control).DragDrop += MP3DragDrop;
                        DragingFile = true;
                    }
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    DragingFile = false;
                }
            }
        }

        private void MP3DragDrop(object sender, DragEventArgs e)
        {
            string[] objects = (string[])e.Data.GetData(DataFormats.FileDrop);
            InitializePlayer(objects[0], Device);
            (sender as Control).DragDrop -= MP3DragDrop;
            DragingFile = false;
        }

        public void Play()
        {
            if (_musicPlayer.PlaybackState != PlaybackState.Playing)
            {
                _musicPlayer.Play();
            }
        }

        public void Pause()
        {
            if (_musicPlayer.PlaybackState == PlaybackState.Playing)
            {
                _musicPlayer.Pause();
            }
        }

        public void Stop()
        {
            if (_musicPlayer.PlaybackState != PlaybackState.Stopped)
            {
                _musicPlayer.Stop();
                _musicPlayer.Position = TimeSpan.Zero;
            }
        }

        public TimeSpan Position()
        {
            return _musicPlayer.Position;
        }

        //public void OnPlaybackStop(object sender, StoppedEventArgs e)
        //{
        //    Mp3Reader.CurrentTime = TimeSpan.FromMilliseconds(0);
        //}

        public PlaybackState PlaybackStateStr
        {
            get { return _musicPlayer.PlaybackState; }
        }

        public void ChangeDevice()
        {
            if (FileLabelString != null)
            {
                InitializePlayer(FileLabelString, Device);
            }
        }

        private void InitializePlayer(string MP3Path, MMDevice device)
        {
            try
            {
                _musicPlayer.Open(MP3Path, device);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open file: " + ex.Message);
            }
            FileLabelString = MP3Path;
            OnInitializedPlayer?.Invoke(MP3Path);
        }

        public void InitializePlayer(string MusicFilePath)
        {
            InitializePlayer(MusicFilePath, Device);
        }

        #endregion     

        public RLCPlayer()
        {
            InitializeComponent();
            components.Add(_musicPlayer);

            using (var mmdeviceEnumerator = new MMDeviceEnumerator())
            {
                using (
                    var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
                {
                    foreach (var device in mmdeviceCollection)
                    {
                        _devices.Add(device);
                    }
                }
            }
            Device = _devices[0];
            _musicPlayer.PlaybackStopped += _musicPlayer_PlaybackStopped;
            this.Timer = new System.Windows.Forms.Timer();
            this.Timer.Interval = 100;
            this.Timer.Tick += Timer_Tick;
            this.Timer.Enabled = true;
            StringFormat = new StringFormat();
            StringFormat.Alignment = StringAlignment.Center;
            StringFormatLeft = new StringFormat();
            StringFormatLeft.Alignment = StringAlignment.Near;
            this.AllowDrop = true;
            this.DragEnter += MP3DragEnter;
            GenerateElementsPositions();
        }

        private void _musicPlayer_PlaybackStopped(object sender, PlaybackStoppedEventArgs e)
        {
            _musicPlayer.Position = TimeSpan.Zero;
            SetTrackProgressWidth();
            GenerateCurrentTimeString();
            Invalidate();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_musicPlayer != null)
            {
                Percent = _musicPlayer.Position.TotalMilliseconds / _musicPlayer.Length.TotalMilliseconds;
                SetTrackProgressWidth();
                GenerateCurrentTimeString();
                Invalidate();
            }
        }
    }
}
