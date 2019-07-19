using System;
using System.Net;

namespace Core.ClientConnectionService
{
    public class DefaultClientConnection : IClientConnection
    {
        public bool Connected => false;

        public IPAddress IPAddress => null;

        public event EventHandler OnChanged;
    }
}
