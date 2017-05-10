using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core;

namespace RemoteLEDServer
{
    public partial class FormCreateProject : Form
    {
        public uint ProjectKey;

        public FormCreateProject()
        {
            InitializeComponent();
        }

        private void button_GenerateKey_Click(object sender, EventArgs e)
        {
            byte[] bytes = new byte[4];

            Random Rand = new Random();
            Rand.NextBytes(bytes);

            uint Key = (uint)((bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + (bytes[3] << 0));

            textBox_ProjectKey.Text = Key.ToString();            
        }

        private void textBox_ProjectKey_TextChanged(object sender, EventArgs e)
        {   
            bool b = ServiceFunc.TextBoxValidation((sender as TextBox), TypesNumeric.t_uint32);
            ServiceFunc.ColoringTextBox((sender as TextBox), b);
            (sender as TextBox).Select((sender as TextBox).TextLength, 0);
            button_Ok.Enabled = b;
        }

        private void button_Ok_Click(object sender, EventArgs e)
        {
            ProjectKey = UInt32.Parse(textBox_ProjectKey.Text);
        }
    }
}
