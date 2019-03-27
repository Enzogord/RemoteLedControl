using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLCCore.Transport
{
    public class RemoteClientMessage : IRemoteClientMessage
    {
        public int ProjectKey { get; set; }

        public int ClientNumber { get; set; }

        public ClientMessages MessageType { get; set; }

        public byte[] Data { get; set; }        
    }
}
