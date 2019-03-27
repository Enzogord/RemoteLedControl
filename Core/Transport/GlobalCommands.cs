using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLCCore.Transport
{
    public enum GlobalCommands : byte
    {
        Play = 1,
        Stop = 2,
        Pause = 6,
        PlayFrom = 7,
        SendServerAddress = 5
    }
}
