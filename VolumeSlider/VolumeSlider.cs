using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VolumeSlider
{
    /// <summary>
    /// VolumeSlider control
    /// </summary>
    public class VolumeSlider : System.Windows.Forms.UserControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private int volume = 100;
        //private float MinDb = -48;
        /// <summary>
        /// Volume changed event
        /// </summary>
        public event EventHandler VolumeChanged;

        /// <summary>
        /// Creates a new VolumeSlider control
        /// </summary>
        public VolumeSlider()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            DoubleBuffered = true;

            // TODO: Add any initialization after the InitComponent call
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        public Color FillColor { get; set; }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // VolumeSlider
            // 
            this.Name = "VolumeSlider";
            this.FillColor = Color.LightGreen;
            this.Size = new System.Drawing.Size(96, 16);

        }
        #endregion

        /// <summary>
        /// <see cref="Control.OnPaint"/>
        /// </summary>
        protected override void OnPaint(PaintEventArgs pe)
        {
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            pe.Graphics.DrawRectangle(Pens.Black, 0, 0, this.Width - 1, this.Height - 1);
            //float db = 20 * (float)Math.Log10(Volume);
            float percent = (float)volume / (float)100;

            pe.Graphics.FillRectangle(new SolidBrush(FillColor), 1, 1, (int)((this.Width - 2) * percent), this.Height - 2);
            string dbValue = String.Format("{0} %", Volume);
            /*if(Double.IsNegativeInfinity(db))
            {
                dbValue = "-\x221e db"; // -8 dB
            }*/

            pe.Graphics.DrawString(dbValue, this.Font,
                Brushes.Black, this.ClientRectangle, format);
            // Calling the base class OnPaint
            //base.OnPaint(pe);
        }

        /// <summary>
        /// <see cref="Control.OnMouseMove"/>
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SetVolumeFromMouse(e.X);
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// <see cref="Control.OnMouseDown"/>
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            SetVolumeFromMouse(e.X);
            base.OnMouseDown(e);
        }

        private void SetVolumeFromMouse(int x)
        {
            // linear Volume = (float) x / this.Width;
            float perc = (float)x / (float)(this.Width-2);
            if (x <= 0)
                Volume = 0;
            else
                Volume = (int)(perc * 100);
        }

        /// <summary>
        /// The volume for this control
        /// </summary>
        [DefaultValue(100)]
        public int Volume
        {
            get
            {
                return volume;
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                if (volume != value)
                {
                    volume = value;
                    if (VolumeChanged != null)
                        VolumeChanged(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }
    }

}
