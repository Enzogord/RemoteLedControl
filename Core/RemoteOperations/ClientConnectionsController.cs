using RLCCore.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Core.RemoteOperations
{
    public class ClientConnectionsController : IClientConnectionsController
    {
        private Dictionary<RemoteClient, ClientConnection> connections;

        public ClientConnectionsController(IEnumerable<RemoteClient> clients)
        {
            connections = new Dictionary<RemoteClient, ClientConnection>();
            foreach(var client in clients) {
                connections.Add(client, new ClientConnection());
            }
        }

        public IClientConnection GetClientConnection(RemoteClient client)
        {
            return GetConnection(client);
        }

        public IClientConnection GetClientConnection(int clientNumber)
        {
            return GetConnection(clientNumber);
        }

        public void UpdateClientActivity(int clientNumber, IPEndPoint endPoint)
        {
            var connection = GetConnection(clientNumber);
            if(connection == null) {
                return;
            }
            if(endPoint == null) {
                connection.RefreshState(true);
            }
            else {
                connection.RefreshState(true, endPoint);
            }
        }

        public bool ContainsClient(RemoteClient client)
        {
            return connections.ContainsKey(client);
        }

        public bool ContainsClient(int clientNumber)
        {
            return connections.Any(x => x.Key.Number == clientNumber);
        }

        private ClientConnection GetConnection(RemoteClient client)
        {
            if(!ContainsClient(client)) {
                return null;
            }
            return connections[client];
        }

        private ClientConnection GetConnection(int clientNumber)
        {
            var connection = connections.Where(x => x.Key.Number == clientNumber).Select(x => x.Value).FirstOrDefault();
            if(connection == null) {
                return null;
            }
            return connection;
        }        
    }
}
