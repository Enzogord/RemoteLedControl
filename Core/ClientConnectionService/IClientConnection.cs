using System;
using System.Net;

namespace Core.ClientConnectionService
{
    public interface IClientConnection
    {
        bool Connected { get; }
        IPAddress IPAddress { get; }
        event EventHandler OnChanged;
    }
}
