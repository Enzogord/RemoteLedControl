using System;
using System.Linq;
using Core;
using Core.ClientConnectionService;
using Core.Messages;
using NLog;
using Service;

namespace RLCCore.RemoteOperations
{
    public class RemoteClientsOperator : NotifyPropertyBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IControlUnit controlUnit;
        private readonly INetworkSettingProvider networkSettingProvider;
        private readonly IRemoteClientCommunication remoteClientCommunication;

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

        public RemoteClientsOperator(IControlUnit controlUnit, INetworkSettingProvider networkSettingProvider, IRemoteClientCommunication remoteClientCommunication)
        {
            this.controlUnit = controlUnit;
            this.networkSettingProvider = networkSettingProvider;
            this.remoteClientCommunication = remoteClientCommunication;
            remoteClientCommunication.OnReceiveMessage -= RemoteClientCommunication_OnReceiveMessage;
            remoteClientCommunication.OnReceiveMessage += RemoteClientCommunication_OnReceiveMessage;
            State = OperatorStates.Ready;
        }

        private void RemoteClientCommunication_OnReceiveMessage(object sender, ClientMessageEventArgs e)
        {
            ProcessMessage(e.Client, e.Message);
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

        #endregion

        public void Play()
        {
            var stateBackup = State;
            try {
                var message = RLCMessageFactory.Play(controlUnit.Key);
                remoteClientCommunication.SendToAll(message);
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
                remoteClientCommunication.SendToAll(message);
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
                remoteClientCommunication.SendToAll(message);
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
