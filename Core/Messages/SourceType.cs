using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Messages
{
    public enum SourceType : byte
    {
        NotSet = 0,
        Server = 1,
        Client = 2
    }
}
