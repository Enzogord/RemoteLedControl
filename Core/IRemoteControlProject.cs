using System.Collections.ObjectModel;
using RLCCore.RemoteOperations;

namespace RLCCore
{
    public interface IRemoteControlProject : IControlUnit
    {
        byte[] AudioFile { get; set; }
        string ClientsConfigFileName { get; set; }
        string WifiSSID { get; set; }
        string WifiPassword { get; set; }

        void AddClient(RemoteClient client);
        void DeleteClient(RemoteClient client);
    }
}