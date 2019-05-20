using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.ClientConnectionService
{
    public interface IClientConnection
    {
        bool Connected { get; }
        IPAddress IPAddress { get; }
        event EventHandler OnChanged;
    }
}
