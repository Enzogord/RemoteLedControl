using System;
using System.Collections.ObjectModel;
using System.Net;

namespace RLCCore
{
    public interface IRemoteClient
    {
        Cyclogramm Cyclogramm { get; set; }
        IPAddress IPAddress { get; set; }
        bool IsOnline { get; }
        DateTime LastUpdateTime { get; }
        int LEDCount { get; }
        string Name { get; set; }
        int Number { get; set; }
        ObservableCollection<Pin> Pins { get; set; }
        string StatusString { get; }
        bool WaitPlayingStatus { get; set; }

        void AddPin(byte pinNumber, ushort ledCount);
        void DeletePin(Pin pin);
        void UpdateStatus(bool isOnline);
    }
}