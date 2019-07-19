using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Service;

namespace RLCCore.Settings
{
    public class NetworkController : NotifyPropertyBase, INetworkSettingProvider
    {
        private bool editable;
        public bool Editable {
            get => editable;
            set => SetField(ref editable, value, () => Editable);
        }

        private NetworkAddressSetting currentAddressSetting;
        public NetworkAddressSetting CurrentAddressSetting {
            get => currentAddressSetting;
            set {
                if(!Editable) {
                    return;
                }
                SetField(ref currentAddressSetting, value, () => CurrentAddressSetting);
            }
        }

        private int port;
        public int Port {
            get => port;
            set => SetField(ref port, value, () => Port);
        }

        public IPAddress BroadcastIPAddress => IPAddress.Broadcast;

        public ObservableCollection<NetworkAddressSetting> AddressSettings { get; set; }

        public NetworkController()
        {
            Editable = true;
            AddressSettings = new ObservableCollection<NetworkAddressSetting>();
            UpdateIPAddresses();
        }

        public void UpdateIPAddresses()
        {
            if(!Editable) {
                return;
            }

            var ipPropertiesList = NetworkInterface.GetAllNetworkInterfaces().Select(x => x.GetIPProperties());
            foreach(var ipProperties in ipPropertiesList.Select(x => x.UnicastAddresses)) {
                foreach(var ipInfomation in ipProperties) {
                    if(ipInfomation.Address == null || ipInfomation.Address.AddressFamily != AddressFamily.InterNetwork) {
                        continue;
                    }
                    NetworkAddressSetting setting = new NetworkAddressSetting(ipInfomation.Address, ipInfomation.IPv4Mask);
                    AddressSettings.Add(setting);
                }
            }

            if(CurrentAddressSetting == null) {
                CurrentAddressSetting = AddressSettings.FirstOrDefault();
                return;
            }

            var foundAddress = AddressSettings.FirstOrDefault(x => x.IPAddress.Equals(CurrentAddressSetting.IPAddress));
            if(foundAddress == null) {
                CurrentAddressSetting = AddressSettings.FirstOrDefault();
            } else {
                CurrentAddressSetting = foundAddress;
            }
        }

        public IPAddress GetServerIPAddress()
        {
            if(CurrentAddressSetting == null) {
                throw new ApplicationException("Нет доступного IP адреса");
            }
            return CurrentAddressSetting.IPAddress;
        }
    }
}
