using System;
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
