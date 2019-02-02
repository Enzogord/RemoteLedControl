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
    [DataContract]
    public class Client
    {
        [DataMember]
        private bool saved;

        [DataMember]
        private string name;
        
        [DataMember]
        private byte number;

        [DataMember]
        private string wifiSSID;

        [DataMember]
        private string wifiPassword;

        [DataMember]
        private ushort udpPort;

        [DataMember]
        private string ledCount;

        [DataMember]
        private bool status;

        [DataMember]
        private uint time;

        [DataMember]
        private Cyclogramm cyclogramm;

        [DataMember]
        public Project Parent { get; set; }

        [DataMember]
        public bool Renamed { get; set; }

        [DataMember]
        public string OldRelativePath { get; set; }

        [DataMember]
        public string RelativePath { get; set; }

        [DataMember]
        public bool PinListIsLock { get; set; }

        [DataMember]
        public List<Pin> PinList { get; set; }

        public IPAddress IPAdress { get; set; }

        public bool Saved {
            get => saved;
            set {
                if (!value && Parent != null) {
                    Parent.Saved = false;
                }
                saved = value;
            }
        }        

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    if (name == null)
                    {
                        name = value;
                        Saved = false;
                    }
                    else
                    {
                        OldRelativePath = RelativePath;
                        name = value;
                        RelativePath = "\\" + Parent.ClientsFolderName + "\\" + name;
                        var fi = new DirectoryInfo(this.Parent.AbsoluteFolderPath + OldRelativePath);
                        Renamed = fi.Exists;
                        Saved = false;
                        OnChange?.Invoke();
                    }
                }
            }
        }
        public byte Number {
            get => number;
            set {
                number = value;
                OnChange?.Invoke();
            }
        }

        public string WifiSSID {
            get => wifiSSID;
            set {
                if (wifiSSID != value) {
                    wifiSSID = value;
                    Saved = false;
                }
            }
        }
        public string WifiPassword {
            get => wifiPassword;
            set {
                if (wifiPassword != value) {
                    wifiPassword = value;
                    Saved = false;
                }
            }
        }

        public ushort UDPPort {
            get => udpPort;
            set {
                if (udpPort != value) {
                    udpPort = value;
                    Saved = false;
                }
            }
        }

        public string LEDCount {
            get => ledCount;
            set {
                if (ledCount != value) {
                    ledCount = value;
                    Saved = false;
                }
            }
        }

        public bool WaitPlayingStatus { get; set; }

        public bool Status
        {
            get { return status; }
            set
            {
                status = value;
                OnChangeStatus?.Invoke();
            }
        }

        public string StatusString => status ? "Онлайн" : "Оффлайн";

        public uint OnlineTime
        {
            get { return time; }
            set
            {
                time = value;
                if ((time > 1000) & (Status))
                {
                    Status = false;
                }
                else if ((time <= 1000) & (!Status))
                {
                    Status = true;
                }
            }
        }

        public Cyclogramm Cyclogramm {
            get => cyclogramm;
            set {
                cyclogramm = value;
                Saved = false;
            }
        }
        public Cyclogramm DeletedCyclogramm { get; set; }

        public delegate void Change();
        public event Change OnChange;
        public delegate void ChangeStatus();
        public event ChangeStatus OnChangeStatus;
        public delegate void ChangePinList();
        public event ChangePinList OnChangePinList;

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
            Name = CName;
            Number = TmpNumber;
            Status = false;
            PinListIsLock = false;
            time = 500;
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
        public void AddPin(string pin, string ledCount)
        {
            if (PinListIsLock)
            {
                return;
            }
            byte TmpPin;
            if (!byte.TryParse(pin, out TmpPin))
            {
                return;
            }

            ushort TmpLEDCount;
            if (!ushort.TryParse(ledCount, out TmpLEDCount))
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
                PinList.Add(new Pin(byte.Parse(pin), ushort.Parse(ledCount)));
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

        public void Send_PlayFrom_12(TimeSpan time)
        {
            uint FrameTime = (uint)(time.TotalMilliseconds / 50);
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
