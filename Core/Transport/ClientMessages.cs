using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLCCore.Transport
{
    public enum ClientMessages : byte
    {
        SendPlateNumber = 3,
        RequestServerIP = 8
    }
}
