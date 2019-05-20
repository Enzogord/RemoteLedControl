using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Messages;
using RLCCore;

namespace Core.ClientConnectionService
{
    public class ClientMessageEventArgs : EventArgs
    {
        public INumeredClient Client { get; }
        public RLCMessage Message { get; }

        public ClientMessageEventArgs(INumeredClient client, RLCMessage message)
        {
            Client = client;
            Message = message;
        }
    }
}
