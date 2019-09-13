using System;
using Core.Messages;

namespace Core.ClientConnectionService
{
    public interface IRemoteClientCommunication
    {
        void Send(INumeredClient client, RLCMessage message);
        void SendToAll(RLCMessage message);
        event EventHandler<ClientMessageEventArgs> OnReceiveMessage;
        event EventHandler<INumeredClient> OnClientAuthorized;
    }
}
