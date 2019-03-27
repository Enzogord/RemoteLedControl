using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transport
{
    /*
    public abstract class RLCMessage
    {
        private byte sourceTypeData;
        private byte[] projectKeyData = new byte[4];
        private byte messageTypeData;
        private byte plateNumberData;
        private byte plateInfoData;
        private byte[] ipAddressData = new byte[4];
        private byte[] timeFrameData = new byte[4];

        public static RLCMessage SendServerIP( )
        {
            RLCMessage result = new SendServerIPMessage();
            result.sourceType = (byte)SourceType.Server;
            result.projectKey = (byte)SourceType.Server;
        }

        public RLCMessage()
        {
        }

        private SourceType sourceType;
        public SourceType SourceType {
            get { return sourceType; }
            set {
                sourceType = value;
                sourceTypeData = (byte)sourceType;
            }
        }

        private uint projectKey;
        public uint ProjectKey {
            get { return projectKey; }
            private set {
                projectKey = value;
                projectKeyData[0] = (byte)(projectKey >> 24);
                projectKeyData[1] = (byte)(projectKey >> 16);
                projectKeyData[2] = (byte)(projectKey >> 8);
                projectKeyData[3] = (byte)(projectKey >> 0);
            }
        }

    }

    public class SendServerIPMessage : RLCMessage
    {
    }

    public enum SourceType : byte
    {
        Server = 0,
        Client = 1
    }*/
}
