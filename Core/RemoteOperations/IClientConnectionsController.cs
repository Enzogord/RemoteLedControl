using RLCCore.Domain;
using System.Collections.Generic;
using System.Net;

namespace Core.RemoteOperations
{
    public interface IClientConnectionsController
    {
        void CreateConnections(IEnumerable<RemoteClient> clients);
        void ClearConnections();
        bool ContainsClient(RemoteClient client);
        bool ContainsClient(int clientNumber);
        IClientConnection GetClientConnection(RemoteClient client);
        IClientConnection GetClientConnection(int clientNumber);
        void UpdateClientActivity(int clientNumber, IPEndPoint endPoint);
    }
}
