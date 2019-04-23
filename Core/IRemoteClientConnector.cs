using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Messages;
using TCPCommunicationService;

namespace Core
{
    public interface IRemoteClientConnector
    {
        bool IsActive { get; }
        void Send(IPAddress address, RLCMessage message);
        void SendToAll(RLCMessage message);
        event ReceiveMessageEventHandler<RLCMessage> OnReceiveMessage;
        void Start();
        void Stop();
    }
}
