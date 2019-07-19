using System;
using System.Net;

namespace Core.ClientConnectionService
{
    public class RemoteClientConnection : IClientConnection
    {
        private RemoteClientConnector connector;

        public bool Connected => connector != null ? connector.Connected : false;

        public IPAddress IPAddress => connector?.IPAddress;

        public event EventHandler OnChanged;

        public RemoteClientConnection(RemoteClientConnector connector)
        {
            this.connector = connector;
            connector.OnDisconnected += Connector_OnDisconnected;
        }

        private void Connector_OnDisconnected(object sender, EventArgs e)
        {
            if(connector == null) {
                return;
            }
            connector.OnDisconnected -= Connector_OnDisconnected;
            connector = null;
            OnChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
