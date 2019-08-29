using System;
using System.Net;
using SNTPService;
using TCPCommunicationService;
using UDPCommunication;

namespace Core.Messages
{
    public class RLCMessage : IUdpMessage, ITcpMessage
    {
        //byte 0
        private byte sourceTypeData;
        //byte 1-4
        private byte[] keyData = new byte[4];
        //byte 5
        private byte messageTypeData;
        //byte 6-7
        private byte[] clientNumberData = new byte[2];
        //byte 8
        private byte clientStateData;
        //byte 9-12
        private byte[] ipAddressData = new byte[4];
        //byte 13-20
        private byte[] playFromTimeData = new byte[8];
        //byte 21-28
        private byte[] sendTimeData = new byte[8];

        //Текущая длина сообщения, при изменение полей пересчитать
        private int Length => 21;

        public byte[] ToArray()
        {
            byte[] result = new byte[200];
            result[0] = sourceTypeData;

            result[1] = keyData[0];
            result[2] = keyData[1];
            result[3] = keyData[2];
            result[4] = keyData[3];

            result[5] = messageTypeData;

            result[6] = clientNumberData[0];
            result[7] = clientNumberData[1];

            result[8] = clientStateData;

            result[9] = ipAddressData[0];
            result[10] = ipAddressData[1];
            result[11] = ipAddressData[2];
            result[12] = ipAddressData[3];

            result[13] = playFromTimeData[0];
            result[14] = playFromTimeData[1];
            result[15] = playFromTimeData[2];
            result[16] = playFromTimeData[3];
            result[17] = playFromTimeData[4];
            result[18] = playFromTimeData[5];
            result[19] = playFromTimeData[6];
            result[20] = playFromTimeData[7];

            result[21] = sendTimeData[0];
            result[22] = sendTimeData[1];
            result[23] = sendTimeData[2];
            result[24] = sendTimeData[3];
            result[25] = sendTimeData[4];
            result[26] = sendTimeData[5];
            result[27] = sendTimeData[6];
            result[28] = sendTimeData[7];

            return result;
        }

        public void FromBytes(byte[] bytes)
        {
            if(!(bytes.Length >= Length && bytes.Length <= 200)) {
                throw new ArgumentException("Длина пакета должна быть не меньше минимально возможно длины сообщения, и не больше максимальной длины сообщения");
            }

            if(!Enum.IsDefined(typeof(SourceType), bytes[0])) {
                throw new ArgumentException($"Невозможно определить параметр {nameof(SourceType)}");
            }

            if(!Enum.IsDefined(typeof(MessageType), bytes[5])) {
                throw new ArgumentException($"Невозможно определить параметр {nameof(MessageType)}");
            }

            if(!Enum.IsDefined(typeof(ClientState), bytes[8])) {
                throw new ArgumentException($"Невозможно определить параметр {nameof(ClientState)}");
            }

            sourceTypeData = bytes[0];
            Array.Copy(bytes, 1, keyData, 0, 4);
            messageTypeData = bytes[5];
            clientNumberData[0] = bytes[6];
            clientNumberData[1] = bytes[7];
            clientStateData = bytes[8];
            Array.Copy(bytes, 9, ipAddressData, 0, 4);
            Array.Copy(bytes, 13, playFromTimeData, 0, 8);
            Array.Copy(bytes, 21, sendTimeData, 0, 8);
        }

        public RLCMessage()
        {
        }

        public RLCMessage(SourceType sourceType, uint key, MessageType messageType)
        {
            SourceType = sourceType;
            Key = key;
            MessageType = messageType;
        }

        public SourceType SourceType {
            get { return (SourceType)sourceTypeData; }
            set { sourceTypeData = (byte)value; }
        }

        public uint Key {
            get { return BytesToUInt(keyData); }
            set { keyData = UIntToBytes(value); }
        }

        public MessageType MessageType {
            get { return (MessageType)messageTypeData; }
            set { messageTypeData = (byte)value; }
        }

        public ushort ClientNumber {
            get { return BytesToUShort(clientNumberData); }
            set { clientNumberData = UShortToBytes(value); }
        }

        public ClientState ClientState {
            get { return (ClientState)clientStateData; }
            set { clientStateData = (byte)value; }
        }

        public IPAddress IPAddress {
            get { return BytesToIPAddress(ipAddressData); }
            set { ipAddressData = IPAddressToBytes(value); }
        }

        public TimeSpan PlayFromTime {
            get { return DateTimeSerializator.DeserializeTimeSpanFromNtpTime(playFromTimeData); }
            set { DateTimeSerializator.SerializeTimeSpanToNtpTime(value, playFromTimeData); }
        }

        public DateTime SendTime {
            get { return DateTimeSerializator.TimestampToDateTime(sendTimeData); }
            set { DateTimeSerializator.DateTimeToTimestamp(value, sendTimeData); }
        }

        #region methods

        private uint BytesToUInt(byte[] bytes)
        {
            return ((uint)(bytes[0] << 24) + (uint)(bytes[1] << 16) + (uint)(bytes[2] << 8) + (uint)(bytes[3] << 0));
        }

        private byte[] UIntToBytes(uint number)
        {
            byte[] result = new byte[4];
            result[0] = (byte)(number >> 24);
            result[1] = (byte)(number >> 16);
            result[2] = (byte)(number >> 8);
            result[3] = (byte)(number >> 0);
            return result;
        }

        private ushort BytesToUShort(byte[] bytes)
        {
            return (ushort)((bytes[0] << 8) + (bytes[1] << 0));
        }

        private byte[] UShortToBytes(ushort number)
        {
            byte[] result = new byte[2];
            result[0] = (byte)(number >> 8);
            result[1] = (byte)(number >> 0);
            return result;
        }

        private IPAddress BytesToIPAddress(byte[] bytes)
        {
            return new IPAddress(bytes);
        }

        private byte[] IPAddressToBytes(IPAddress ipAddress)
        {
            return ipAddress.GetAddressBytes();
        }

        #endregion

    }    
}
