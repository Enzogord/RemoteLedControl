using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ClientConnectionService
{
    public interface IConnectableClient : INumeredClient
    {
        IClientConnection Connection { get; set; }
    }
}
