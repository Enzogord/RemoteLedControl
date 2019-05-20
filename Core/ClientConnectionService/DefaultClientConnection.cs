using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.ClientConnectionService
{
    public class DefaultClientConnection : IClientConnection
    {
        public bool Connected => false;

        public IPAddress IPAddress => null;

        public event EventHandler OnChanged;
    }
}
