using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Messages
{
    public enum ClientState : byte
    {
        NotSet = 0,
        Playing = 1,
        Stoped = 2,
        Paused = 3
    }
}
