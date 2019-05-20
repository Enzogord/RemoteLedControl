using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPCommunicationService
{
    public class DataEventArgs : EventArgs
    {
        public DataEventArgs(byte[] data)
        {
            Data = data;
        }

        public byte[] Data { get; }
    }
}
