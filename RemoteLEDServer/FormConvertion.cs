using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteLEDServer
{
    public partial class FormConvertion : Form
    {
        public string InputFile;
        public string OutputFile;

        public FormConvertion()
        {
            InitializeComponent();
        }

        private void FormConvertion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.Yes && DialogResult != DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        public void ConvertCyclogramm()
        {
            string InFile = "";
            string OutFile = "";        

            while (InFile == "" || OutFile == "")
            {
                BeginInvoke(new Action(delegate ()
                {
                    InFile = InputFile;
                    OutFile = OutputFile;
                }));
            }

            //bool result = false;
            try
            {
                //progressBar_Cyclogramm.Visible = true;
                FileStream FileInput = new FileStream(InFile, FileMode.Open, FileAccess.Read);
                byte CurrentByte;
                int CurrentByteBuffer;
                byte Percent = 0;
                uint[] ArPer = new uint[100];
                ArPer[0] = (uint)(FileInput.Length / 100);
                for (int i = 1; i < 99; i++)
                {
                    ArPer[i] += ArPer[i - 1] + ArPer[0];
                }
                int ColorByteCount = 1; //счетчик от 1 до 3 (Определяет значение одного цвета)
                string colorBuffer = "";
                FileStream FileOutput = new FileStream(OutFile, FileMode.OpenOrCreate, FileAccess.Write);
                //FileOutput.Position = FileOutput.Length;
                FileOutput.Position = 0;
                while (FileInput.Position <= FileInput.Length)
                {
                    if (ArPer[Percent] == FileInput.Position)
                    {
                        BeginInvoke(new Action(delegate ()
                        {
                            progressBar1.Value = Percent;
                        }));

                        Percent++;
                    }

                    CurrentByteBuffer = FileInput.ReadByte();
                    if (CurrentByteBuffer > 0)
                    {
                        CurrentByte = (byte)(CurrentByteBuffer);
                    }
                    else
                    {
                        break;
                    }

                    // Проверка, является ли текущий байт запятой "," символом перевода на новую строку или символом возврата каретки
                    if ((CurrentByte == 44) || (CurrentByte == 13) || (CurrentByte == 10))
                        continue;

                    if (ColorByteCount <= 3)
                    {
                        colorBuffer += Convert.ToChar(CurrentByte);
                        ColorByteCount++;
                    }

                    if (ColorByteCount == 4)
                    {
                        ColorByteCount = 1;
                        FileOutput.WriteByte(Convert.ToByte(colorBuffer));
                        colorBuffer = "";
                    }
                }
                FileInput.Close();
                FileOutput.Close();

                BeginInvoke(new Action(delegate ()
                {
                    DialogResult = DialogResult.Yes;
                    this.Close();
                })); 
            }
            catch (Exception e)
            {
                BeginInvoke(new Action(delegate ()
                {
                    DialogResult = DialogResult.No;
                    this.Close();
                }));
            }
        }

        private void FormConvertion_Shown(object sender, EventArgs e)
        {
            this.Refresh();
            Thread newThread = new Thread((ConvertCyclogramm));
            newThread.Start();
        }
    }
}
