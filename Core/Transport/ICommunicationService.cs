using System;
using RLCCore.Transport;

namespace RLCCore
{
    public interface ICommunicationService
    {
        event EventHandler<IRemoteClientMessage> OnReceiveMessage;
        void SendGlobalCommand(GlobalCommands command, ICommandContext commandContext);
        void SendClientCommand(ClientCommands command, IRemoteClient client, ICommandContext commandContext);
    }
}