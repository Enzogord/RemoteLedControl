using Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace RLCCore
{
    [DataContract]
    public class RemoteClient : NotifyPropertyBase, ISettingsProvider, IRemoteClient
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

        private bool isOnline;
        [DataMember]
        public bool IsOnline {
            get => isOnline;
            private set {
                SetField(ref isOnline, value, () => IsOnline);
                if(isOnline) {
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

        #region Calculated properties

        public int LEDCount => Pins.Sum(x => x.LEDCount);

        public string StatusString => isOnline ? "Онлайн" : "Оффлайн";

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
            IsOnline = false;
            LastUpdateTime = DateTime.Now;
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
            IsOnline = isOnline;
        }
    }

}
