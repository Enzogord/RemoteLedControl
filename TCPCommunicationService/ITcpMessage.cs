using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPCommunicationService
{
    public interface ITcpMessage
    {
        byte[] ToArray();
        void FromBytes(byte[] bytes);
    }
}
