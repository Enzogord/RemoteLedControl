using System;
using System.ComponentModel;
using System.Net;

namespace Core.RemoteOperations
{
    public interface IClientConnection : INotifyPropertyChanged
    {
        DateTime LastRefreshTime { get; }

        DateTime FirstConnectedTime { get; }

        DateTime LastConnectionRefreshTime { get; }

        bool Connected { get; }

        IPEndPoint EndPoint { get; }
    }
}
