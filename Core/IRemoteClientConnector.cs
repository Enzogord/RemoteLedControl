using System;
using System.Net;
using Core.ClientConnectionService;
using Core.Messages;
using Core.RemoteOperations;
using TCPCommunicationService;

namespace Core
{
    public interface IRemoteClientConnectionService
    {
        bool IsActive { get; }
        void Send(INumeredClient client, RLCMessage message);
        void SendToAll(RLCMessage message);
        event EventHandler<ClientMessageEventArgs> OnReceiveMessage;
        void Start();
        void Stop();
    }
}
