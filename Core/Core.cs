using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Collections.ObjectModel;

namespace Core
{
    public enum ProjectServerMode
    {
        Test = 1,
        Release = 2
    }

    public enum ClientState
    {
        Wait = 1,
        Play = 2,
        Pause = 3
    }

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

        // Properties        

        public ProjectServerMode Mode
        {
            get { return FMode; }
            set
            {
                if (FMode != value)
                {
                    FMode = value;
                    OnChangeMode?.Invoke();
                }
            }
        }

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
        public delegate void DChangeMode();
        public event DChangeMode OnChangeMode;
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
        }
    }

    public struct ClientValues
    {
        public string Name;
        public string Number;
        public string WifiSSID;
        public string WifiPassword;
        public ushort UDPPort;
        public string LEDCount;
        public string RelativeFolderPath;
    }

    /// <summary>
    /// Информация о настройке и состоянии клиента
    /// </summary>
    [DataContract]
    public class Client
    {
        //Fields
        [DataMember]
        public Project Parent;
        [DataMember]
        private bool FSaved;
        [DataMember]
        private string FStatusString;
        [DataMember]
        private string FName;
        [DataMember]
        private string FOldRelativePath;
        [DataMember]
        public bool Renamed;
        [DataMember]
        private byte FNumber;
        [DataMember]
        private string FWifiSSID;
        [DataMember]
        private string FWifiPassword;
        [DataMember]
        private ushort FUDPPort;
        [DataMember]
        private string FLEDCount;
        [DataMember]
        private bool Status;
        [DataMember]
        private uint Time;
        [DataMember]
        private bool FPinListIsLock;
        [DataMember]
        private Cyclogramm FCyclogramm;
        private IPAddress FIPAdress;
        
        // Properties       
        public IPAddress IPAdress
        {
            get { return FIPAdress; }
            set { FIPAdress = value; }
        }        
        public bool Saved
        {
            get { return FSaved; }
            set
            {
                if (!value && Parent != null)
                {
                    Parent.Saved = false;
                }
                FSaved = value;
            }
        }
        public string OldRelativePath
        {
            get { return FOldRelativePath; }
            set
            {
                FOldRelativePath = value;
            }
        }
        public string Name
        {
            get { return FName; }
            set
            {
                if (FName != value)
                {
                    if (FName == null)
                    {
                        FName = value;
                        Saved = false;
                    }
                    else
                    {
                        OldRelativePath = RelativePath;
                        FName = value;
                        RelativePath = "\\" + Parent.ClientsFolderName + "\\" + FName;
                        Saved = false;
                        Renamed = true;
                        OnChange?.Invoke();
                    }
                }
            }
        }
        public byte Number
        {
            get { return FNumber; }
            set
            {
                FNumber = value;
                OnChange?.Invoke();
            }
        }
        public string WifiSSID
        {
            get
            {
                return FWifiSSID;
            }

            set
            {
                if (FWifiSSID != value)
                {
                    FWifiSSID = value;
                    Saved = false;
                }
            }
        }
        public string WifiPassword
        {
            get
            {
                return FWifiPassword;
            }

            set
            {
                if (FWifiPassword != value)
                {
                    FWifiPassword = value;
                    Saved = false;
                }
            }
        }
        public ushort UDPPort
        {
            get
            {
                return FUDPPort;
            }

            set
            {
                if (FUDPPort != value)
                {
                    FUDPPort = value;
                    Saved = false;
                }
            }
        }
        public string LEDCount
        {
            get
            {
                return FLEDCount;
            }

            set
            {
                if (FLEDCount != value)
                {
                    FLEDCount = value;
                    Saved = false;
                }
            }
        }
        [DataMember]
        public string RelativePath { get; set; }
        public bool WaitPlayingStatus { get; set; }
        public string StatusString
        {
            get { return FStatusString; }
        }
        public bool OnlineStatus
        {
            get { return Status; }
            set
            {
                Status = value;
                //OnChangeStatus?.Invoke();
                if (Status)
                {
                    FStatusString = "Онлайн";
                }
                else
                {
                    FStatusString = "Оффлайн";
                }
                //OnChange?.Invoke();
                //this.FStatusString = "";
                OnChangeStatus?.Invoke();
            }
        }
        public uint OnlineTime
        {
            get { return Time; }
            set
            {
                Time = value;
                if ((Time > 1000) & (OnlineStatus))
                {
                    OnlineStatus = false;
                }
                else if ((Time <= 1000) & (!OnlineStatus))
                {
                    OnlineStatus = true;
                }
            }
        }
        public bool PinListIsLock
        {
            get { return FPinListIsLock; }
            set
            {
                FPinListIsLock = value;
                OnPinListLocked?.Invoke(value);
            }
        }
        [DataMember]
        public List<Pin> PinList { get; set; }
        public Cyclogramm Cyclogramm
        {
            get { return FCyclogramm; }
            set
            {
                FCyclogramm = value;
                Saved = false;
                //OnChangeCurrentCyclogramm?.Invoke(this, FCyclogramm);
            }
        }
        public Cyclogramm DeletedCyclogramm { get; set; }

        // EVENTS  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public delegate void Change();
        public event Change OnChange;
        public delegate void ChangeStatus();
        public event ChangeStatus OnChangeStatus;
        public delegate void ChangeCurrentCyclogramm(object sender, Cyclogramm CurrentCyclogramm);
        public event ChangeCurrentCyclogramm OnChangeCurrentCyclogramm;
        public delegate void ChangePinList();
        public event ChangePinList OnChangePinList;
        public delegate void PinListLocked(bool Value);
        public event PinListLocked OnPinListLocked;
        //public delegate void ChangeCyclogrammList();
        //public event ChangeCyclogrammList OnChangeCyclogrammList;
        //public delegate void RefreshCyclogrammList();
        //public event RefreshCyclogrammList OnRefreshCyclogrammList;


        // METHODS  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public Client(string CName, string CNumber)
        {
            byte TmpNumber;
            if (!byte.TryParse(CNumber, out TmpNumber))
            {
                return;
            }
            if (CName == "")
            {
                return;
            }
            PinList = new List<Pin>();
            //CyclogrammList = new List<Cyclogramm>();
            Name = CName;
            Number = TmpNumber;
            OnlineStatus = false;
            FPinListIsLock = false;
            Time = 500;
        }
        public void SaveClientSettingFile(string FullPath)
        {
            FileStream FileOutput;
            try
            {
                if (File.Exists(FullPath))
                {
                    File.Delete(FullPath);
                }
                FileOutput = new FileStream(FullPath, FileMode.OpenOrCreate, FileAccess.Write);

            }
            catch (Exception)
            {
                //MessageBox.Show(e.Message, "Ошибка");
                return;
            }

            if (FileOutput != null)
            {
                StreamWriter ClienSettingFile = new StreamWriter(FileOutput, Encoding.ASCII);
                ClienSettingFile.WriteLine("SSID=" + this.WifiSSID + ";");
                ClienSettingFile.WriteLine("Password=" + this.WifiPassword + ";");
                ClienSettingFile.WriteLine("PlateNumber=" + this.Number + ";");
                ClienSettingFile.WriteLine("ProjectKey=" + this.Parent.Key.ToString() + ";");
                ClienSettingFile.WriteLine("UDPPackageSize=200;");
                ClienSettingFile.WriteLine("UDPPort=" + this.UDPPort + ";");
                ClienSettingFile.WriteLine("LEDCount=" + this.LEDCount + ";");
                ClienSettingFile.WriteLine("RefreshInterval=50;");
                if (this.PinList != null)
                {
                    string tmpPins = "Pins=";
                    for (int i = 0; i < this.PinList.Count; i++)
                    {
                        tmpPins += this.PinList[i].PinNumber.ToString() + "-" + this.PinList[i].LEDCount.ToString();
                        if (i == this.PinList.Count - 1)
                        {
                            tmpPins += ";";
                        }
                        else
                        {
                            tmpPins += ",";
                        }
                    }
                    ClienSettingFile.WriteLine(tmpPins);
                }

                ClienSettingFile.Close();
                FileOutput.Close();
            }
        }
        public void AddPin(string _Pin, string _LEDCount)
        {
            if (PinListIsLock)
            {
                return;
            }
            byte TmpPin;
            if (!byte.TryParse(_Pin, out TmpPin))
            {
                return;
            }

            ushort TmpLEDCount;
            if (!ushort.TryParse(_LEDCount, out TmpLEDCount))
            {
                return;
            }

            if ((TmpLEDCount == 0))
            {
                return;
            }

            int t = PinList.FindIndex(x => x.PinNumber == TmpPin);
            if (t >= 0)
            {
                PinList[t].LEDCount = TmpLEDCount;
                OnChangePinList?.Invoke();
                Saved = false;
            }
            else
            {
                PinList.Add(new Pin(_Pin, _LEDCount));
                OnChangePinList?.Invoke();
                Saved = false;
            }
        }
        public void DeletePin(Pin pin)
        {
            if (PinList == null)
            {
                return;
            }
            if (!PinListIsLock)
            {
                if (PinList.Remove(pin))
                {
                    Saved = false;
                }
            }
        }
        //public int AddCyclogramm(string Name, string InCycFile)
        //{
        //    int result = -1;
        //    if ((Name == "") || (InCycFile == ""))
        //    {
        //        return result;
        //    }
        //    int t = CyclogrammList.FindIndex(x => x.Name == Name);
        //    if (t < 0)
        //    {
        //        Cyclogramm TmpCyclogramm = new Cyclogramm();
        //        TmpCyclogramm.Name = Name;
        //        TmpCyclogramm.Parent = this;
        //        TmpCyclogramm.InputFileName = InCycFile;
        //        TmpCyclogramm.Converted = false;
        //        TmpCyclogramm.OnConverted += () =>
        //        {
        //            //OnRefreshCyclogrammList?.Invoke();
        //        };
        //        TmpCyclogramm.OnFinalFlagChange += TmpCyclogramm_OnFinalFlagChange;
        //        CyclogrammList.Add(TmpCyclogramm);
        //        result = CyclogrammList.IndexOf(TmpCyclogramm);
        //        OnChangeCyclogrammList?.Invoke();
        //        Saved = false;
        //    }
        //    return result;
        //}
        //public void DeleteCyclogramm(Cyclogramm cyclogramm)
        //{
        //    if (CyclogrammList == null)
        //    {
        //        return;
        //    }
        //    if (DeletedCyclogrammList == null)
        //    {
        //        DeletedCyclogrammList = new List<Cyclogramm>();
        //    }
        //    if (CyclogrammList.Remove(cyclogramm))
        //    {
        //        DeletedCyclogrammList.Add(cyclogramm);
        //        OnChangeCyclogrammList?.Invoke();
        //        Saved = false;
        //    }
        //}
        public void Send_PlayFrom_12(TimeSpan Time)
        {
            uint FrameTime = (uint)(Time.TotalMilliseconds / 50);
            byte[] Content = new byte[5];
            Content[0] = Number;
            Content[1] = (byte)(FrameTime >> 24);
            Content[2] = (byte)(FrameTime >> 16);
            Content[3] = (byte)(FrameTime >> 8);
            Content[4] = (byte)(FrameTime >> 0);            
            Parent.Server.SendCommand(IPAdress, UDPPort, 12, Content);
        }
    }

    /// <summary>
    ///  Информация о пине используемом на плате клиента
    /// </summary>
    [DataContract]
    public class Pin
    {
        // Fields
        [DataMember]
        private byte PPinNumber;
        [DataMember]
        private ushort PLEDCount;

        // Propeties
        public byte PinNumber
        {
            get { return PPinNumber; }
            set
            {
                PPinNumber = value;
                OnChange?.Invoke();
            }
        }
        public ushort LEDCount
        {
            get { return PLEDCount; }
            set
            {
                PLEDCount = value;
                OnChange?.Invoke();
            }
        }

        // Events
        public delegate void Change();
        public event Change OnChange;

        // Methods
        public Pin(string _Pin, string _LEDCount)
        {
            byte TmpPin;
            if (!byte.TryParse(_Pin, out TmpPin))
            {
                return;
            }
            ushort TmpLEDCount;
            if (!ushort.TryParse(_LEDCount, out TmpLEDCount))
            {
                return;
            }
            PinNumber = TmpPin;
            LEDCount = TmpLEDCount;
        }

    }

    /// <summary>
    /// Информация о циклограмме воспроизводимой клиентом
    /// </summary>
    [DataContract]
    public class Cyclogramm
    {
        // Fields
        [DataMember]
        private string FName;
        [DataMember]
        public Client Parent;
        [DataMember]
        public int FileSize { get; set; }
        //private string FOutputFileName;
        [DataMember]
        private bool FConverted;
        [DataMember]
        private string FConvertedStr;
        private bool FSaved;

        // Properties
        public string Name
        {
            get { return FName; }
            set
            {
                FName = value;
                OnChange?.Invoke();
            }
        }
        
        //public string OutputFileName
        //{
        //    get { return FOutputFileName; }
        //    set
        //    {
        //        FOutputFileName = value;
        //        OnChange?.Invoke();
        //    }
        //}
        public bool Saved
        {
            get { return FSaved; }
            set
            {
                FSaved = value;
                if (FSaved == false)
                {
                    Parent.Saved = false;
                }
            }
        }
        public bool Converted
        {
            get { return FConverted; }
            set
            {
                FConverted = value;
                if (FConverted == true)
                {
                    FConvertedStr = "Да";
                }
                else
                {
                    FConvertedStr = "Нет";
                }
                OnConverted?.Invoke();
            }
        }
        public string ConvertedStr
        {
            get { return FConvertedStr; }
            set
            {
                FConvertedStr = value;
                if (FConverted == true)
                {
                    FConvertedStr = "Да";
                }
                else
                {
                    if (FConvertedStr == "")
                    {
                        FConvertedStr = "Нет";
                    }
                }
                OnConverted?.Invoke();
            }
        }

        // Events
        public delegate void ChangeCyclogram();
        public event ChangeCyclogram OnChange;
        public delegate void ConvertedCyclogramm();
        public event ConvertedCyclogramm OnConverted;
        //public delegate void FinalFlagChange(object sender);
        //public event FinalFlagChange OnFinalFlagChange;
    }

    /// <summary>
    /// Класс осуществляющий сохранение данных в XML файл
    /// </summary>
    public class XMLSaver
    {
        public Project Fields;

        //Запись настроек в файл
        public void WriteXml(string SavePath)
        {
            if (Fields != null)
            {
                XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };
                XmlWriter w = XmlDictionaryWriter.Create(SavePath, settings);
                NetDataContractSerializer ser = new NetDataContractSerializer();
                ser.WriteObject(w, Fields);
                w.Close();
            }


        }
        //Чтение насроек из файла
        public void ReadXml(string OpenPath)
        {
            if (Fields != null)
            {
                if (File.Exists(OpenPath))
                {
                    FileStream fs = new FileStream(OpenPath, FileMode.Open);
                    XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                    NetDataContractSerializer ser = new NetDataContractSerializer();
                    Fields = (ser.ReadObject(reader, true) as Project);
                    fs.Close();

                }
            }
        }
    }

    /// <summary>
    /// Класс осуществляющий передачу и прием информации с клиентов по протоколу UDP
    /// </summary>
    [DataContract]
    public class UDPServer
    {
        // Fields
        private UdpClient PackageSender;
        private UdpClient PackageReceiver;
        private IPAddress PServerIPAdress;
        private IPEndPoint endPoint;
        private bool stop;
        [DataMember]
        private ushort PUDPport;
        private bool PIsRun;
        private uint PSetTime = 0;

        // Properties
        public IPAddress ServerIPAdress
        {
            get { return PServerIPAdress; }
            set
            {
                PServerIPAdress = value;
                OnServerIPChange?.Invoke();
            }
        }
        public bool IsRun
        {
            get { return PIsRun; }
        }
        private bool PropIsRun
        {
            get { return PIsRun; }
            set
            {
                PIsRun = value;
                OnStatusChange?.Invoke();
            }
        }
        [DataMember]
        public uint ProjectKey { get; set; }
        public uint SetTime
        {
            get { return PSetTime; }
            set { PSetTime = value; }
        }
        public ushort UDPPort
        {
            get { return PUDPport; }
            set { PUDPport = value; }
        }

        // Events
        //public delegate void SendString(string s);
        //public event SendString OnParsePackageSendMessage;
        public delegate void SendCyclogrammName(byte ClientNumber, byte CyclogrammSendType, string CyclogrammName);
        public event SendCyclogrammName OnSendCyclogrammName;
        public delegate void SendFinalCyclogrammName(byte ClientNumber, string CyclogrammName);
        public event SendFinalCyclogrammName OnSendFinalCyclogrammName;
        public delegate void SendNumberPlate(byte s, IPEndPoint IP, ClientState clientState);
        public event SendNumberPlate OnSendNumberPlate;
        public delegate void SendTypePackage(byte Type);
        public event SendTypePackage OnParsePackage;
        public delegate void StatusChange();
        public event StatusChange OnStatusChange;
        public delegate void ServerIPChange();
        public event ServerIPChange OnServerIPChange;

        // Methods
        public UDPServer(uint Key)
        {
            ProjectKey = Key;
            PropIsRun = false;
        }
        public void StartReceiving()
        {
            if (PUDPport > 0)
            {
                stop = false;
                PackageReceiver = new UdpClient(PUDPport);
                if (PackageReceiver != null)
                {
                    PropIsRun = true;
                }
                Receive();
            }
        }
        public void StopReceiving()
        {
            stop = true;
            PackageReceiver.Client.Close();
            PackageReceiver = null;
            Thread.Sleep(500);
            PropIsRun = false;
        }
        private void Receive()
        {
            PackageReceiver.BeginReceive(new AsyncCallback(MyReceiveCallback), null);
        }
        private void MyReceiveCallback(IAsyncResult result)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
            if (!stop)
            {
                Byte[] receiveBytes = PackageReceiver.EndReceive(result, ref ip);
                ParsePackage(receiveBytes, ip);
                Receive();
            }
        }
        public void ParsePackage(byte[] bytes, IPEndPoint SenderIp)
        {
            if (bytes.Length != 200)
            {
                return;
            }

            uint g = (uint)((bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + (bytes[3] << 0));

            if (g == ProjectKey)
            {
                switch (bytes[4])
                {
                    case 3:
                        ClientState state;// = new ClientState();
                        switch (bytes[8])
                        {
                            case 1:
                                state = ClientState.Wait;
                                break;
                            case 2:
                                state = ClientState.Play;
                                break;
                            case 3:
                                state = ClientState.Pause;
                                break;
                            default:
                                state = ClientState.Wait;
                                break;
                        }
                        
                        OnSendNumberPlate?.Invoke(bytes[7], SenderIp, state);
                        break;
                    case 8:
                        SendCommand(SenderIp.Address, UDPPort, 5, new byte[0]);
                        break;
                    case 10:
                        ushort ContentLength = (ushort)((bytes[5] << 8) + (bytes[6] << 0));
                        byte ClientNumber = bytes[7];
                        byte CyclogrammSendType = bytes[8];
                        byte[] CycName = new byte[ContentLength - 2];
                        Array.Copy(bytes, 9, CycName, 0, ContentLength - 2);

                        string Name = System.Text.Encoding.ASCII.GetString(CycName);

                        OnSendCyclogrammName?.Invoke(ClientNumber, CyclogrammSendType, Name);
                        break;
                    case 14:
                        ushort ContentLength2 = (ushort)((bytes[5] << 8) + (bytes[6] << 0));
                        byte ClientNumber2 = bytes[7];
                        byte[] CycName2 = new byte[ContentLength2 - 1];
                        Array.Copy(bytes, 8, CycName2, 0, ContentLength2 - 1);

                        string Name2 = System.Text.Encoding.ASCII.GetString(CycName2);

                        OnSendFinalCyclogrammName?.Invoke(ClientNumber2, Name2);
                        break;
                    default:
                        break;
                }
                OnParsePackage?.Invoke(bytes[4]);
            }
        }

        //public void CommandSender(byte cmd, IPAddress IP, int Port)
        //{
        //    if (cmd == 9 || cmd == 13)
        //    {
        //        return;
        //    }

        //    CommandSender1(cmd, IP, Port, new byte[0]);
        //}

        //public void CommandSender(IPAddress IP, int Port, byte ClientNumber, string CyclogrammName, byte StringCommand)
        //{
        //    byte[] tmpBytes = System.Text.Encoding.ASCII.GetBytes(CyclogrammName);
        //    byte[] content = new byte[tmpBytes.Length + 1];
        //    content[0] = ClientNumber;
        //    Array.Copy(tmpBytes, 0, content, 1, tmpBytes.Length);
        //    CommandSender1(StringCommand, IP, Port, content);
        //}

        //public void CommandSender1(byte cmd, IPAddress IP, int Port, byte[] Content)
        //{
        //    ushort ContentLength;
        //    byte[] ByteArray = new byte[200];
        //    ByteArray[0] = (byte)(ProjectKey >> 24);
        //    ByteArray[1] = (byte)(ProjectKey >> 16);
        //    ByteArray[2] = (byte)(ProjectKey >> 8);
        //    ByteArray[3] = (byte)(ProjectKey >> 0);
        //    ByteArray[4] = cmd;
        //    switch (cmd)
        //    {
        //        case 1:
        //            ContentLength = 1;
        //            ByteArray[5] = (byte)(ContentLength >> 8);
        //            ByteArray[6] = (byte)(ContentLength >> 0);
        //            ByteArray[7] = 0;
        //            for (int i = 8; i < ByteArray.Length - 1; i++)
        //            {
        //                ByteArray[i] = 0;
        //            }
        //            UDPSend(ByteArray, IP, Port);
        //            break;
        //        case 2:
        //            ContentLength = 1;
        //            ByteArray[5] = (byte)(ContentLength >> 8);
        //            ByteArray[6] = (byte)(ContentLength >> 0);
        //            ByteArray[7] = 0;
        //            for (int i = 8; i < ByteArray.Length - 1; i++)
        //            {
        //                ByteArray[i] = 0;
        //            }
        //            UDPSend(ByteArray, IP, Port);
        //            break;
        //        //case 4:
        //        //    for (int i = 5; i < ByteArray.Length - 1; i++)
        //        //    {
        //        //        ByteArray[i] = 0;
        //        //    }
        //        //    UDPSend(ByteArray, IP, Port);
        //        //    break;
        //        case 5:
        //            ContentLength = 4;
        //            byte[] TempBytes = PServerIPAdress.GetAddressBytes();
        //            ByteArray[5] = (byte)(ContentLength >> 8);
        //            ByteArray[6] = (byte)(ContentLength >> 0);
        //            ByteArray[7] = TempBytes[0];
        //            ByteArray[8] = TempBytes[1];
        //            ByteArray[9] = TempBytes[2];
        //            ByteArray[10] = TempBytes[3];

        //            for (int i = 11; i < ByteArray.Length - 1; i++)
        //            {
        //                ByteArray[i] = 0;
        //            }
        //            UDPSend(ByteArray, IP, Port);
        //            break;
        //        case 6:
        //            for (int i = 5; i < ByteArray.Length - 1; i++)
        //            {
        //                ByteArray[i] = 0;
        //            }
        //            UDPSend(ByteArray, IP, Port);
        //            break;
        //        case 7:
        //            ContentLength = 4;
        //            ByteArray[5] = (byte)(ContentLength >> 8);
        //            ByteArray[6] = (byte)(ContentLength >> 0);
        //            ByteArray[7] = (byte)(PSetTime >> 24);
        //            ByteArray[8] = (byte)(PSetTime >> 16);
        //            ByteArray[9] = (byte)(PSetTime >> 8);
        //            ByteArray[10] = (byte)(PSetTime >> 0);

        //            for (int i = 11; i < ByteArray.Length - 1; i++)
        //            {
        //                ByteArray[i] = 0;
        //            }
        //            UDPSend(ByteArray, IP, Port);
        //            break;
        //        case 9:
        //            ContentLength = (ushort)Content.Length;
        //            ByteArray[5] = (byte)(ContentLength >> 8);
        //            ByteArray[6] = (byte)(ContentLength >> 0);
        //            Array.Copy(Content, 0, ByteArray, 7, Content.Length);
        //            for (int i = 7 + ContentLength; i < ByteArray.Length - 1; i++)
        //            {
        //                ByteArray[i] = 0;
        //            }
        //            UDPSend(ByteArray, IP, Port);
        //            break;
        //        case 13:
        //            ContentLength = (ushort)Content.Length;
        //            ByteArray[5] = (byte)(ContentLength >> 8);
        //            ByteArray[6] = (byte)(ContentLength >> 0);
        //            Array.Copy(Content, 0, ByteArray, 7, Content.Length);
        //            for (int i = 7 + ContentLength; i < ByteArray.Length - 1; i++)
        //            {
        //                ByteArray[i] = 0;
        //            }
        //            UDPSend(ByteArray, IP, Port);
        //            break;
        //        default:
        //            break;
        //    }
        //}

        public void Send_PlayAll_1()
        {
            SendCommand(IPAddress.Broadcast, UDPPort, 1, new byte[0]);
        }

        public void Send_StopAll_2()
        {
            SendCommand(IPAddress.Broadcast, UDPPort, 2, new byte[0]);
        }

        public void Send_PauseAll_6()
        {
            SendCommand(IPAddress.Broadcast, UDPPort, 6, new byte[0]);
        }

        public void Send_PlayFromAll_7(TimeSpan Time)
        {
            uint FrameTime = (uint)(Time.TotalMilliseconds / 50D);
            byte[] Content = new byte[4];
            Content[0] = (byte)(FrameTime >> 24);
            Content[1] = (byte)(FrameTime >> 16);
            Content[2] = (byte)(FrameTime >> 8);
            Content[3] = (byte)(FrameTime >> 0);
            SendCommand(IPAddress.Broadcast, UDPPort, 7, Content);
        }

        public void SendCommand(IPAddress IP, int Port, byte cmd, byte[] Content)
        {
            ushort ContentLength;
            byte[] ByteArray = new byte[200];
            ByteArray[0] = (byte)(ProjectKey >> 24);
            ByteArray[1] = (byte)(ProjectKey >> 16);
            ByteArray[2] = (byte)(ProjectKey >> 8);
            ByteArray[3] = (byte)(ProjectKey >> 0);
            ByteArray[4] = cmd;
            switch (cmd)
            {
                case 1:
                    for (int i = 5; i < ByteArray.Length - 1; i++)
                    {
                        ByteArray[i] = 0;
                    }
                    UDPSend(ByteArray, IP, Port);
                    break;
                case 2:
                    for (int i = 5; i < ByteArray.Length - 1; i++)
                    {
                        ByteArray[i] = 0;
                    }
                    UDPSend(ByteArray, IP, Port);
                    break;
                //case 4:
                //    for (int i = 5; i < ByteArray.Length - 1; i++)
                //    {
                //        ByteArray[i] = 0;
                //    }
                //    UDPSend(ByteArray, IP, Port);
                //    break;
                case 5:
                    ContentLength = 4;
                    byte[] IPAdressBytes = PServerIPAdress.GetAddressBytes();
                    ByteArray[5] = (byte)(ContentLength >> 8);
                    ByteArray[6] = (byte)(ContentLength >> 0);
                    ByteArray[7] = IPAdressBytes[0];
                    ByteArray[8] = IPAdressBytes[1];
                    ByteArray[9] = IPAdressBytes[2];
                    ByteArray[10] = IPAdressBytes[3];
                    for (int i = 11; i < ByteArray.Length - 1; i++)
                    {
                        ByteArray[i] = 0;
                    }
                    UDPSend(ByteArray, IP, Port);
                    break;
                case 6:
                    for (int i = 5; i < ByteArray.Length - 1; i++)
                    {
                        ByteArray[i] = 0;
                    }
                    UDPSend(ByteArray, IP, Port);
                    break;
                case 7:
                    ContentLength = (ushort)Content.Length;
                    if (ContentLength == 4)
                    {
                        ByteArray[5] = (byte)(ContentLength >> 8);
                        ByteArray[6] = (byte)(ContentLength >> 0);
                        ByteArray[7] = Content[0];
                        ByteArray[8] = Content[1];
                        ByteArray[9] = Content[2];
                        ByteArray[10] = Content[3];

                        for (int i = 11; i < ByteArray.Length - 1; i++)
                        {
                            ByteArray[i] = 0;
                        }
                        UDPSend(ByteArray, IP, Port);
                    }
                    break;
                case 9:
                    ContentLength = (ushort)Content.Length;
                    ByteArray[5] = (byte)(ContentLength >> 8);
                    ByteArray[6] = (byte)(ContentLength >> 0);
                    Array.Copy(Content, 0, ByteArray, 7, Content.Length);
                    for (int i = 7 + ContentLength; i < ByteArray.Length - 1; i++)
                    {
                        ByteArray[i] = 0;
                    }
                    UDPSend(ByteArray, IP, Port);
                    break;
                case 12:
                    ContentLength = (ushort)Content.Length;
                    if (ContentLength == 5)
                    {
                        ByteArray[5] = (byte)(ContentLength >> 8);
                        ByteArray[6] = (byte)(ContentLength >> 0);
                        ByteArray[7] = Content[0];
                        ByteArray[8] = Content[1];
                        ByteArray[9] = Content[2];
                        ByteArray[10] = Content[3];
                        ByteArray[11] = Content[4];

                        for (int i = 12; i < ByteArray.Length - 1; i++)
                        {
                            ByteArray[i] = 0;
                        }
                        UDPSend(ByteArray, IP, Port);
                    }
                    break;
                case 13:
                    ContentLength = (ushort)Content.Length;
                    ByteArray[5] = (byte)(ContentLength >> 8);
                    ByteArray[6] = (byte)(ContentLength >> 0);
                    Array.Copy(Content, 0, ByteArray, 7, Content.Length);
                    for (int i = 7 + ContentLength; i < ByteArray.Length - 1; i++)
                    {
                        ByteArray[i] = 0;
                    }
                    UDPSend(ByteArray, IP, Port);
                    break;
                default:
                    break;
            }
        }



        //Отправляет UDP дейтаграмму содержащую массив байтов
        public void UDPSend(byte[] bytes, IPAddress remoteIPAddress, int remotePort)
        {
            if (remoteIPAddress != null)
            {
                PackageSender = new UdpClient();
                PackageSender.EnableBroadcast = true;
                endPoint = new IPEndPoint(remoteIPAddress, remotePort);
                try
                {
                    PackageSender.Send(bytes, bytes.Length, endPoint);
                }
                finally
                {
                    PackageSender.Close();
                    endPoint = null;
                }
            }
        }
    }

    //public class ClientPacketSender
    //{
    //    public Client Parent;
    //    public void Send_SelectCyclogrammName_9(string CyclogrammName)
    //    {
    //        byte[] Content = new byte[CyclogrammName.Length + 1];
    //        Content[0] = Parent.Number;
    //        Array.Copy(System.Text.Encoding.ASCII.GetBytes(CyclogrammName), 0, Content, 1, CyclogrammName.Length);
    //        //Parent.Parent.Server.SendCommand(, , 9, Content);
    //    }
    //    public void Send_PlayFrom_12(TimeSpan Time)
    //    {

    //    }
    //    public void Send_DefaultCyclogrammName_13(string CyclogrammName)
    //    {

    //    }
    //}

    //public class CyclogrammConverter
    //{
    //    private string OutFileName;
    //    private string InputFileName;

    //    public CyclogrammConverter(string POutFileName, string InputFile)
    //    {
    //        OutFileName = POutFileName;
    //        InputFileName = InputFile;
    //    }

    //    public delegate void ConverterSendString(byte s);

    //    public event ConverterSendString OnConverterSendMessage;

    //    public delegate void ConvertionDone(int ThreadID, bool Done);
    //    public event ConvertionDone OnConvertionDone;

    //    public void Start()
    //    {
    //        try
    //        {
    //            FileStream FileInput = new FileStream(InputFileName, FileMode.Open, FileAccess.Read);
    //            byte CurrentByte;
    //            int CurrentByteBuffer;
    //            byte Percent = 0;
    //            uint[] ArPer = new uint[100];
    //            ArPer[0] = (uint)(FileInput.Length / 100);
    //            for (int i = 1; i < 99; i++)
    //            {
    //                ArPer[i] += ArPer[i - 1] + ArPer[0];
    //            }
    //            int ColorByteCount = 1; //счетчик от 1 до 3 (Определяет значение одного цвета)
    //            string colorBuffer = "";
    //            FileStream FileOutput = new FileStream(OutFileName, FileMode.OpenOrCreate, FileAccess.Write);
    //            FileOutput.Position = FileOutput.Length;
    //            while (FileInput.Position <= FileInput.Length)
    //            {
    //                if (ArPer[Percent] == FileInput.Position)
    //                {
    //                    OnConverterSendMessage(Percent);
    //                    Percent++;
    //                }

    //                CurrentByteBuffer = FileInput.ReadByte();
    //                if (CurrentByteBuffer > 0)
    //                {
    //                    CurrentByte = (byte)(CurrentByteBuffer);
    //                }
    //                else
    //                {
    //                    break;
    //                }

    //                // Проверка, является ли текущий байт запятой "," символом перевода на новую строку или символом возврата каретки
    //                if ((CurrentByte == 44) || (CurrentByte == 13) || (CurrentByte == 10))
    //                    continue;

    //                if (ColorByteCount <= 3)
    //                {
    //                    colorBuffer += Convert.ToChar(CurrentByte);
    //                    ColorByteCount++;
    //                }

    //                if (ColorByteCount == 4)
    //                {
    //                    ColorByteCount = 1;
    //                    FileOutput.WriteByte(Convert.ToByte(colorBuffer));
    //                    colorBuffer = "";
    //                }
    //            }
    //            FileInput.Close();
    //            FileOutput.Close();
    //            OnConvertionDone?.Invoke(Thread.CurrentThread.ManagedThreadId, true);
    //        }
    //        catch (Exception)
    //        {
    //            OnConvertionDone?.Invoke(Thread.CurrentThread.ManagedThreadId, false);
    //        }
    //    }
    //}

}