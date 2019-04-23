using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Messages;
using TCPCommunicationService;

namespace Core
{
    public class RemoteClientConnector : TCPService<RLCMessage>, IRemoteClientConnector
    {
        public RemoteClientConnector(IPAddress address, int port, IEnumerable<IRemoteClient> expectedClients, int bufferSize, byte workersCount = 1) : this(new IPEndPoint(address, port), expectedClients, bufferSize, workersCount)
        {

        }

        public RemoteClientConnector(IPEndPoint localIpEndPoint, IEnumerable<IRemoteClient> expectedClients, int bufferSize, byte workersCount = 1) : base(localIpEndPoint, expectedClients, bufferSize, workersCount)
        {

        }
    }
}
