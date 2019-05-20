using System;
using System.Linq;
using System.Net;
using Core;
using Core.ClientConnectionService;
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
        private readonly IRemoteClientConnectionService clientConnectionService;

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
        
        public RemoteClientsOperator(IControlUnit controlUnit, INetworkSettingProvider networkSettingProvider, IRemoteClientConnectionService remoteClientConnector)
        {
            this.controlUnit = controlUnit;
            this.networkSettingProvider = networkSettingProvider;
            this.clientConnectionService = remoteClientConnector;
            State = OperatorStates.Configure;
        }
        
        private void ProcessMessage(INumeredClient client, RLCMessage message)
        {
            if(message.SourceType != SourceType.Client) {
                logger.Info($"Получено сообщение не типа {nameof(SourceType.Client)}, игнорируется.");
                return;
            }
            switch(message.MessageType) {
                case MessageType.ClientInfo:
                    break;
                default:
                    break;
            }

            UpdateClientInfo(client, message);
        }

        #region private methods

        private void UpdateClientInfo(INumeredClient client, RLCMessage message)
        {
            RemoteClient foundClient = controlUnit.Clients.FirstOrDefault(x => x.Number == client.Number);
            if(foundClient == null) {
                return;
            }
            foundClient.ClientState = message.ClientState;
        }

        /*
        private void SendServerIP(IPAddress ipAddress)
        {
            RLCMessage message =  RLCMessageFactory.SendServerIP(controlUnit.Key, networkSettingProvider.GetServerIPAddress());
            var messageBytes = message.ToArray();
            clientConnectionService.Send(ipAddress, message);
        } */

        #endregion

        public void StartService()
        {
            if(State != OperatorStates.Configure) {
                logger.Warn($"Старт оператора клиентов возможен только из статуса {OperatorStates.Configure}");
                return;
            }

            clientConnectionService.OnReceiveMessage -= TcpService_OnReceivePackage;
            clientConnectionService.OnReceiveMessage += TcpService_OnReceivePackage;
            clientConnectionService.Start();
            State = OperatorStates.Ready;
        }

        public void StopService()
        {
            if(State == OperatorStates.Configure) {
                logger.Warn($"Оператор клиентов уже остановлен");
                return;
            }
            if(clientConnectionService.IsActive) {
                logger.Warn($"TCP служба не запущена");
                return;
            }
            clientConnectionService.Stop();
            clientConnectionService.OnReceiveMessage -= TcpService_OnReceivePackage;
            State = OperatorStates.Configure;
        }

        private void TcpService_OnReceivePackage(object sender, ClientMessageEventArgs e)
        {
            ProcessMessage(e.Client, e.Message);
        }

        public void Play()
        {
            var stateBackup = State;
            try {
                var message = RLCMessageFactory.Play(controlUnit.Key);
                clientConnectionService.SendToAll(message);
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
                var message = RLCMessageFactory.Stop(controlUnit.Key);
                clientConnectionService.SendToAll(message);
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
                var message = RLCMessageFactory.Pause(controlUnit.Key);
                clientConnectionService.SendToAll(message);
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
