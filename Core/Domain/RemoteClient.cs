using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using Core.ClientConnectionService;
using Core.Messages;
using NotifiedObjectsFramework;
using RLCCore.Serialization;

namespace RLCCore.Domain
{
    [DataContract]
    public class RemoteClient : NotifyPropertyChangedBase, ISettingsProvider, INumeredClient, IConnectableClient
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

        private bool isDigitalPWMSignal;
        [DataMember]
        public bool IsDigitalPWMSignal {
            get => isDigitalPWMSignal;
            set => SetField(ref isDigitalPWMSignal, value, () => IsDigitalPWMSignal);
        }

        private bool isInvertedPWMSignal;
        [DataMember]
        public bool IsInvertedPWMSignal {
            get => isInvertedPWMSignal;
            set => SetField(ref isInvertedPWMSignal, value, () => IsInvertedPWMSignal);
        }

        private bool defaultLight;
        [DataMember]
        public bool DefaultLight {
            get => defaultLight;
            set => SetField(ref defaultLight, value, () => DefaultLight);
        }

        private byte spiLedGlobalBrightness;
        [DataMember]
        public byte SPILedGlobalBrightness {
            get => spiLedGlobalBrightness;
            set => SetField(ref spiLedGlobalBrightness, value, () => SPILedGlobalBrightness);
        }

        private ObservableCollection<Pin> pins;
        [DataMember]
        public ObservableCollection<Pin> Pins {
            get => pins;
            set => SetField(ref pins, value, () => Pins);
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
            set {
                var oldConnection = connection;
                if(SetField(ref connection, value, () => Connection)) {
                    if(oldConnection != null) {
                        oldConnection.OnChanged -= OldConnection_OnChanged;
                    }
                    if(connection != null) {
                        connection.OnChanged += OldConnection_OnChanged;
                    }
                    UpdateConnectionsInfo();
                }
            }
        }

        public bool Connected => Connection.Connected;

        public IPAddress IPAddress => Connection.IPAddress;

        private void OldConnection_OnChanged(object sender, EventArgs e)
        {
            UpdateConnectionsInfo();
        }

        private void UpdateConnectionsInfo()
        {
            OnPropertyChanged(() => Connected);
            OnPropertyChanged(() => IPAddress);
        }

        #region Calculated properties

        public int LEDCount => Pins.Sum(x => x.LEDCount);

        #endregion

        public RemoteClient(string name, int number)
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
            Connection = new DefaultClientConnection();
        }

        #region ISettingsProvider implementation

        public IDictionary<string, string> GetSettings()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("PlateNumber", $"{Number}");
            result.Add("IsDigitalPWMSignal", IsDigitalPWMSignal ? "1" : "0");
            result.Add("InvertedPWMSignal", IsInvertedPWMSignal ? "1" : "0");
            result.Add("DefaultLightMode", DefaultLight ? "On" : "Off");
            result.Add("SPILedGlobalBrightness", $"{SPILedGlobalBrightness}");
            result.Add("Pins", "P5-1,P4-1,P0-1,S2-1");

            /*string pinsValue = "";
            foreach(var pin in Pins) {
                if(!string.IsNullOrWhiteSpace(pinsValue)) {
                    pinsValue += ',';
                }
                pinsValue += $"{pin.PinNumber}-{pin.LEDCount}";
            }
            result.Add("Pins", pinsValue);*/

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
