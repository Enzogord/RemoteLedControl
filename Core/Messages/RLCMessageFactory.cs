using System;
using System.Net;
using System.Threading;

namespace Core.Messages
{
    public static class RLCMessageFactory
    {
        private static int messageId;
        private static int GetNextMessageId()
        {
            Interlocked.Increment(ref messageId);
            return messageId;
        }

        private static void SetMessageId(RLCMessage message)
        {
            message.MessageId = GetNextMessageId();
        }

        public static RLCMessage State(uint key, uint frame, DateTime frameStartTime, ClientState state)
        {
            var message = new RLCMessage(SourceType.Server, key, MessageType.State);
            message.Frame = frame;
            message.FrameStartTime = frameStartTime;
            message.ClientState = state;
            SetMessageId(message);
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
