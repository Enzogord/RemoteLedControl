using System;
using System.Net;
using NLog;
using UDPTransmission;

namespace UDPCommunication
{
    /// <summary>
    /// Класс осуществляющий передачу и прием информации с клиентов по протоколу UDP
    /// </summary>
    public class UDPService<TMessage>
        where TMessage : class, IUdpMessage, new()
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private UDPTransmiter udpTransmitter;
/*
        private IPEndPoint endPoint;
        private bool stop;

        private IPAddress broadcastIPAddress;
        private IPAddress ipAddress;
        private int port;*/

        public bool IsRun => udpTransmitter == null ? false : udpTransmitter.IsRun;

        public event EventHandler OnStatusChange;
        //public event EventHandler OnServerIPChange;

        public UDPService(IPAddress ipAddress, int port)
        {
            udpTransmitter = new UDPTransmiter(ipAddress, port);
            udpTransmitter.OnReceivePackage += UdpTransmitter_OnReceivePackage;
            udpTransmitter.OnChangeStatus += UdpTransmitter_OnChangeStatus;
        }

        /*
        public void Initialize(IPAddress ipAddress, int port, IPAddress broadcastIPAddress)
        {
            ResetTransmitter();
            this.broadcastIPAddress = broadcastIPAddress;
            this.ipAddress = ipAddress;
            this.port = port;
            udpTransmitter = new UDPTransmiter(ipAddress, port);
            udpTransmitter.OnReceivePackage += UdpTransmitter_OnReceivePackage;
            udpTransmitter.OnChangeStatus += UdpTransmitter_OnChangeStatus;
        }*/

        private void ResetTransmitter()
        {
            if(udpTransmitter == null) {
                return;
            }
            if(udpTransmitter.IsRun) {
                udpTransmitter.StopReceiving();
            }
            udpTransmitter.OnReceivePackage -= UdpTransmitter_OnReceivePackage;
            udpTransmitter.Dispose();
            udpTransmitter = null;
        }

        public void StartReceiving()
        {
            if(udpTransmitter == null || udpTransmitter.IsRun) {
                return;
            }
            udpTransmitter.StartReceiving();
        }

        public void StopReceiving()
        {
            if(udpTransmitter == null || !udpTransmitter.IsRun) {
                return;
            }
            udpTransmitter.StopReceiving();
        }

        public TMessage Parse(byte[] bytes)
        {
            TMessage message = new TMessage();
            try {
                message.FromBytes(bytes);
                return message;
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка при распознавании сообщения");
                return null;
            }
        }

        public event ReceiveMessageEventHandler<TMessage> OnReceiveMessage;

        private void UdpTransmitter_OnReceivePackage(object sender, ReceivingDataEventArgs e)
        {
            TMessage message = Parse(e.Data);
            if(message != null) {
                OnReceiveMessage?.Invoke(this, e.Ip, message);
            }
        }

        private void UdpTransmitter_OnChangeStatus(object sender, EventArgs e)
        {
            OnStatusChange?.Invoke(this, EventArgs.Empty);
        }
        /*
        private void ParsePackage(byte[] package)
        {
            ClientMessageParser parser = new ClientMessageParser();
            var message = parser.ParseMessage(package);
            OnReceiveMessage?.Invoke(this, message);
        }*/

        //Удалить после завершения парсинга сообщений
        /*public void ParsePackage(byte[] bytes, IPEndPoint senderIp)
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
                        SendCommand(senderIp.Address, port, 5, new byte[0]);
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
        }*/

        #region ICommunicationService implementation

        public void Send(TMessage message, IPAddress address, int port)
        {
            udpTransmitter.UDPSend(message.ToArray(), address, port);
        }

        /*
        

        public void SendGlobalCommand(GlobalCommands command, ICommandContext commandContext)
        {
            byte[] package;
            try {
                package = PackageBuilder.CreatePackage(ProjectKey)
                .InsertCommand(command, commandContext)
                .GetPackage();
            }
            catch(Exception ex) {
                logger.Error(ex, "Не удалось сформировать пакет.");
                return;
            }

            udpTransmitter.UDPSend(package, broadcastIPAddress, port);
        }

        public void SendClientCommand(ClientCommands command, IRemoteClient client, ICommandContext commandContext)
        {
            byte[] package;
            try {
                package = PackageBuilder.CreatePackage(ProjectKey)
                .InsertCommand(command, commandContext)
                .GetPackage();
            }
            catch(Exception ex) {
                logger.Error(ex, "Не удалось сформировать пакет.");
                return;
            }
            udpTransmitter.UDPSend(package, client.IPAddress, port);
        }*/

        #endregion
    }
}
