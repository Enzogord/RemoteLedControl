using System.Net;

namespace RLCCore
{
    public interface IServerAddressProvider
    {
        IPAddress GetServerIPAddress();
    }
}