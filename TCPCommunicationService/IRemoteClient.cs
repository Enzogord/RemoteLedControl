using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TCPCommunicationService
{
    public interface IRemoteClient
    {
        IPAddress IPAddress { get; }
        event RemoteClientBeforeAddressUpdated OnBeforeAddressUpdated;
    }

    public delegate void RemoteClientBeforeAddressUpdated(IRemoteClient client, IPAddress address);
}
