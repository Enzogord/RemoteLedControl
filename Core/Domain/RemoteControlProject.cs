using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Core.Messages;
using Core.RemoteOperations;
using NotifiedObjectsFramework;
using RLCCore.Exceptions;
using RLCCore.Serialization;
using Service;

namespace RLCCore.Domain
{
    [DataContract]
    public class RemoteControlProject : NotifyPropertyChangedBase, ISettingsProvider, IMessageReceiver
    {
        private const int defaultRlcPort = 11010;
        private const int defaultSntpPort = 11011;

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

        private int rlcPort = defaultRlcPort;
        [DataMember]
        [Display(Name = "RLC Port")]
        public int RlcPort {
            get => rlcPort;
            set => SetField(ref rlcPort, value, () => RlcPort);
        }

        private int sntpPort = defaultSntpPort;
        [DataMember]
        [Display(Name = "SNTP Port")]
        public int SntpPort {
            get => sntpPort;
            set => SetField(ref sntpPort, value, () => SntpPort);
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

        #region ISettingsProvider implementation

        public IDictionary<string, string> GetSettings()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("SSID", WifiSSID);
            result.Add("Password", WifiPassword);

            result.Add("ProjectKey", Key.ToString());
            result.Add("UDPPort", RlcPort.ToString());

            return result;
        }

        public void Receive(RLCMessage message)
        {
            RemoteClient foundClient = Clients.FirstOrDefault(x => x.Number == message.ClientNumber);
            if(foundClient == null) {
                return;
            }

            foundClient.ClientState = message.ClientState;
        }

        #endregion
    }

}
