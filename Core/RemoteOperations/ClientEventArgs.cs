using System;
using RLCCore.Domain;

namespace Core.RemoteOperations
{
    public class ClientEventArgs : EventArgs
    {
        public RemoteClient RemoteClient { get; }

        public ClientEventArgs(RemoteClient remoteClient)
        {
            RemoteClient = remoteClient;
        }
    }
}
