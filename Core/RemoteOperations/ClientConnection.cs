using NotifiedObjectsFramework;
using System;
using System.Net;

namespace Core.RemoteOperations
{
    public class ClientConnection : NotifyPropertyChangedBase, IClientConnection
    {
        bool firstUpdate = true;

        private DateTime lastRefreshTime;
        public DateTime LastRefreshTime {
            get => lastRefreshTime;
            set => SetField(ref lastRefreshTime, value);
        }

        private DateTime firstConnectedTime;
        public DateTime FirstConnectedTime {
            get => firstConnectedTime;
            set => SetField(ref firstConnectedTime, value);
        }

        private DateTime lastConnectionRefreshTime;
        public DateTime LastConnectionRefreshTime {
            get => lastConnectionRefreshTime;
            set => SetField(ref lastConnectionRefreshTime, value);
        }

        private bool connected;
        public bool Connected {
            get => connected;
            set => SetField(ref connected, value);
        }

        private IPEndPoint endPoint;
        public IPEndPoint EndPoint {
            get => endPoint;
            set => SetField(ref endPoint, value);
        }

        public ClientConnection()
        {
        }

        public void RefreshState(bool connected, IPEndPoint endPoint)
        {
            EndPoint = endPoint;
            RefreshState(connected);
        }

        public void RefreshState(bool connected)
        {
            Connected = connected;
            var now = DateTime.Now;
            LastRefreshTime = now;
            if(Connected) {
                if(firstUpdate) {
                    FirstConnectedTime = now;
                    firstUpdate = false;
                }
                LastConnectionRefreshTime = now;
            }
        }
    }
}
