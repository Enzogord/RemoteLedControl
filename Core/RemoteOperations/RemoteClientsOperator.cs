using System;
using System.Linq;
using System.Net;
using Core.Messages;
using NLog;
using Service;
using TCPCommunicationService;

namespace RLCCore.RemoteOperations
{
    public class RemoteClientsOperator : NotifyPropertyBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IControlUnit controlUnit;
        private readonly INetworkSettingProvider networkSettingProvider;

        TCPService tcpService;
        public event EventHandler<OperatorStateEventArgs> StateChanged;

        private OperatorStates state;
        public OperatorStates State {
            get => state;
            private set {
                if(SetField(ref state, value, () => State)) {
                    StateChanged?.Invoke(this, new OperatorStateEventArgs(state));
                }
            }
        }
        
        public RemoteClientsOperator(IControlUnit controlUnit, INetworkSettingProvider networkSettingProvider)
        {
            this.controlUnit = controlUnit;
            this.networkSettingProvider = networkSettingProvider;
            State = OperatorStates.Configure;
        }
        
        private void OnReceiveMessage(IPAddress address, RLCMessage message)
        {
            if(message.SourceType != SourceType.Client) {
                logger.Info($"Получено сообщение не типа {nameof(SourceType.Client)}, игнорируется.");
                return;
            }
            switch(message.MessageType) {
                case MessageType.ClientInfo:
                    UpdateClientInfo(address, message);
                    break;
                case MessageType.RequestServerIp:
                    //SendServerIP();
                    break;
                default:
                    break;
            }
        }

        /*
        private void CommunicationService_OnReceiveMessage(object sender, IRemoteClientMessage e)
        {
            switch(e.MessageType) {
                case ClientMessages.SendPlateNumber:
                    UpdateClientState(e.ClientNumber);
                    break;
                case ClientMessages.RequestServerIP:
                    SendServerIP();
                    break;
                default:
                    break;
            }
        }*/

        #region private methods

        private void UpdateClientInfo(IPAddress ipAddress, RLCMessage message)
        {
            var client = controlUnit.Clients.FirstOrDefault(x => x.Number == message.ClientNumber);
            if(client == null) {
                return;
            }
            client.IPAddress = ipAddress;
            client.ClientState = message.ClientState;
            //TODO получать состояние клиента из TCP сервиса
            client.UpdateStatus(true);
        }

        private void SendServerIP(IPAddress ipAddress)
        {
            RLCMessage message =  RLCMessageFactory.SendServerIP(controlUnit.Key, networkSettingProvider.GetServerIPAddress());
            var messageBytes = message.ToArray();
            tcpService.Send(ipAddress, messageBytes, messageBytes.Length);
        } 

        #endregion

        public void Start()
        {
            if(State != OperatorStates.Configure) {
                logger.Warn($"Старт оператора клиентов возможен только из статуса {OperatorStates.Configure}");
                return;
            }

            tcpService = new TCPService(new IPEndPoint(networkSettingProvider.GetServerIPAddress(), networkSettingProvider.Port), controlUnit.Clients, 1024);
            tcpService.OnReceivePackage -= TcpService_OnReceivePackage;
            tcpService.OnReceivePackage += TcpService_OnReceivePackage;
            tcpService.Start();
            State = OperatorStates.Ready;
        }

        private void TcpService_OnReceivePackage(IPAddress address, byte[] package)
        {
            RLCMessage message = new RLCMessage();
            try {
                message.FromBytes(package);
            }
            catch(Exception ex) {
                logger.Error(ex, "Не удалось спарсить сообщение.");
                return;
            }

            OnReceiveMessage(address, message);
        }

        public void Play()
        {
            var stateBackup = State;
            try {
                var messageBytes = RLCMessageFactory.Play(controlUnit.Key).ToArray();
                tcpService.SendToAll(messageBytes, messageBytes.Length);
                State = OperatorStates.Play;
            }
            catch(Exception ex) {
                logger.Error(ex, $"Не удалось отправить команду {OperatorStates.Play}");
                //восстановление состояния
                State = stateBackup;
                throw;
            }
        }

        public void Stop()
        {
            var stateBackup = State;
            try {
                var messageBytes = RLCMessageFactory.Stop(controlUnit.Key).ToArray();
                tcpService.SendToAll(messageBytes, messageBytes.Length);
                State = OperatorStates.Stop;
            }
            catch(Exception ex) {
                logger.Error(ex, $"Не удалось отправить команду {OperatorStates.Stop}");
                //восстановление состояния
                State = stateBackup;
                throw;
            }
        }

        public void Pause()
        {
            var stateBackup = State;
            try {
                var messageBytes = RLCMessageFactory.Pause(controlUnit.Key).ToArray();
                tcpService.SendToAll(messageBytes, messageBytes.Length);
                State = OperatorStates.Pause;
            }
            catch(Exception ex) {
                logger.Error(ex, $"Не удалось отправить команду {OperatorStates.Pause}");
                //восстановление состояния
                State = stateBackup;
                throw;
            }
        }

        public void PlayFrom()
        {
            /*var state = GlobalCommands.PlayFrom;
            var playFromContext = globalContextProvider.GetCommandContext(state);
            communicationService.SendGlobalCommand(state, playFromContext);*/


            /*RLCMessage message =  RLCMessageFactory.PlayFrom(controlUnit.Key, );
            udpService.Send(message, networkSettingProvider.BroadcastIPAddress, networkSettingProvider.Port);
            State = OperatorStates.Play;*/
        }
    }
}
