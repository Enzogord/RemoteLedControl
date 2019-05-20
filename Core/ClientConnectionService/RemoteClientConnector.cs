using System;
using System.Net;
using TCPCommunicationService;

namespace Core.ClientConnectionService
{
    public class RemoteClientConnector
    {
        public TcpClientListener ClientListener { get; private set; }

        public bool Connected => ClientListener.IsConnected;

        public IPAddress IPAddress => ClientListener?.IPEndPoint?.Address;

        public event EventHandler OnDisconnected;

        public RemoteClientConnector(TcpClientListener clientListener)
        {
            this.ClientListener = clientListener ?? throw new ArgumentNullException(nameof(clientListener));
            clientListener.OnDisconnected += (sender, e) =>
            {
                OnDisconnected?.Invoke(clientListener, EventArgs.Empty);
            };
        }
    }
}
