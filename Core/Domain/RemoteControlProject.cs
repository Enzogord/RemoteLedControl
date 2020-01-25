using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Core.Messages;
using Core.Model;
using Core.RemoteOperations;
using NotifiedObjectsFramework;
using RLCCore.Exceptions;
using RLCCore.Serialization;

namespace RLCCore.Domain
{
    [DataContract]
    public class RemoteControlProject : NotifyPropertyChangedBase, ISettingsProvider, IMessageReceiver, IUniqueClientProvider
    {
        private const int defaultRlcPort = 11010;
        private const int defaultSntpPort = 11011;

        private uint key;
        [DataMember]
        [Display(Name = "Ключ")]
        public uint Key {
            get => key;
            set => SetField(ref key, value);
        }

        private string wifiSSID;
        [DataMember]
        [Display(Name = "Wifi SSID")]
        public string WifiSSID {
            get => wifiSSID;
            set => SetField(ref wifiSSID, value);
        }

        private string wifiPassword;
        [DataMember]
        [Display(Name = "Wifi password")]
        public string WifiPassword {
            get => wifiPassword;
            set => SetField(ref wifiPassword, value);
        }

        private int rlcPort = defaultRlcPort;
        [DataMember]
        [Display(Name = "RLC Port")]
        public int RlcPort {
            get => rlcPort;
            set => SetField(ref rlcPort, value);
        }

        private int sntpPort = defaultSntpPort;
        [DataMember]
        [Display(Name = "SNTP Port")]
        public int SntpPort {
            get => sntpPort;
            set => SetField(ref sntpPort, value);
        }

        private string clientsConfigFileName;
        [DataMember]
        [Display(Name = "Имя файла конфигурации")]
        public string ClientsConfigFileName {
            get => clientsConfigFileName;
            set => SetField(ref clientsConfigFileName, value);
        }

        private ObservableCollection<RemoteClient> clients;
        [DataMember]
        [Display(Name = "Клиенты")]
        public ObservableCollection<RemoteClient> Clients {
            get => clients;
            set => SetField(ref clients, value);
        }

        private byte[] audioFile;
        [DataMember]
        [Display(Name = "Аудиофайл")]
        public byte[] AudioFile {
            get => audioFile;
            set => SetField(ref audioFile, value);
        }

        private string soundtrackFileName;
        [DataMember]
        public string SoundtrackFileName {
            get => soundtrackFileName;
            set => SetField(ref soundtrackFileName, value);
        }

        [Obsolete("Для поддержки на момент перехода к новой версии")]
        public System.IO.FileInfo AudioFilePath { get; set; }

        public RemoteControlProject(uint ProjectKey)
        {
            Clients = new ObservableCollection<RemoteClient>();
            ClientsConfigFileName = "set.txt";
            Key = ProjectKey;
        }

        public int GenerateClientNumber()
        {
            if(Clients.Any()) {
                return Clients.Max(x => x.Number) + 1;
            }
            return 1;
        }

        public bool ClientExists(int number)
        {            
            return Clients.Any(x => x.Number == number);
        }

        public void AddClient(RemoteClient client)
        {
            if(Clients.Any(x => x.Number == client.Number)) {
                throw ExceptionsHelper.GetUserNotificationNotImplementedException(new ArgumentException("Клиент с таким номером уже содержится в списке"));
            }
            Clients.Add(client);
        }

        public void DeleteClient(RemoteClient client)
        {
            if(Clients.Contains(client)) {
                Clients.Remove(client);
            }
        }

        public void Receive(RLCMessage message)
        {
            RemoteClient foundClient = Clients.FirstOrDefault(x => x.Number == message.ClientNumber);
            if(foundClient == null) {
                return;
            }

            foundClient.ClientState = message.ClientState;
            foundClient.SetBatteryChargeLevel(message.BatteryCharge);
        }

        #region ISettingsProvider implementation

        public IDictionary<string, string> GetSettings()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("SSID", WifiSSID);
            result.Add("Password", WifiPassword);

            result.Add("ProjectKey", Key.ToString());
            result.Add("UDPPort", RlcPort.ToString());
            result.Add("UDPPackageSize", "200");
            result.Add("RefreshInterval", "50");

            return result;
        }        

        #endregion
    }

}
