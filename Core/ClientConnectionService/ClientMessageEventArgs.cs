using System;
using Core.Messages;

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
