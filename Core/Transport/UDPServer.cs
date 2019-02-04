using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading;

namespace Core
{
    /// <summary>
    /// Класс осуществляющий передачу и прием информации с клиентов по протоколу UDP
    /// </summary>
    [DataContract]
    public class UDPServer
    {
        private UDPTransmiter udpTransmitter;

        private IPEndPoint endPoint;
        private bool stop;

        public IPAddress SubNetBroadcastAddress { get; set; }

        public uint SetTime { get; set; } = 0;

        [DataMember]
        public uint ProjectKey { get; set; }

        [DataMember]
        [Obsolete("Возможно не нужен")]
        public ushort UDPPort { get; set; }

        private IPAddress serverIPAdress;
        [Obsolete("Возможно не нужен")]
        public IPAddress ServerIPAdress {
            get => serverIPAdress;
            set {
                if (serverIPAdress != value) {
                    serverIPAdress = value;
                    OnServerIPChange?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool IsRun => udpTransmitter == null ? false : udpTransmitter.IsRun;

        public bool IsInitialized => udpTransmitter != null;

        public event EventHandler<PlateInfoEventArgs> OnSendNumberPlate;
        public event EventHandler<byte> OnParsePackage;
        public event EventHandler OnStatusChange;
        public event EventHandler OnServerIPChange;

        public UDPServer(uint Key)
        {
            ProjectKey = Key;
        }

        public void Initialize(IPAddress ipAddress, int port)
        {
            ResetTransmitter();
            ServerIPAdress = ipAddress;
            UDPPort = (ushort)port;
            udpTransmitter = new UDPTransmiter(ipAddress, port);
            udpTransmitter.OnReceivePackage += UdpTransmitter_OnReceivePackage;
            udpTransmitter.OnChangeStatus += UdpTransmitter_OnChangeStatus;
        }        

        private void ResetTransmitter()
        {
            if(udpTransmitter == null) {
                return;
            }
            udpTransmitter.OnReceivePackage -= UdpTransmitter_OnReceivePackage;
            udpTransmitter.Dispose();
            udpTransmitter = null;
        }

        public void StartReceiving()
        {
            if(!IsInitialized) {
                return;
            }
            udpTransmitter.StartReceiving();
        }

        public void StopReceiving()
        {
            if(!IsInitialized) {
                return;
            }
            udpTransmitter.StopReceiving();
        }

        private void UdpTransmitter_OnReceivePackage(object sender, ReceivingDataEventArgs e)
        {
            ParsePackage(e.Data, e.Ip);
        }

        private void UdpTransmitter_OnChangeStatus(object sender, EventArgs e)
        {
            OnStatusChange?.Invoke(this, EventArgs.Empty);
        }

        public void ParsePackage(byte[] bytes, IPEndPoint senderIp)
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
                        ClientState state;
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

                        OnSendNumberPlate?.Invoke(this, new PlateInfoEventArgs(bytes[7], senderIp, state));
                        break;
                    case 8:
                        SendCommand(senderIp.Address, UDPPort, 5, new byte[0]);
                        break;
                    case 10:
                        ushort ContentLength = (ushort)((bytes[5] << 8) + (bytes[6] << 0));
                        byte ClientNumber = bytes[7];
                        byte CyclogrammSendType = bytes[8];
                        byte[] CycName = new byte[ContentLength - 2];
                        Array.Copy(bytes, 9, CycName, 0, ContentLength - 2);

                        string Name = System.Text.Encoding.ASCII.GetString(CycName);

                        //вызов события циклограммы
                        break;
                    case 14:
                        ushort ContentLength2 = (ushort)((bytes[5] << 8) + (bytes[6] << 0));
                        byte ClientNumber2 = bytes[7];
                        byte[] CycName2 = new byte[ContentLength2 - 1];
                        Array.Copy(bytes, 8, CycName2, 0, ContentLength2 - 1);

                        string Name2 = System.Text.Encoding.ASCII.GetString(CycName2);

                        //вызов события циклограммы
                        break;
                    default:
                        break;
                }
                OnParsePackage?.Invoke(this, bytes[4]);
            }
        }

        public void Send_PlayAll_1()
        {
            SendCommand(SubNetBroadcastAddress, UDPPort, 1, new byte[0]);
        }

        public void Send_StopAll_2()
        {
            SendCommand(SubNetBroadcastAddress, UDPPort, 2, new byte[0]);
        }

        public void Send_PauseAll_6()
        {
            SendCommand(SubNetBroadcastAddress, UDPPort, 6, new byte[0]);
        }

        public void Send_PlayFromAll_7(TimeSpan Time)
        {
            uint FrameTime = (uint)(Time.TotalMilliseconds / 50D);
            byte[] Content = new byte[4];
            Content[0] = (byte)(FrameTime >> 24);
            Content[1] = (byte)(FrameTime >> 16);
            Content[2] = (byte)(FrameTime >> 8);
            Content[3] = (byte)(FrameTime >> 0);
            SendCommand(SubNetBroadcastAddress, UDPPort, 7, Content);
        }

        public void SendCommand(IPAddress IP, int Port, byte cmd, byte[] Content)
        {
            if(!IsInitialized) {
                return;
            }
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
                    udpTransmitter.UDPSend(ByteArray, IP, Port);
                    break;
                case 2:
                    for (int i = 5; i < ByteArray.Length - 1; i++)
                    {
                        ByteArray[i] = 0;
                    }
                    udpTransmitter.UDPSend(ByteArray, IP, Port);
                    break;
               case 4:
                    break;
                case 5:
                    ContentLength = 4;
                    byte[] IPAdressBytes = serverIPAdress.GetAddressBytes();
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
                    udpTransmitter.UDPSend(ByteArray, IP, Port);
                    break;
                case 6:
                    for (int i = 5; i < ByteArray.Length - 1; i++)
                    {
                        ByteArray[i] = 0;
                    }
                    udpTransmitter.UDPSend(ByteArray, IP, Port);
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
                        udpTransmitter.UDPSend(ByteArray, IP, Port);
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
                    udpTransmitter.UDPSend(ByteArray, IP, Port);
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
                        udpTransmitter.UDPSend(ByteArray, IP, Port);
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
                    udpTransmitter.UDPSend(ByteArray, IP, Port);
                    break;
                default:
                    break;
            }
        }        
    }
}
