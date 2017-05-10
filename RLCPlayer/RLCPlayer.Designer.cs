using System;
using System.Drawing;
using System.ComponentModel;

namespace RLCPlayer
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner,System.Design", typeof(System.ComponentModel.Design.IDesigner))]
    partial class RLCPlayer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = new Container();

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // NAudioPlayer
            //
            this.Name = "EnzoMusicPlayer";            
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(150, 50);
            this.Size = new System.Drawing.Size(500, 50);            
            this.LabelsVertMargin = 2;            
            this.PlayerTrackHeight = 15;
            this.PlayerTrackTopBottomMargin = 4;
            this.PlayerTrackLeftRightMargin = 10;
            this.PlayerTrackBackColor = Color.White;
            this.PlayerTrackBorderSize = 2;
            this.PlayerTrackBorderColor = Color.LightGray;
            this.PlayerTrackBorderHoverColor = Color.Black;
            this.PlayerTrackTrackColor = Color.Red;
            this.LabelStringHorizMargin = 4;
            this.ResumeLayout(false);

        }

        #endregion
    }
}
