using System;
using System.Collections.Generic;
using System.Linq;
using Core.ClientConnectionService;
using Core.Messages;
using Core.RemoteOperations;
using NLog;
using NotifiedObjectsFramework;
using RLCCore.Domain;
using RLCCore.Settings;

namespace RLCCore.RemoteOperations
{
    public class RemoteClientsOperator : NotifyPropertyChangedBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly uint key;
        private readonly IEnumerable<RemoteClient> clients;
        private readonly IMessageReceiver messageReceiver;
        private readonly INetworkSettingProvider networkSettingProvider;
        private readonly IRemoteClientCommunication remoteClientCommunication;


        private OperatorStates state;
        public OperatorStates State {
            get => state;
            private set => SetField(ref state, value, () => State);
        }

        private ISequenceTimeProvider timeProvider;
        public ISequenceTimeProvider TimeProvider {
            get => timeProvider;
            set => SetField(ref timeProvider, value, () => TimeProvider);
        }


        public RemoteClientsOperator(uint key, IEnumerable<RemoteClient> clients, IMessageReceiver messageReceiver, INetworkSettingProvider networkSettingProvider, IRemoteClientCommunication remoteClientCommunication)
        {
            this.key = key;
            this.messageReceiver = messageReceiver;
            this.networkSettingProvider = networkSettingProvider;
            this.remoteClientCommunication = remoteClientCommunication;
            this.clients = clients ?? throw new ArgumentNullException(nameof(clients));
            remoteClientCommunication.OnReceiveMessage -= RemoteClientCommunication_OnReceiveMessage;
            remoteClientCommunication.OnReceiveMessage += RemoteClientCommunication_OnReceiveMessage;
            remoteClientCommunication.OnClientAuthorized -= RemoteClientCommunication_OnClientAuthorized;
            remoteClientCommunication.OnClientAuthorized += RemoteClientCommunication_OnClientAuthorized;
            State = OperatorStates.Ready;
        }

        private void RemoteClientCommunication_OnClientAuthorized(object sender, INumeredClient e)
        {
            RemoteClient foundClient = clients.FirstOrDefault(x => x.Number == e.Number);
            if(foundClient != null){
                RestoreClientPlaying(foundClient);
            }
        }

        private void RestoreClientPlaying(RemoteClient client)
        {
            if(TimeProvider == null) {
                return;
            }
            if(State == OperatorStates.Play || State == OperatorStates.Pause) {
                ClientState clientState;
                switch(State) {
                    case OperatorStates.Play:
                        clientState = ClientState.Playing;
                        break;
                    case OperatorStates.Pause:
                        clientState = ClientState.Paused;
                        break;
                    default:
                        return;
                }
                remoteClientCommunication.Send(client, RLCMessageFactory.Rewind(key, TimeProvider.CurrentTime, clientState));
            }
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

            messageReceiver.Receive(message);
        }

        public void Play()
        {
            var stateBackup = State;
            try {
                var message = RLCMessageFactory.Play(key);
                remoteClientCommunication.SendToAll(message);
                State = OperatorStates.Play;
            }
            catch(Exception ex) {
                logger.Error(ex, $"Не удалось отправить команду {MessageType.Play}");
                //восстановление состояния
                State = stateBackup;
                throw;
            }
        }

        public void Stop()
        {
            var stateBackup = State;
            try {
                var message = RLCMessageFactory.Stop(key);
                remoteClientCommunication.SendToAll(message);
                State = OperatorStates.Stop;
            }
            catch(Exception ex) {
                logger.Error(ex, $"Не удалось отправить команду {MessageType.Stop}");
                //восстановление состояния
                State = stateBackup;
                throw;
            }
        }

        public void Pause()
        {
            var stateBackup = State;
            try {
                var message = RLCMessageFactory.Pause(key);
                remoteClientCommunication.SendToAll(message);
                State = OperatorStates.Pause;
            }
            catch(Exception ex) {
                logger.Error(ex, $"Не удалось отправить команду {MessageType.Pause}");
                //восстановление состояния
                State = stateBackup;
                throw;
            }
        }

        public void PlayFrom(TimeSpan time)
        {
            var stateBackup = State;
            try {
                var message = RLCMessageFactory.PlayFrom(key, time);
                remoteClientCommunication.SendToAll(message);
                State = OperatorStates.Play;
            }
            catch(Exception ex) {
                logger.Error(ex, $"Не удалось отправить команду {MessageType.PlayFrom}");
                //восстановление состояния
                State = stateBackup;
                throw;
            }
        }
    }
}
