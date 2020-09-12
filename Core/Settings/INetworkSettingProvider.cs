using System.Net;

namespace RLCCore.Settings
{
    public interface INetworkSettingProvider
    {
        IPAddress BroadcastIPAddress { get; }
        IPAddress GetServerIPAddress();
    }
}