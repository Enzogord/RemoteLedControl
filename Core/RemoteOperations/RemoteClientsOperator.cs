using System;
using Core.ClientConnectionService;
using Core.Messages;
using Core.RemoteOperations;
using NLog;
using RLCCore.Settings;
using Service;

namespace RLCCore.RemoteOperations
{
    public class RemoteClientsOperator : NotifyPropertyBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly uint key;
        private readonly IMessageReceiver messageReceiver;
        private readonly INetworkSettingProvider networkSettingProvider;
        private readonly IRemoteClientCommunication remoteClientCommunication;


        private OperatorStates state;
        public OperatorStates State {
            get => state;
            private set => SetField(ref state, value, () => State);
        }

        public RemoteClientsOperator(uint key, IMessageReceiver messageReceiver, INetworkSettingProvider networkSettingProvider, IRemoteClientCommunication remoteClientCommunication)
        {
            this.key = key;
            this.messageReceiver = messageReceiver;
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

            messageReceiver.Receive(message);
        }

        #region private methods

        #endregion

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
