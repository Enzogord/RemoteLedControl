using Core.ClientConnectionService;
using Core.Messages;
using Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using TCPCommunicationService;

namespace RLCCore
{
    [DataContract]
    public class RemoteClient : NotifyPropertyBase, ISettingsProvider, INumeredClient, IConnectableClient
    {
        private string name;
        [DataMember]
        public string Name {
            get => name;
            set => SetField(ref name, value, () => Name);
        }

        private int number;
        [DataMember]
        public int Number {
            get => number;
            set => SetField(ref number, value, () => Number);
        }

        private bool connected;
        [DataMember]
        public bool Connected {
            get => connected;
            set {
                SetField(ref connected, value, () => Connected);
                if(connected) {
                    LastUpdateTime = DateTime.Now;
                }
            }
        }

        private DateTime lastUpdateTime;
        public DateTime LastUpdateTime {
            get => lastUpdateTime;
            private set => SetField(ref lastUpdateTime, value, () => LastUpdateTime);
        }

        private ObservableCollection<Pin> pins;
        [DataMember]
        public ObservableCollection<Pin> Pins {
            get => pins;
            set => SetField(ref pins, value, () => Pins);
        }

        private IPAddress ipAddress;
        public IPAddress IPAddress {
            get => ipAddress;
            set => SetField(ref ipAddress, value, () => IPAddress);
        }

        public bool WaitPlayingStatus { get; set; }    

        private Cyclogramm cyclogramm;
        [DataMember]
        public Cyclogramm Cyclogramm {
            get => cyclogramm;
            set => SetField(ref cyclogramm, value, () => Cyclogramm);
        }

        private ClientState clientState;        
        public ClientState ClientState {
            get => clientState;
            set => SetField(ref clientState, value, () => ClientState);
        }

        private IClientConnection connection;
        public IClientConnection Connection {
            get => connection;
            set => SetField(ref connection, value, () => Connection);
        }

        #region Calculated properties

        public int LEDCount => Pins.Sum(x => x.LEDCount);

        public string StatusString => connected ? "Онлайн" : "Оффлайн";

        #endregion

        public RemoteClient(string name, byte number)
        {
            if(string.IsNullOrWhiteSpace(name)){
                throw new ArgumentException($"Аргумент {nameof(name)} не должен быть равен null или быть пустым");
            }

            if(number == 0) {
                throw new ArgumentException($"Аргумент {nameof(number)} не должен быть равен нулю");
            }

            Pins = new ObservableCollection<Pin>();
            Name = name;
            Number = number;
            Connected = false;
            LastUpdateTime = DateTime.Now;
            Connection = new DefaultClientConnection();
        }

        #region ISettingsProvider implementation

        public IDictionary<string, string> GetSettings()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("PlateNumber", Number.ToString());

            string pinsValue = "";
            foreach(var pin in Pins) {
                if(!string.IsNullOrWhiteSpace(pinsValue)) {
                    pinsValue += ',';
                }
                pinsValue += $"{pin.PinNumber}-{pin.LEDCount}";
            }
            result.Add("Pins", pinsValue);

            return result;
        }

        #endregion

        public void AddPin(byte pinNumber, ushort ledCount)
        {
            if(ledCount == 0){
                return;
            }

            var pin = Pins.FirstOrDefault(x => x.PinNumber == pinNumber);
            if(pin == null) {
                pin = new Pin(pinNumber, ledCount);
            } else {
                pin.LEDCount = ledCount;
            }
        }

        public void DeletePin(Pin pin)
        {
            if(pin == null){
                return;
            }
            if(Pins.Contains(pin)) {
                Pins.Remove(pin);
            }
        }   
        
        public void UpdateStatus(bool isOnline)
        {
            Connected = isOnline;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            RemoteClient comparedClient = obj as RemoteClient;
            if(comparedClient == null) {
                return false;
            }
            return Number == comparedClient.Number;
        }
    }

}
