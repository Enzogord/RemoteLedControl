using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Основной класс осуществляющий всю работу сервера, клиентов, вывода информации
    /// </summary>
    [DataContract]
    public class Project
    {
        // Fields
        public System.Windows.Forms.Timer timer;
        private ProjectServerMode FMode;
        [DataMember]
        public UDPServer Server;
        [DataMember]
        private uint FKey;
        [DataMember]
        private string FAbsoluteFilePath;
        private bool FSaved;
        [DataMember]
        private List<Client> FClientList;
        private List<Client> FDeletedClientList;
        /// <summary>
        /// Хранит список открытых потоков для конвертирования циклограмм
        /// </summary>
        private List<Thread> FRuningThreadsList = new List<Thread>();
        [DataMember]
        private FileInfo FBindedAudioFile;

        public List<Client> DeletedClientList
        {
            get
            {
                return FDeletedClientList;
            }
            set
            {
                FDeletedClientList = value;
            }
        }
        public string Name { get; set; }
        public uint Key
        {
            get { return FKey; }
        }
        public string AbsoluteFilePath
        {
            get
            {
                return FAbsoluteFilePath;
            }
            set
            {
                FAbsoluteFilePath = value;
                AbsoluteFolderPath = Path.GetDirectoryName(FAbsoluteFilePath);
            }
        }
        [DataMember]
        public string AbsoluteFolderPath { get; set; }
        [DataMember]
        public string TEMPFolderName { get; set; }
        [DataMember]
        public string ClientsFolderName { get; set; }
        public bool Saved
        {
            get { return FSaved; }
            set
            {
                if (FSaved != value)
                {
                    FSaved = value;
                    OnSave?.Invoke();
                }
            }
        }
        [DataMember]
        public string ClientsConfigFileName { get; set; }
        public List<Client> ClientList
        {
            get { return FClientList; }
        }
        /// <summary>
        /// Свойство для получения списка открытых потоков для конвертирования циклограмм
        /// </summary>
        public List<Thread> RuningThreadsList
        {
            get { return FRuningThreadsList; }
            set { FRuningThreadsList = value; }
        }

        public FileInfo BindedAudioFile
        {
            get { return FBindedAudioFile; }
            set
            {
                if (value != FBindedAudioFile)
                {
                    Saved = false;
                    FBindedAudioFile = value;
                }
            }
        }

        // Events
        public delegate void DChangeClientList();
        public event DChangeClientList OnChangeClientList;
        public delegate void DHaveActiveThreads(bool HaveTreads);
        public event DHaveActiveThreads OnActiveThreadsChange;
        public delegate void DSave();
        public event DSave OnSave;

        // Methods
        public Project(uint ProjectKey)
        {
            FClientList = new List<Client>();
            ClientsFolderName = "Clients";
            ClientsConfigFileName = "set.txt";
            TEMPFolderName = "RemoteLEDControl";
            FKey = ProjectKey;
            Server = new UDPServer(FKey);
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
            FRuningThreadsList.Add(thread);
            if (FRuningThreadsList.Count > 0)
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
            FRuningThreadsList.RemoveAll(x => x.ManagedThreadId == ThreadID);
            if (FRuningThreadsList.Count > 0)
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
                    TmpClient.OnlineStatus = false;
                    TmpClient.OnChange += Client_OnChange;
                    TmpClient.OnChangeStatus += TmpClient_OnChangeStatus;
                    FClientList.Add(TmpClient);
                    Result = FClientList.IndexOf(TmpClient);
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
            if (FDeletedClientList == null)
            {
                FDeletedClientList = new List<Client>();
            }
            FDeletedClientList.Add(client);
            OnChangeClientList?.Invoke();
            Saved = false;
        }
    }

}
