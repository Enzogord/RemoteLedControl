using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core
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
