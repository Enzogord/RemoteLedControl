using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteLEDServer
{
    static class Program
    {
        public static RemoteLEDControl RemoteLEDControlMainForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RemoteLEDControlMainForm = new RemoteLEDControl();
            Application.Run(RemoteLEDControlMainForm);
            int a = 222;
        }
    }
}
