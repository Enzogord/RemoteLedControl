namespace RemoteLEDServer
{
    partial class FormCreateProject
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_Ok = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_ProjectKey = new System.Windows.Forms.TextBox();
            this.button_GenerateKey = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_Ok
            // 
            this.button_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_Ok.Enabled = false;
            this.button_Ok.Location = new System.Drawing.Point(26, 91);
            this.button_Ok.Name = "button_Ok";
            this.button_Ok.Size = new System.Drawing.Size(75, 23);
            this.button_Ok.TabIndex = 0;
            this.button_Ok.Text = "Ок";
            this.button_Ok.UseVisualStyleBackColor = true;
            this.button_Ok.Click += new System.EventHandler(this.button_Ok_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(107, 91);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 1;
            this.button_Cancel.Text = "Отмена";
            this.button_Cancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(23, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Необходимо ввести ключ проекта:";
            // 
            // textBox_ProjectKey
            // 
            this.textBox_ProjectKey.Location = new System.Drawing.Point(26, 44);
            this.textBox_ProjectKey.Name = "textBox_ProjectKey";
            this.textBox_ProjectKey.Size = new System.Drawing.Size(209, 20);
            this.textBox_ProjectKey.TabIndex = 3;
            this.textBox_ProjectKey.TextChanged += new System.EventHandler(this.textBox_ProjectKey_TextChanged);
            // 
            // button_GenerateKey
            // 
            this.button_GenerateKey.Location = new System.Drawing.Point(241, 42);
            this.button_GenerateKey.Name = "button_GenerateKey";
            this.button_GenerateKey.Size = new System.Drawing.Size(104, 24);
            this.button_GenerateKey.TabIndex = 4;
            this.button_GenerateKey.Text = "Сгенерировать";
            this.button_GenerateKey.UseVisualStyleBackColor = true;
            this.button_GenerateKey.Click += new System.EventHandler(this.button_GenerateKey_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Число от 1 до 4 294 967 295";
            // 
            // FormCreateProject
            // 
            this.AcceptButton = this.button_Ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSkyBlue;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(368, 130);
            this.Controls.Add(this.button_GenerateKey);
            this.Controls.Add(this.textBox_ProjectKey);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormCreateProject";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormCreateProject";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Ok;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_ProjectKey;
        private System.Windows.Forms.Button button_GenerateKey;
        private System.Windows.Forms.Label label2;
    }
}