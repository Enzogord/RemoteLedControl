using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPCommunicationService
{
    public class TCPClientHandler<TClient>
    {
        public TClient Client { get; }

        public TcpClientListener ClientListener { get; set; }

        public TCPClientHandler(TClient client)
        {
            Client = client;
        }
    }
}
