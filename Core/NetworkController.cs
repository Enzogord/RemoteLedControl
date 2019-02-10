using Service;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Core
{
    public class NetworkController : NotifyPropertyBase
    {
        private bool isLocked;
        public bool IsLocked {
            get => isLocked;
            set => SetField(ref isLocked, value, () => IsLocked);
        }

        private NetworkAddressSetting currentAddressSetting;
        public NetworkAddressSetting CurrentAddressSetting {
            get => currentAddressSetting;
            set {
                if(IsLocked) {
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
            AddressSettings = new ObservableCollection<NetworkAddressSetting>();
            UpdateIPAddresses();
        }

        public void UpdateIPAddresses()
        {
            if(IsLocked) {
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
        
    }
}
