using System;
using System.Net;

namespace NetworkServer
{
    public class ReceivedDataEventArgs : EventArgs
    {
        public byte[] Data { get; }
        public int Size { get; }
        public EndPoint RemoteEndPoint { get; }

        public ReceivedDataEventArgs(byte[] data, int size, EndPoint remoteEndPoint)
        {
            Data = data;
            Size = size;
            RemoteEndPoint = remoteEndPoint ?? throw new ArgumentNullException(nameof(remoteEndPoint));
        }
    }
}
