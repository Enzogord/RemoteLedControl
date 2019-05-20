using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPCommunicationService;

namespace Core.ClientConnectionService
{
    public class ClientIdentifiedEventArgs : EventArgs
    {
        public INumeredClient Client { get; }
        public TcpClientListener Listener { get; }

        public ClientIdentifiedEventArgs(INumeredClient client, TcpClientListener listener)
        {
            Client = client;
            Listener = listener;
        }
    }
}
