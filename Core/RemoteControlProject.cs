using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using RLCCore.Exceptions;
using Service;

namespace RLCCore
{
    [DataContract]
    public class RemoteControlProject : NotifyPropertyBase, IRemoteControlProject, ISettingsProvider
    {
        private uint key;
        [DataMember]
        [Display(Name = "Ключ")]
        public uint Key {
            get => key;
            set => SetField(ref key, value, () => Key);
        }

        private string wifiSSID;
        [DataMember]
        [Display(Name = "Wifi SSID")]
        public string WifiSSID {
            get => wifiSSID;
            set => SetField(ref wifiSSID, value, () => WifiSSID);
        }

        private string wifiPassword;
        [DataMember]
        [Display(Name = "Wifi password")]
        public string WifiPassword {
            get => wifiPassword;
            set => SetField(ref wifiPassword, value, () => WifiPassword);
        }

        private int port;
        [DataMember]
        [Display(Name = "Port")]
        public int Port {
            get => port;
            set => SetField(ref port, value, () => Port);
        }


        private string clientsConfigFileName;
        [DataMember]
        [Display(Name = "Имя файла конфигурации")]
        public string ClientsConfigFileName {
            get => clientsConfigFileName;
            set => SetField(ref clientsConfigFileName, value, () => ClientsConfigFileName);
        }

        private ObservableCollection<RemoteClient> clients;
        [DataMember]
        [Display(Name = "Клиенты")]
        public ObservableCollection<RemoteClient> Clients {
            get => clients;
            set => SetField(ref clients, value, () => Clients);
        }

        private byte[] audioFile;
        [DataMember]
        [Display(Name = "Аудиофайл")]
        public byte[] AudioFile {
            get => audioFile;
            set => SetField(ref audioFile, value, () => AudioFile);
        }

        [Obsolete("Для поддержки на момент перехода к новой версии")]
        public System.IO.FileInfo AudioFilePath { get; set; }

        public RemoteControlProject(uint ProjectKey)
        {
            Clients = new ObservableCollection<RemoteClient>();
            ClientsConfigFileName = "set.txt";
            Key = ProjectKey;
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

        public void UpdateClientIPAddress(int clientNumber, IPAddress ipAddress)
        {
            var client = Clients.FirstOrDefault(x => x.Number == clientNumber);
            if(client != null) {
                client.IPAddress = ipAddress;
            }
        }

        #region ISettingsProvider implementation

        public IDictionary<string, string> GetSettings()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("SSID", WifiSSID);
            result.Add("Password", WifiPassword);

            result.Add("ProjectKey", Key.ToString());
            result.Add("UDPPort", Port.ToString());

            return result;
        }

        #endregion
    }

}
