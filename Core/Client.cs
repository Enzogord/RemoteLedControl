using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
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
                        var fi = new DirectoryInfo(this.Parent.AbsoluteFolderPath + OldRelativePath);
                        Renamed = fi.Exists;
                        Saved = false;
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

        /// <summary>
        /// Ожидание ответа от клиента о воспроизведении
        /// </summary>
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
                PinList.Add(new Pin(byte.Parse(_Pin), ushort.Parse(_LEDCount)));
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

}
