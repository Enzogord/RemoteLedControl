using System;
using System.Net;
using SNTPService;
using TCPCommunicationService;
using UDPCommunication;

namespace Core.Messages
{
    public class RLCMessage : IUdpMessage, ITcpMessage
    {
        public const int MaxMessageLength = 200;

        private byte sourceTypeData;
        private byte[] keyData = new byte[4];
        private byte[] messageIdData = new byte[4];
        private byte messageTypeData;
        private byte[] clientNumberData = new byte[2];
        private byte clientStateData;
        private byte[] ipAddressData = new byte[4];
        private byte[] frameData = new byte[4];
        private byte[] frameStartTime = new byte[8];
        private byte[] batteryChargeData = new byte[2];

        //Текущая длина сообщения, при изменение полей пересчитать
        private int Length => 25;

        public byte[] ToArray()
        {
            byte[] result = new byte[MaxMessageLength];
            int i = 0;

            result[i++] = sourceTypeData;

            result[i++] = keyData[0];
            result[i++] = keyData[1];
            result[i++] = keyData[2];
            result[i++] = keyData[3];

            result[i++] = messageIdData[0];
            result[i++] = messageIdData[1];
            result[i++] = messageIdData[2];
            result[i++] = messageIdData[3];

            result[i++] = messageTypeData;

            result[i++] = clientNumberData[0];
            result[i++] = clientNumberData[1];

            result[i++] = clientStateData;

            result[i++] = ipAddressData[0];
            result[i++] = ipAddressData[1];
            result[i++] = ipAddressData[2];
            result[i++] = ipAddressData[3];

            result[i++] = frameData[0];
            result[i++] = frameData[1];
            result[i++] = frameData[2];
            result[i++] = frameData[3];

            result[i++] = frameStartTime[0];
            result[i++] = frameStartTime[1];
            result[i++] = frameStartTime[2];
            result[i++] = frameStartTime[3];
            result[i++] = frameStartTime[4];
            result[i++] = frameStartTime[5];
            result[i++] = frameStartTime[6];
            result[i++] = frameStartTime[7];

            result[i++] = batteryChargeData[0];
            result[i++] = batteryChargeData[1];

            return result;
        }

        public void FromBytes(byte[] bytes)
        {
            if(!(bytes.Length >= Length && bytes.Length <= MaxMessageLength)) {
                throw new ArgumentException("Длина пакета должна быть не меньше минимально возможно длины сообщения, и не больше максимальной длины сообщения");
            }

            if(!Enum.IsDefined(typeof(SourceType), bytes[0])) {
                throw new ArgumentException($"Невозможно определить параметр {nameof(SourceType)}");
            }

            if(!Enum.IsDefined(typeof(MessageType), bytes[9])) {
                throw new ArgumentException($"Невозможно определить параметр {nameof(MessageType)}");
            }

            if(!Enum.IsDefined(typeof(ClientState), bytes[12])) {
                throw new ArgumentException($"Невозможно определить параметр {nameof(ClientState)}");
            }
            int i = 0;
            sourceTypeData = bytes[i++];

            Array.Copy(bytes, i, keyData, 0, 4);
            i += 4;

            Array.Copy(bytes, i, messageIdData, 0, 4);
            i += 4;

            messageTypeData = bytes[i++];

            Array.Copy(bytes, i, clientNumberData, 0, 2);
            i += 2;

            clientStateData = bytes[i++];

            Array.Copy(bytes, i, ipAddressData, 0, 4);
            i += 4;

            Array.Copy(bytes, i, frameData, 0, 4);
            i += 4;

            Array.Copy(bytes, i, frameStartTime, 0, 8);
            i += 8;

            Array.Copy(bytes, i, batteryChargeData, 0, 2);
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

        public virtual SourceType SourceType {
            get { return (SourceType)sourceTypeData; }
            set { sourceTypeData = (byte)value; }
        }

        public virtual uint Key {
            get { return BytesToUInt(keyData); }
            set { keyData = UIntToBytes(value); }
        }

        public virtual int MessageId {
            get { return BytesToInt(messageIdData); }
            set { messageIdData = IntToBytes(value); }
        }

        public virtual MessageType MessageType {
            get { return (MessageType)messageTypeData; }
            set { messageTypeData = (byte)value; }
        }

        public virtual ushort ClientNumber {
            get { return BytesToUShort(clientNumberData); }
            set { clientNumberData = UShortToBytes(value); }
        }

        public virtual ClientState ClientState {
            get { return (ClientState)clientStateData; }
            set { clientStateData = (byte)value; }
        }

        public virtual IPAddress IPAddress {
            get { return BytesToIPAddress(ipAddressData); }
            set { ipAddressData = IPAddressToBytes(value); }
        }

        public virtual uint Frame {
            get { return BytesToUInt(frameData); }
            set { frameData = UIntToBytes(value); }
        }

        public virtual DateTime FrameStartTime {
            get { return DateTimeSerializator.TimestampToDateTime(frameStartTime); }
            set { DateTimeSerializator.DateTimeToTimestamp(value, frameStartTime); }
        }

        public virtual ushort BatteryCharge {
            get { return BytesToUShort(batteryChargeData); }
            set { batteryChargeData = UShortToBytes(value); }
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

        private int BytesToInt(byte[] bytes)
        {
            return (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + (bytes[3] << 0);
        }

        private byte[] IntToBytes(int number)
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
