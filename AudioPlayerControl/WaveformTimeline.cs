﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AudioPlayer.TimeLine
{
    [ToolboxItem(nameof(WaveformTimeline))]
    [TemplatePart(Name = "PART_Waveform", Type = typeof(Canvas)),
     TemplatePart(Name = "PART_Timeline", Type = typeof(Canvas)),
     TemplatePart(Name = "PART_Progress", Type = typeof(Canvas))]
    public class WaveformTimeline : Control
    {
        #region Fields
        private Canvas waveformCanvas;
        private Canvas timelineCanvas;
        private Canvas progressCanvas;
        private readonly Path leftPath = new Path();
        private readonly Path rightPath = new Path();
        private readonly Line centerLine = new Line();

        private readonly Line progressLine = new Line();
        private readonly Path progressIndicator = new Path();

        private readonly Line positionSelectorLine = new Line();
        private readonly Path positionSelectorIndicator = new Path();

        private readonly List<Line> timeLineTicks = new List<Line>();
        private readonly Rectangle timelineBackgroundRegion = new Rectangle();
        private readonly List<TextBlock> timestampTextBlocks = new List<TextBlock>();
        private bool isMouseDown;
        private Point mouseDownPoint;
        private Point currentPoint;
        #endregion

        #region Constants
        private const int mouseMoveTolerance = 3;
        private const int indicatorTriangleWidth = 4;
        private const int majorTickHeight = 10;
        private const int minorTickHeight = 3;
        private const int timeStampMargin = 5;
        #endregion

        #region Dependency Properties
        #region LeftLevelBrush
        /// <summary>
        /// Identifies the <see cref="LeftLevelBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LeftLevelBrushProperty = DependencyProperty.Register("LeftLevelBrush", typeof(Brush), typeof(WaveformTimeline), new UIPropertyMetadata(new SolidColorBrush(Colors.Blue), OnLeftLevelBrushChanged, OnCoerceLeftLevelBrush));

        private static object OnCoerceLeftLevelBrush(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceLeftLevelBrush((Brush)value);
            else
                return value;
        }

        private static void OnLeftLevelBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnLeftLevelBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="LeftLevelBrush"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="LeftLevelBrush"/></param>
        /// <returns>The adjusted value of <see cref="LeftLevelBrush"/></returns>
        protected virtual Brush OnCoerceLeftLevelBrush(Brush value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="LeftLevelBrush"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="LeftLevelBrush"/></param>
        /// <param name="newValue">The new value of <see cref="LeftLevelBrush"/></param>
        protected virtual void OnLeftLevelBrushChanged(Brush oldValue, Brush newValue)
        {
            leftPath.Fill = LeftLevelBrush;
            UpdateWaveform();
        }

        /// <summary>
        /// Gets or sets a brush used to draw the left channel output on the waveform.
        /// </summary>        
        [Category("Brushes")]
        public Brush LeftLevelBrush {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get {
                return (Brush)GetValue(LeftLevelBrushProperty);
            }
            set {
                SetValue(LeftLevelBrushProperty, value);
            }
        }
        #endregion

        #region RightLevelBrush
        /// <summary>
        /// Identifies the <see cref="RightLevelBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty RightLevelBrushProperty = DependencyProperty.Register("RightLevelBrush", typeof(Brush), typeof(WaveformTimeline), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), OnRightLevelBrushChanged, OnCoerceRightLevelBrush));

        private static object OnCoerceRightLevelBrush(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceRightLevelBrush((Brush)value);
            else
                return value;
        }

        private static void OnRightLevelBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnRightLevelBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="RightLevelBrush"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="RightLevelBrush"/></param>
        /// <returns>The adjusted value of <see cref="RightLevelBrush"/></returns>
        protected virtual Brush OnCoerceRightLevelBrush(Brush value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="RightLevelBrush"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="RightLevelBrush"/></param>
        /// <param name="newValue">The new value of <see cref="RightLevelBrush"/></param>
        protected virtual void OnRightLevelBrushChanged(Brush oldValue, Brush newValue)
        {
            rightPath.Fill = RightLevelBrush;
            UpdateWaveform();
        }

        /// <summary>
        /// Gets or sets a brush used to draw the right speaker levels on the waveform.
        /// </summary>
        [Category("Brushes")]
        public Brush RightLevelBrush {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get {
                return (Brush)GetValue(RightLevelBrushProperty);
            }
            set {
                SetValue(RightLevelBrushProperty, value);
            }
        }
        #endregion

        #region ProgressBarBrush
        /// <summary>
        /// Identifies the <see cref="ProgressBarBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ProgressBarBrushProperty = DependencyProperty.Register("ProgressBarBrush", typeof(Brush), typeof(WaveformTimeline), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(0xCD, 0xBA, 0x00, 0xFF)), OnProgressBarBrushChanged, OnCoerceProgressBarBrush));

        private static object OnCoerceProgressBarBrush(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceProgressBarBrush((Brush)value);
            else
                return value;
        }

        private static void OnProgressBarBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnProgressBarBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="ProgressBarBrush"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="ProgressBarBrush"/></param>
        /// <returns>The adjusted value of <see cref="ProgressBarBrush"/></returns>
        protected virtual Brush OnCoerceProgressBarBrush(Brush value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="ProgressBarBrush"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="ProgressBarBrush"/></param>
        /// <param name="newValue">The new value of <see cref="ProgressBarBrush"/></param>
        protected virtual void OnProgressBarBrushChanged(Brush oldValue, Brush newValue)
        {
            progressIndicator.Fill = ProgressBarBrush;
            progressLine.Stroke = ProgressBarBrush;

            CreateProgressIndicator();
        }

        private void CreateProgressIndicator()
        {
            if (SoundPlayer == null || timelineCanvas == null || progressCanvas == null)
                return;

            const double xLocation = 0.0d;

            progressLine.X1 = xLocation;
            progressLine.X2 = xLocation;
            progressLine.Y1 = timelineCanvas.RenderSize.Height;
            progressLine.Y2 = progressCanvas.RenderSize.Height;

            PolyLineSegment indicatorPolySegment = new PolyLineSegment();
            indicatorPolySegment.Points.Add(new Point(xLocation, timelineCanvas.RenderSize.Height));
            indicatorPolySegment.Points.Add(new Point(xLocation - indicatorTriangleWidth, timelineCanvas.RenderSize.Height - indicatorTriangleWidth));
            indicatorPolySegment.Points.Add(new Point(xLocation + indicatorTriangleWidth, timelineCanvas.RenderSize.Height - indicatorTriangleWidth));
            indicatorPolySegment.Points.Add(new Point(xLocation, timelineCanvas.RenderSize.Height));
            PathGeometry indicatorGeometry = new PathGeometry();
            PathFigure indicatorFigure = new PathFigure();
            indicatorFigure.Segments.Add(indicatorPolySegment);
            indicatorGeometry.Figures.Add(indicatorFigure);

            progressIndicator.Data = indicatorGeometry;
            UpdateProgressIndicator();
        }

        private void UpdateProgressIndicator()
        {
            if (SoundPlayer == null || progressCanvas == null)
                return;

            double xLocation = 0.0d;
            if (SoundPlayer.ChannelLength != 0) {
                double progressPercent = SoundPlayer.ChannelPosition / SoundPlayer.ChannelLength;
                xLocation = progressPercent * progressCanvas.RenderSize.Width;
            }
            progressLine.Margin = new Thickness(xLocation, 0, 0, 0);
            progressIndicator.Margin = new Thickness(xLocation, 0, 0, 0);
        }

        /// <summary>
        /// Gets or sets a brush used to draw the track progress indicator bar.
        /// </summary>
        [Category("Brushes")]
        public Brush ProgressBarBrush {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get {
                return (Brush)GetValue(ProgressBarBrushProperty);
            }
            set {
                SetValue(ProgressBarBrushProperty, value);
            }
        }
        #endregion

        #region PositionSelectorBrush

        public static readonly DependencyProperty PositionSelectorBrushProperty = 
            DependencyProperty.Register(
                "PositionSelectorBrush", 
                typeof(Brush), 
                typeof(WaveformTimeline), 
                new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(0xCD, 0xBA, 0x00, 0xFF)), OnPositionSelectorBrushChanged, OnCoercePositionSelectorBrush));

        private static object OnCoercePositionSelectorBrush(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoercePositionSelectorBrush((Brush)value);
            else
                return value;
        }

        private static void OnPositionSelectorBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnPositionSelectorBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoercePositionSelectorBrush(Brush value)
        {
            return value;
        }

        protected virtual void OnPositionSelectorBrushChanged(Brush oldValue, Brush newValue)
        {
            positionSelectorIndicator.Fill = PositionSelectorBrush;
            positionSelectorLine.Stroke = PositionSelectorBrush;

            CreatePositionSelectorIndicator();
        }

        private void CreatePositionSelectorIndicator()
        {
            if (SoundPlayer == null || timelineCanvas == null || progressCanvas == null)
                return;

            const double xLocation = 0d;

            positionSelectorLine.X1 = xLocation;
            positionSelectorLine.X2 = xLocation;
            positionSelectorLine.Y1 = timelineCanvas.RenderSize.Height;
            positionSelectorLine.Y2 = progressCanvas.RenderSize.Height;

            PolyLineSegment indicatorPolySegment = new PolyLineSegment();
            indicatorPolySegment.Points.Add(new Point(xLocation, timelineCanvas.RenderSize.Height));
            indicatorPolySegment.Points.Add(new Point(xLocation - indicatorTriangleWidth, timelineCanvas.RenderSize.Height - indicatorTriangleWidth));
            indicatorPolySegment.Points.Add(new Point(xLocation + indicatorTriangleWidth, timelineCanvas.RenderSize.Height - indicatorTriangleWidth));
            indicatorPolySegment.Points.Add(new Point(xLocation, timelineCanvas.RenderSize.Height));
            PathGeometry indicatorGeometry = new PathGeometry();
            PathFigure indicatorFigure = new PathFigure();
            indicatorFigure.Segments.Add(indicatorPolySegment);
            indicatorGeometry.Figures.Add(indicatorFigure);

            positionSelectorIndicator.Data = indicatorGeometry;
        }

        private void UpdatePositionSelectorIndicator(Point mousePosition)
        {
            bool isWaveformEnter = VisualTreeHelper.HitTest(waveformCanvas, mousePosition) != null;           

            if (isWaveformEnter && IsEnabled) {
                positionSelectorLine.Visibility = Visibility.Visible;
                positionSelectorIndicator.Visibility = Visibility.Visible;
                positionSelectorLine.Margin = new Thickness(mousePosition.X, 0, 0, 0);
                positionSelectorIndicator.Margin = new Thickness(mousePosition.X, 0, 0, 0);
            } else {
                positionSelectorLine.Visibility = Visibility.Hidden;
                positionSelectorIndicator.Visibility = Visibility.Hidden;
            }            
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            positionSelectorLine.Visibility = Visibility.Hidden;
            positionSelectorIndicator.Visibility = Visibility.Hidden;
            base.OnMouseLeave(e);
        }

        [Category("Brushes")]
        public Brush PositionSelectorBrush {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get {
                return (Brush)GetValue(PositionSelectorBrushProperty);
            }
            set {
                SetValue(PositionSelectorBrushProperty, value);
            }
        }

        #endregion

        #region ProgressBarThickness
        /// <summary>
        /// Identifies the <see cref="ProgressBarThickness" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ProgressBarThicknessProperty = DependencyProperty.Register("ProgressBarThickness", typeof(double), typeof(WaveformTimeline), new UIPropertyMetadata(2.0d, OnProgressBarThicknessChanged, OnCoerceProgressBarThickness));

        private static object OnCoerceProgressBarThickness(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceProgressBarThickness((double)value);
            else
                return value;
        }

        private static void OnProgressBarThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnProgressBarThicknessChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="ProgressBarThickness"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="ProgressBarThickness"/></param>
        /// <returns>The adjusted value of <see cref="ProgressBarThickness"/></returns>
        protected virtual double OnCoerceProgressBarThickness(double value)
        {
            value = Math.Max(value, 0.0d);
            return value;
        }

        /// <summary>
        /// Called after the <see cref="ProgressBarThickness"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="ProgressBarThickness"/></param>
        /// <param name="newValue">The new value of <see cref="ProgressBarThickness"/></param>
        protected virtual void OnProgressBarThicknessChanged(double oldValue, double newValue)
        {
            progressLine.StrokeThickness = ProgressBarThickness;
            CreateProgressIndicator();
        }

        /// <summary>
        /// Get or sets the thickness of the progress indicator bar.
        /// </summary>
        [Category("Common")]
        public double ProgressBarThickness {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get {
                return (double)GetValue(ProgressBarThicknessProperty);
            }
            set {
                SetValue(ProgressBarThicknessProperty, value);
            }
        }
        #endregion

        #region CenterLineBrush
        /// <summary>
        /// Identifies the <see cref="CenterLineBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CenterLineBrushProperty = DependencyProperty.Register("CenterLineBrush", typeof(Brush), typeof(WaveformTimeline), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), OnCenterLineBrushChanged, OnCoerceCenterLineBrush));

        private static object OnCoerceCenterLineBrush(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceCenterLineBrush((Brush)value);
            else
                return value;
        }

        private static void OnCenterLineBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnCenterLineBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="CenterLineBrush"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="CenterLineBrush"/></param>
        /// <returns>The adjusted value of <see cref="CenterLineBrush"/></returns>
        protected virtual Brush OnCoerceCenterLineBrush(Brush value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="CenterLineBrush"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="CenterLineBrush"/></param>
        /// <param name="newValue">The new value of <see cref="CenterLineBrush"/></param>
        protected virtual void OnCenterLineBrushChanged(Brush oldValue, Brush newValue)
        {
            centerLine.Stroke = CenterLineBrush;
            UpdateWaveform();
        }

        /// <summary>
        /// Gets or sets a brush used to draw the center line separating left and right levels.
        /// </summary>
        [Category("Brushes")]
        public Brush CenterLineBrush {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get {
                return (Brush)GetValue(CenterLineBrushProperty);
            }
            set {
                SetValue(CenterLineBrushProperty, value);
            }
        }
        #endregion

        #region CenterLineThickness
        /// <summary>
        /// Identifies the <see cref="CenterLineThickness" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CenterLineThicknessProperty = DependencyProperty.Register("CenterLineThickness", typeof(double), typeof(WaveformTimeline), new UIPropertyMetadata(1.0d, OnCenterLineThicknessChanged, OnCoerceCenterLineThickness));

        private static object OnCoerceCenterLineThickness(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceCenterLineThickness((double)value);
            else
                return value;
        }

        private static void OnCenterLineThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnCenterLineThicknessChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="CenterLineThickness"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="CenterLineThickness"/></param>
        /// <returns>The adjusted value of <see cref="CenterLineThickness"/></returns>
        protected virtual double OnCoerceCenterLineThickness(double value)
        {
            value = Math.Max(value, 0.0d);
            return value;
        }

        /// <summary>
        /// Called after the <see cref="CenterLineThickness"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="CenterLineThickness"/></param>
        /// <param name="newValue">The new value of <see cref="CenterLineThickness"/></param>
        protected virtual void OnCenterLineThicknessChanged(double oldValue, double newValue)
        {
            centerLine.StrokeThickness = CenterLineThickness;
            UpdateWaveform();
        }

        /// <summary>
        /// Gets or sets the thickness of the center line separating left and right levels.
        /// </summary>
        [Category("Common")]
        public double CenterLineThickness {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get {
                return (double)GetValue(CenterLineThicknessProperty);
            }
            set {
                SetValue(CenterLineThicknessProperty, value);
            }
        }
        #endregion

        #region TimelineTickBrush
        /// <summary>
        /// Identifies the <see cref="TimelineTickBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty TimelineTickBrushProperty = DependencyProperty.Register("TimelineTickBrush", typeof(Brush), typeof(WaveformTimeline), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), OnTimelineTickBrushChanged, OnCoerceTimelineTickBrush));

        private static object OnCoerceTimelineTickBrush(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceTimelineTickBrush((Brush)value);
            else
                return value;
        }

        private static void OnTimelineTickBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnTimelineTickBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="TimelineTickBrush"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="TimelineTickBrush"/></param>
        /// <returns>The adjusted value of <see cref="TimelineTickBrush"/></returns>
        protected virtual Brush OnCoerceTimelineTickBrush(Brush value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="TimelineTickBrush"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="TimelineTickBrush"/></param>
        /// <param name="newValue">The new value of <see cref="TimelineTickBrush"/></param>
        protected virtual void OnTimelineTickBrushChanged(Brush oldValue, Brush newValue)
        {
            UpdateTimeline();
        }

        /// <summary>
        /// Gets or sets a brush used to draw the tickmarks on the timeline.
        /// </summary>
        [Category("Brushes")]
        public Brush TimelineTickBrush {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get {
                return (Brush)GetValue(TimelineTickBrushProperty);
            }
            set {
                SetValue(TimelineTickBrushProperty, value);
            }
        }
        #endregion

        #region AutoScaleWaveformCache
        /// <summary>
        /// Identifies the <see cref="AutoScaleWaveformCache" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty AutoScaleWaveformCacheProperty = DependencyProperty.Register("AutoScaleWaveformCache", typeof(bool), typeof(WaveformTimeline), new UIPropertyMetadata(false, OnAutoScaleWaveformCacheChanged, OnCoerceAutoScaleWaveformCache));

        private static object OnCoerceAutoScaleWaveformCache(DependencyObject o, object value)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                return waveformTimeline.OnCoerceAutoScaleWaveformCache((bool)value);
            else
                return value;
        }

        private static void OnAutoScaleWaveformCacheChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaveformTimeline waveformTimeline = o as WaveformTimeline;
            if (waveformTimeline != null)
                waveformTimeline.OnAutoScaleWaveformCacheChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Coerces the value of <see cref="AutoScaleWaveformCache"/> when a new value is applied.
        /// </summary>
        /// <param name="value">The value that was set on <see cref="AutoScaleWaveformCache"/></param>
        /// <returns>The adjusted value of <see cref="AutoScaleWaveformCache"/></returns>
        protected virtual bool OnCoerceAutoScaleWaveformCache(bool value)
        {
            return value;
        }

        /// <summary>
        /// Called after the <see cref="AutoScaleWaveformCache"/> value has changed.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="AutoScaleWaveformCache"/></param>
        /// <param name="newValue">The new value of <see cref="AutoScaleWaveformCache"/></param>
        protected virtual void OnAutoScaleWaveformCacheChanged(bool oldValue, bool newValue)
        {
            UpdateWaveformCacheScaling();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the waveform should attempt to autoscale
        /// its render buffer in size.
        /// </summary>
        /// <remarks>
        /// If true, the control will attempt to set the waveform's bitmap cache
        /// at a resolution based on the sum of all ScaleTransforms applied
        /// in the control's visual tree heirarchy. This can make the waveform appear
        /// less blurry if a ScaleTransform is applied at a higher level.
        /// The only ScaleTransforms that are considered here are those that have 
        /// uniform vertical and horizontal scaling (generally used to "zoom in"
        /// on a window or controls).
        /// </remarks>
        [Category("Common")]
        public bool AutoScaleWaveformCache {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get {
                return (bool)GetValue(AutoScaleWaveformCacheProperty);
            }
            set {
                SetValue(AutoScaleWaveformCacheProperty, value);
            }
        }
        #endregion
        #endregion

        #region Template Overrides
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code
        /// or internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            waveformCanvas = GetTemplateChild("PART_Waveform") as Canvas;
            waveformCanvas.CacheMode = new BitmapCache();

            // Used to make the transparent regions clickable.
            waveformCanvas.Background = new SolidColorBrush(Colors.Transparent);

            waveformCanvas.Children.Add(centerLine);
            waveformCanvas.Children.Add(leftPath);
            waveformCanvas.Children.Add(rightPath);

            timelineCanvas = GetTemplateChild("PART_Timeline") as Canvas;
            timelineCanvas.Children.Add(timelineBackgroundRegion);
            timelineCanvas.SizeChanged += timelineCanvas_SizeChanged;

            progressCanvas = GetTemplateChild("PART_Progress") as Canvas;
            progressCanvas.Children.Add(progressIndicator);
            progressCanvas.Children.Add(progressLine);

            progressCanvas.Children.Add(positionSelectorIndicator);
            progressCanvas.Children.Add(positionSelectorLine);

            UpdateWaveformCacheScaling();
        }

        /// <summary>
        /// Called whenever the control's template changes. 
        /// </summary>
        /// <param name="oldTemplate">The old template</param>
        /// <param name="newTemplate">The new template</param>
        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            base.OnTemplateChanged(oldTemplate, newTemplate);
            if (waveformCanvas != null)
                waveformCanvas.Children.Clear();
            if (timelineCanvas != null) {
                timelineCanvas.SizeChanged -= timelineCanvas_SizeChanged;
                timelineCanvas.Children.Clear();
            }
            if (progressCanvas != null)
                progressCanvas.Children.Clear();
        }
        #endregion

        #region Constructor
        static WaveformTimeline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaveformTimeline), new FrameworkPropertyMetadata(typeof(WaveformTimeline)));
        }

        public WaveformTimeline()
        {
            IsEnabled = false;
        }
        #endregion

        public IWaveformPlayer SoundPlayer {
            get => (IWaveformPlayer)GetValue(SoundPlayerProperty);
            set => SetValue(SoundPlayerProperty, value);
        }

        // Using a DependencyProperty as the backing store for SoundPlayer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SoundPlayerProperty =
            DependencyProperty.Register(nameof(SoundPlayer), typeof(IWaveformPlayer), typeof(WaveformTimeline), new PropertyMetadata(null, OnSoundPlayerValuePropertyChanged));

        private static void OnSoundPlayerValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timeline = (WaveformTimeline)d;
            var player = (IWaveformPlayer)e.NewValue;
            if(player != null) {
                timeline.SubscribeSoundPlayer(player);
            }
            timeline.UpdateAllRegions();
        }

        private void SubscribeSoundPlayer(IWaveformPlayer player)
        {
            player.PropertyChanged -= soundPlayer_PropertyChanged;
            player.PropertyChanged += soundPlayer_PropertyChanged;
        }


        #region Event Handlers
        private void soundPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName) {
                case "WaveformData":
                    UpdateWaveform();
                    break;
                case "ChannelPosition":
                    UpdateProgressIndicator();
                    break;
                case "ChannelLength":
                    UpdateAllRegions();
                    break;
                case "IsEnabled":
                    IsEnabled = SoundPlayer.IsEnabled;
                    break;
            }
        }

        private void timelineCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTimeline();
        }
        #endregion

        #region Event Overrides
        /// <summary>
        /// Raises the SizeChanged event, using the specified information as part of the eventual event data. 
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateWaveformCacheScaling();
            UpdateAllRegions();
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonDown routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            isMouseDown = true;
            mouseDownPoint = e.GetPosition(waveformCanvas);
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonUp routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (!isMouseDown)
                return;

            isMouseDown = false;
            double initialPositionX = mouseDownPoint.X;
            if (Math.Abs(currentPoint.X - mouseDownPoint.X) > mouseMoveTolerance) {
                initialPositionX = currentPoint.X;
            }
            double position = (initialPositionX / RenderSize.Width) * SoundPlayer.ChannelLength;
            SoundPlayer.ChangePosition(Math.Min(SoundPlayer.ChannelLength, Math.Max(0, position)));
        }

        /// <summary>
        /// Invoked when an unhandled Mouse.MouseMove attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            currentPoint = e.GetPosition(waveformCanvas);
            UpdatePositionSelectorIndicator(e.GetPosition(waveformCanvas));
        }
        #endregion

        #region Private Utiltiy Methods
        private void UpdateWaveformCacheScaling()
        {
            if (waveformCanvas == null)
                return;

            BitmapCache waveformCache = (BitmapCache)waveformCanvas.CacheMode;
            if (AutoScaleWaveformCache) {
                double totalTransformScale = GetTotalTransformScale();
                if (waveformCache.RenderAtScale != totalTransformScale)
                    waveformCache.RenderAtScale = totalTransformScale;
            } else {
                waveformCache.RenderAtScale = 1.0d;
            }
        }

        private double GetTotalTransformScale()
        {
            double totalTransform = 1.0d;
            DependencyObject currentVisualTreeElement = this;
            do {
                Visual visual = currentVisualTreeElement as Visual;
                if (visual != null) {
                    Transform transform = VisualTreeHelper.GetTransform(visual);

                    // This condition is a way of determining if it
                    // was a uniform scale transform. Is there some better way?
                    if ((transform != null) &&
                        (transform.Value.M12 == 0) &&
                        (transform.Value.M21 == 0) &&
                        (transform.Value.OffsetX == 0) &&
                        (transform.Value.OffsetY == 0) &&
                        (transform.Value.M11 == transform.Value.M22)) {
                        totalTransform *= transform.Value.M11;
                    }
                }
                currentVisualTreeElement = VisualTreeHelper.GetParent(currentVisualTreeElement);
            }
            while (currentVisualTreeElement != null);

            return totalTransform;
        }

        private void UpdateAllRegions()
        {
            CreateProgressIndicator();
            CreatePositionSelectorIndicator();
            UpdateTimeline();
            UpdateWaveform();
        }

        private void UpdateTimeline()
        {
            if (SoundPlayer == null || timelineCanvas == null)
                return;

            foreach (TextBlock textblock in timestampTextBlocks) {
                timelineCanvas.Children.Remove(textblock);
            }
            timestampTextBlocks.Clear();

            foreach (Line line in timeLineTicks) {
                timelineCanvas.Children.Remove(line);
            }
            timeLineTicks.Clear();

            double bottomLoc = timelineCanvas.RenderSize.Height - 1;

            timelineBackgroundRegion.Width = timelineCanvas.RenderSize.Width;
            timelineBackgroundRegion.Height = timelineCanvas.RenderSize.Height;

            double minorTickDuration = 1.00d; // Major tick = 5 seconds, Minor tick = 1.00 second
            double majorTickDuration = 5.00d;
            if (SoundPlayer.ChannelLength >= 120.0d) // Major tick = 1 minute, Minor tick = 15 seconds.
            {
                minorTickDuration = 15.0d;
                majorTickDuration = 60.0d;
            } else if (SoundPlayer.ChannelLength >= 60.0d) // Major tick = 30 seconds, Minor tick = 5.0 seconds.
              {
                minorTickDuration = 5.0d;
                majorTickDuration = 30.0d;
            } else if (SoundPlayer.ChannelLength >= 30.0d) // Major tick = 10 seconds, Minor tick = 2.0 seconds.
              {
                minorTickDuration = 2.0d;
                majorTickDuration = 10.0d;
            }

            if (SoundPlayer.ChannelLength < minorTickDuration)
                return;

            int minorTickCount = (int)(SoundPlayer.ChannelLength / minorTickDuration);
            for (int i = 1; i <= minorTickCount; i++) {
                Line timelineTick = new Line()
                {
                    Stroke = TimelineTickBrush,
                    StrokeThickness = 1.0d
                };
                if (i % (majorTickDuration / minorTickDuration) == 0) // Draw Large Ticks and Timestamps at minute marks
                {
                    double xLocation = ((i * minorTickDuration) / SoundPlayer.ChannelLength) * timelineCanvas.RenderSize.Width;

                    bool drawTextBlock = false;
                    double lastTimestampEnd;
                    if (timestampTextBlocks.Count != 0) {
                        TextBlock lastTextBlock = timestampTextBlocks[timestampTextBlocks.Count - 1];
                        lastTimestampEnd = lastTextBlock.Margin.Left + lastTextBlock.ActualWidth;
                    } else
                        lastTimestampEnd = 0;

                    if (xLocation > lastTimestampEnd + timeStampMargin)
                        drawTextBlock = true;

                    // Flag that we're at the end of the timeline such 
                    // that there is not enough room for the text to draw.
                    bool isAtEndOfTimeline = (timelineCanvas.RenderSize.Width - xLocation < 28.0d);

                    if (drawTextBlock) {
                        timelineTick.X1 = xLocation;
                        timelineTick.Y1 = bottomLoc;
                        timelineTick.X2 = xLocation;
                        timelineTick.Y2 = bottomLoc - majorTickHeight;

                        if (isAtEndOfTimeline)
                            continue;

                        TimeSpan timeSpan = TimeSpan.FromSeconds(i * minorTickDuration);
                        TextBlock timestampText = new TextBlock()
                        {
                            Margin = new Thickness(xLocation + 2, 0, 0, 0),
                            FontFamily = this.FontFamily,
                            FontStyle = this.FontStyle,
                            FontWeight = this.FontWeight,
                            FontStretch = this.FontStretch,
                            FontSize = this.FontSize,
                            Foreground = this.Foreground,
                            Text = (timeSpan.TotalHours >= 1.0d) ? string.Format(@"{0:hh\:mm\:ss}", timeSpan) : string.Format(@"{0:mm\:ss}", timeSpan)
                        };
                        timestampTextBlocks.Add(timestampText);
                        timelineCanvas.Children.Add(timestampText);
                        UpdateLayout(); // Needed so that we know the width of the textblock.
                    } else // If still on the text block, draw a minor tick mark instead of a major.
                      {
                        timelineTick.X1 = xLocation;
                        timelineTick.Y1 = bottomLoc;
                        timelineTick.X2 = xLocation;
                        timelineTick.Y2 = bottomLoc - minorTickHeight;
                    }
                } else // Draw small ticks
                  {
                    double xLocation = ((i * minorTickDuration) / SoundPlayer.ChannelLength) * timelineCanvas.RenderSize.Width;
                    timelineTick.X1 = xLocation;
                    timelineTick.Y1 = bottomLoc;
                    timelineTick.X2 = xLocation;
                    timelineTick.Y2 = bottomLoc - minorTickHeight;
                }
                timeLineTicks.Add(timelineTick);
                timelineCanvas.Children.Add(timelineTick);
            }
        }

        private void UpdateWaveform()
        {
            const double minValue = 0;
            const double maxValue = 1.5;
            const double dbScale = (maxValue - minValue);

            if (SoundPlayer == null || 
                SoundPlayer.WaveformData == null || 
                waveformCanvas == null ||
                waveformCanvas.RenderSize.Width < 1 || 
                waveformCanvas.RenderSize.Height < 1
                ) {
                leftPath.Data = null;
                rightPath.Data = null;
                return;
            }

            double leftRenderHeight;
            double rightRenderHeight;

            int pointCount = (int)(SoundPlayer.WaveformData.Length / 2.0d);
            double pointThickness = waveformCanvas.RenderSize.Width / pointCount;
            double waveformSideHeight = waveformCanvas.RenderSize.Height / 2.0d;
            double centerHeight = waveformSideHeight;

            if (CenterLineBrush != null) {
                centerLine.X1 = 0;
                centerLine.X2 = waveformCanvas.RenderSize.Width;
                centerLine.Y1 = centerHeight;
                centerLine.Y2 = centerHeight;
            }

            if (SoundPlayer.WaveformData != null && SoundPlayer.WaveformData.Length > 1) {
                PolyLineSegment leftWaveformPolyLine = new PolyLineSegment();
                leftWaveformPolyLine.Points.Add(new Point(0, centerHeight));

                PolyLineSegment rightWaveformPolyLine = new PolyLineSegment();
                rightWaveformPolyLine.Points.Add(new Point(0, centerHeight));

                double xLocation = 0.0d;
                for (int i = 0; i < SoundPlayer.WaveformData.Length; i += 2) {
                    xLocation = (i / 2) * pointThickness;
                    leftRenderHeight = ((SoundPlayer.WaveformData[i] - minValue) / dbScale) * waveformSideHeight;
                    leftWaveformPolyLine.Points.Add(new Point(xLocation, centerHeight - leftRenderHeight));
                    rightRenderHeight = ((SoundPlayer.WaveformData[i + 1] - minValue) / dbScale) * waveformSideHeight;
                    rightWaveformPolyLine.Points.Add(new Point(xLocation, centerHeight + rightRenderHeight));
                }

                leftWaveformPolyLine.Points.Add(new Point(xLocation, centerHeight));
                leftWaveformPolyLine.Points.Add(new Point(0, centerHeight));
                rightWaveformPolyLine.Points.Add(new Point(xLocation, centerHeight));
                rightWaveformPolyLine.Points.Add(new Point(0, centerHeight));

                PathGeometry leftGeometry = new PathGeometry();
                PathFigure leftPathFigure = new PathFigure();
                leftPathFigure.Segments.Add(leftWaveformPolyLine);
                leftGeometry.Figures.Add(leftPathFigure);
                PathGeometry rightGeometry = new PathGeometry();
                PathFigure rightPathFigure = new PathFigure();
                rightPathFigure.Segments.Add(rightWaveformPolyLine);
                rightGeometry.Figures.Add(rightPathFigure);

                leftPath.Data = leftGeometry;
                rightPath.Data = rightGeometry;
            } else {
                leftPath.Data = null;
                rightPath.Data = null;
            }
        }
        #endregion
    }
}
