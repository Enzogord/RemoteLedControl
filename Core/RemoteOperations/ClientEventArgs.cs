using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLCCore;

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
