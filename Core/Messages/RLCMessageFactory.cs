using System;
using System.Net;

namespace Core.Messages
{
    public static class RLCMessageFactory
    {
        public static RLCMessage Play(uint key)
        {
            var message = new RLCMessage(SourceType.Server, key, MessageType.Play);
            return message;
        }

        public static RLCMessage Stop(uint key)
        {
            var message = new RLCMessage(SourceType.Server, key, MessageType.Stop);
            return message;
        }

        public static RLCMessage Pause(uint key)
        {
            var message = new RLCMessage(SourceType.Server, key, MessageType.Pause);
            return message;
        }

        public static RLCMessage PlayFrom(uint key, TimeSpan time)
        {
            var message = new RLCMessage(SourceType.Server, key, MessageType.PlayFrom);
            message.PlayFromTime = time;
            return message;
        }

        public static RLCMessage Rewind(uint key, TimeSpan time, ClientState state)
        {
            var message = new RLCMessage(SourceType.Server, key, MessageType.Rewind);
            message.PlayFromTime = time;
            message.ClientState = state;
            return message;
        }

        public static RLCMessage SendServerIP(uint key, IPAddress ipAddress)
        {
            var message = new RLCMessage(SourceType.Server, key, MessageType.SendServerIp);
            message.IPAddress = ipAddress;
            return message;
        }

        public static RLCMessage RequestClientInfo(uint key)
        {
            var message = new RLCMessage(SourceType.Server, key, MessageType.RequestClientInfo);
            return message;
        }
    }
}
