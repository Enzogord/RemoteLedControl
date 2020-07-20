using System;
using System.Net;

namespace UDPTransmission
{
    public class ReceivingDataEventArgs : EventArgs
    {
        public byte[] Data { get; private set; }
        public IPEndPoint Ip { get; private set; }

        public ReceivingDataEventArgs(byte[] data, IPEndPoint ip)
        {
            Data = data;
            Ip = ip;
        }
    }
}
