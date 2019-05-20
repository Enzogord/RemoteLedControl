using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Messages;

namespace Core.ClientConnectionService
{
    public interface IConnectorMessageService
    {
        IEnumerable<IConnectableClient> Clients { get; }
        RLCMessage CreateRequestClientInfoMessage();
        bool TryParseClient(byte[] data, out INumeredClient clientNumber);
    }
}
