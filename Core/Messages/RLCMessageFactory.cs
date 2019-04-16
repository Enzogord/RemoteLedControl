using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

        public static RLCMessage PlayFrom(uint key, uint frameTime)
        {
            var message = new RLCMessage(SourceType.Server, key, MessageType.PlayFrom);
            message.TimeFrame = frameTime;
            return message;
        }

        public static RLCMessage SendServerIP(uint key, IPAddress ipAddress)
        {
            var message = new RLCMessage(SourceType.Server, key, MessageType.SendServerIp);
            message.IPAddress = ipAddress;
            return message;
        }
    }
}
