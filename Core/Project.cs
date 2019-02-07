using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;

namespace Core
{
    /// <summary>
    /// Основной класс осуществляющий всю работу сервера, клиентов, вывода информации
    /// </summary>
    [DataContract]
    public class Project
    {
        public System.Windows.Forms.Timer Timer { get; set; }

        public List<Client> DeletedClientList { get; set; } = new List<Client>();

        [DataMember]
        public uint Key { get; private set; }

        [DataMember]
        private string absoluteFilePath;
        public string AbsoluteFilePath {
            get => absoluteFilePath;
            set {
                absoluteFilePath = value;
                AbsoluteFolderPath = Path.GetDirectoryName(absoluteFilePath);
            }
        }

        [DataMember]
        public string AbsoluteFolderPath { get; set; }

        [DataMember]
        public string TEMPFolderName { get; set; }

        [DataMember]
        public string ClientsFolderName { get; set; }

        private bool saved;
        public bool Saved {
            get => saved;
            set {
                if (saved != value) {
                    saved = value;
                    OnSave?.Invoke();
                }
            }
        }

        [DataMember]
        public string ClientsConfigFileName { get; set; }

        [DataMember]
        public List<Client> ClientList { get; private set; }

        /// <summary>
        /// Свойство для получения списка открытых потоков для конвертирования циклограмм
        /// </summary>
        public List<Thread> RuningThreadsList { get; set; } = new List<Thread>();

        [DataMember]
        private FileInfo bindedAudioFile;
        public FileInfo BindedAudioFile {
            get => bindedAudioFile;
            set {
                if (value != bindedAudioFile) {
                    Saved = false;
                    bindedAudioFile = value;
                }
            }
        }

        public delegate void DChangeClientList();
        public event DChangeClientList OnChangeClientList;
        public delegate void DHaveActiveThreads(bool HaveTreads);
        public event DHaveActiveThreads OnActiveThreadsChange;
        public delegate void DSave();
        public event DSave OnSave;

        public Project(uint ProjectKey)
        {
            ClientList = new List<Client>();
            ClientsFolderName = "Clients";
            ClientsConfigFileName = "set.txt";
            TEMPFolderName = "RemoteLEDControl";
            Key = ProjectKey;
        }

        public string GetAbsoluteTEMPPath()
        {
            string result = Path.GetTempPath() + "\\" + TEMPFolderName;
            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }
            return result;
        }

        public void AddThread(Thread thread)
        {
            RuningThreadsList.Add(thread);
            if (RuningThreadsList.Count > 0)
            {
                OnActiveThreadsChange?.Invoke(true);
            }
            else
            {
                OnActiveThreadsChange?.Invoke(false);
            }
        }

        public void RemoveThread(int ThreadID)
        {
            int i = Thread.CurrentThread.ManagedThreadId;
            RuningThreadsList.RemoveAll(x => x.ManagedThreadId == ThreadID);
            if (RuningThreadsList.Count > 0)
            {
                OnActiveThreadsChange?.Invoke(true);
            }
            else
            {
                OnActiveThreadsChange?.Invoke(false);
            }
        }

        /// <summary>
        /// Добавляет новый клиент в список. Возвращает индекс добавленного клиента в списке, если не добавился возвращает -1
        /// </summary>
        public int AddClient(ClientValues Values)
        {
            int Result = -1;
            Client TmpClient = new Client(Values.Name, Values.Number);
            if (TmpClient != null)
            {
                if (TmpClient.Number > 0)
                {
                    TmpClient.WifiSSID = Values.WifiSSID;
                    TmpClient.WifiPassword = Values.WifiPassword;
                    TmpClient.UDPPort = Values.UDPPort;
                    TmpClient.LEDCount = Values.LEDCount;
                    TmpClient.RelativePath = Values.RelativeFolderPath;
                    TmpClient.Renamed = false;
                    TmpClient.Parent = this;
                    TmpClient.Saved = false;
                    TmpClient.Status = false;
                    TmpClient.OnChange += Client_OnChange;
                    TmpClient.OnChangeStatus += TmpClient_OnChangeStatus;
                    ClientList.Add(TmpClient);
                    Result = ClientList.IndexOf(TmpClient);
                    OnChangeClientList?.Invoke();
                }
            }
            return Result;
        }

        private void TmpClient_OnChangeStatus()
        {
            OnChangeClientList?.Invoke();
        }

        private void Client_OnChange()
        {
            OnChangeClientList?.Invoke();
        }

        public void DeleteClient(Client client)
        {
            ClientList.Remove(client);
            if (DeletedClientList == null) {
                DeletedClientList = new List<Client>();
            }
            DeletedClientList.Add(client);
            OnChangeClientList?.Invoke();
            Saved = false;
        }
    }

}
