using RLCCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Timers;

namespace Core.RemoteOperations
{
    public class ClientConnectionsController : IClientConnectionProvider, IDisposable
    {
        private Dictionary<int, ClientConnection> connections;
        private Timer timer;

        public event EventHandler ConnectionsUpdated;

        public ClientConnectionsController()
        {
            timer = new Timer();
            timer.Elapsed += (s, e) => RefreshConnections();
            timer.Interval = 1000;
        }

        private void RefreshConnections()
        {
            foreach(var con in connections.Values) {
                con.Refresh();
            }
        }

        public void CreateConnections(IEnumerable<RemoteClient> clients)
        {
            if(clients is null) {
                throw new ArgumentNullException(nameof(clients));
            }

            connections = new Dictionary<int, ClientConnection>();
            foreach(var clientId in clients.Select(x => x.Number)) {
                connections.Add(clientId, new ClientConnection());
            }
            RaiseConnectionsUpdated();
            timer.Start();
        }

        public void ClearConnections()
        {
            timer.Stop();
            if(connections != null) {
                foreach(var c in connections.Values) {
                    c.Connected = false;
                }
                connections.Clear();
            }
            RaiseConnectionsUpdated();
        }

        private void RaiseConnectionsUpdated()
        {
            ConnectionsUpdated?.Invoke(this, EventArgs.Empty);
        }

        public bool ContainsClientConnection(int clientNumber)
        {
            return connections.ContainsKey(clientNumber);
        }

        public bool TryGetClientConnection(int clientNumber, out IClientConnection connection)
        {
            connection = null;
            if(!connections.TryGetValue(clientNumber, out ClientConnection result)) {
                return false;
            }
            connection = result;
            return true;
        }

        public void UpdateConnection(int clientNumber, IPEndPoint endPoint)
        {
            if(!connections.TryGetValue(clientNumber, out ClientConnection connection)) {
                return;
            }
            if(endPoint != null) {
                connection.EndPoint = endPoint;
            }
            connection.UpdateConnection();
        }        

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue) {
                if(disposing) {
                    timer.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
