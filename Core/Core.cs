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
}