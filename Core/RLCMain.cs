using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class RLCMain
    {
        public static string TempFolderPath => Path.Combine(Path.GetTempPath(), "RemoteLEDControl");
    }
}
