using System.Net;

namespace RLCCore
{
    public interface INetworkSettingProvider
    {
        IPAddress BroadcastIPAddress { get; }
        int Port { get; }
        IPAddress GetServerIPAddress();
    }
}