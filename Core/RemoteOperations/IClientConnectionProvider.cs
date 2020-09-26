using System;

namespace Core.RemoteOperations
{
    public interface IClientConnectionProvider
    {
        event EventHandler ConnectionsUpdated;
        bool TryGetClientConnection(int clientId, out IClientConnection connection);
    }
}
