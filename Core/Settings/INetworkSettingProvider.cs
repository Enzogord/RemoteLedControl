using System.Net;

namespace RLCCore.Settings
{
    public interface INetworkSettingProvider
    {
        IPAddress BroadcastIPAddress { get; }
        int Port { get; }
        IPAddress GetServerIPAddress();
    }
}