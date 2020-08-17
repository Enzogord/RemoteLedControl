using RLCCore.Domain;
using System;
using NetworkServer.UDP;
using Core.Messages;
using RLCCore.Settings;
using NLog;
using System.Net;
using NotifiedObjectsFramework;
using RLCCore.RemoteOperations;
using System.Threading.Tasks;

namespace Core.RemoteOperations
{
    public class UdpClientOperator : NotifyPropertyChangedBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UdpServer udpServer;
        private readonly RemoteControlProject project;
        private readonly IClientConnectionsController connectionsController;
        private readonly INetworkSettingProvider networkSettingProvider;
        private readonly ISequenceTimeProvider sequenceTimeProvider;

        public UdpClientOperator(
            UdpServer udpServer, 
            RemoteControlProject project,
            IClientConnectionsController connectionsController,
            INetworkSettingProvider networkSettingProvider,
            ISequenceTimeProvider sequenceTimeProvider
            )
        {
            this.udpServer = udpServer ?? throw new ArgumentNullException(nameof(udpServer));
            this.project = project ?? throw new ArgumentNullException(nameof(project));
            this.connectionsController = connectionsController ?? throw new ArgumentNullException(nameof(connectionsController));
            this.networkSettingProvider = networkSettingProvider ?? throw new ArgumentNullException(nameof(networkSettingProvider));
            this.sequenceTimeProvider = sequenceTimeProvider ?? throw new ArgumentNullException(nameof(sequenceTimeProvider));
            udpServer.DataReceived += UdpServer_DataReceived;
            udpServer.StatusChanged += UdpServer_StatusChanged;
        }

        private void UdpServer_StatusChanged(object sender, EventArgs e)
        {
            if(udpServer.IsActive) {
                State = OperatorStates.Ready;
            }
            else {
                State = OperatorStates.Configure;
            }
        }

        private OperatorStates state;
        public OperatorStates State {
            get => state;
            private set => SetField(ref state, value);
        }

        #region Restore

        private void CheckFirstTimeConnection(RLCMessage message)
        {
            if(State != OperatorStates.Play && State != OperatorStates.Pause) {
                return;
            }

            if(message.MessageType == MessageType.RequestServerIp) {
                Task.Run(() => {
                    RestoreClientPlaying(message.ClientNumber);
                });
            }
        }

        private void RestoreClientPlaying(int clientNumber)
        {
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
            var message = RLCMessageFactory.Rewind(project.Key, sequenceTimeProvider.CurrentTime, clientState);
            Send(clientNumber, message);
        }

        #endregion Restore

        #region Control

        public void Play()
        {
            if(State == OperatorStates.Pause) {
                var stateBackup = State;
                try {
                    var message = RLCMessageFactory.Play(project.Key);
                    SendToAll(message);
                    State = OperatorStates.Play;
                }
                catch(Exception ex) {
                    logger.Error(ex, $"Не удалось отправить команду {MessageType.Play}");
                    State = stateBackup;
                    throw;
                }
                return;
            }
            PlayFrom(TimeSpan.FromMilliseconds(0));
        }

        public void Stop()
        {
            var stateBackup = State;
            try {
                var message = RLCMessageFactory.Stop(project.Key);
                SendToAll(message);
                State = OperatorStates.Stop;
            }
            catch(Exception ex) {
                logger.Error(ex, $"Не удалось отправить команду {MessageType.Stop}");
                State = stateBackup;
                throw;
            }
        }

        public void Pause()
        {
            var stateBackup = State;
            try {
                var message = RLCMessageFactory.Pause(project.Key);
                SendToAll(message);
                State = OperatorStates.Pause;
            }
            catch(Exception ex) {
                logger.Error(ex, $"Не удалось отправить команду {MessageType.Pause}");
                State = stateBackup;
                throw;
            }
        }

        public void PlayFrom(TimeSpan time)
        {
            var stateBackup = State;
            try {
                var message = RLCMessageFactory.PlayFrom(project.Key, time);
                SendToAll(message);
                State = OperatorStates.Play;
            }
            catch(Exception ex) {
                logger.Error(ex, $"Не удалось отправить команду {MessageType.PlayFrom}");
                State = stateBackup;
                throw;
            }
        }

        #endregion Control

        #region Receiving

        private bool ValidateMessage(RLCMessage message)
        {
            if(message.SourceType != SourceType.Client) {
                logger.Info($"Получено сообщение не от клиента (Тип источника: {message.SourceType})." +
                    $"Сообщение будет проигнорировано");
                return false;
            }

            if(message.Key != project.Key) {
                logger.Info($"Получено сообщение от клиента ({message.ClientNumber}) настроенного на другой проект ({message.Key})." +
                    $"Сообщение будет проигнорировано");
                return false;
            }

            if(!connectionsController.ContainsClient(message.ClientNumber)) {
                logger.Info($"Получено сообщение от клиента ({message.ClientNumber}) не добавленного в текущий проект." +
                    $"Сообщение будет проигнорировано");
                return false;
            }

            return true;
        }

        private bool TryParseMessage(byte[] messageData, out RLCMessage message)
        {
            message = new RLCMessage();
            try {
                message.FromBytes(messageData);
                return true;
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка при распознавании сообщения");
                return false;
            }
        }

        private void UdpServer_DataReceived(object sender, NetworkServer.ReceivedDataEventArgs e)
        {
            if(!TryParseMessage(e.Data, out RLCMessage message)) {
                return;
            }
            if(!ValidateMessage(message)) {
                return;
            }
            CheckFirstTimeConnection(message);
            connectionsController.UpdateClientActivity(message.ClientNumber, e.RemoteEndPoint as IPEndPoint);
            Receive(message);
        }

        private void Receive(RLCMessage message)
        {
            if(message.MessageType == MessageType.ClientInfo) {
                return;
            }
            project.Receive(message);
        }

        #endregion Receiving

        #region Sending

        private void Send(RemoteClient client, RLCMessage message)
        {
            if(client is null) {
                throw new ArgumentNullException(nameof(client));
            }
            Send(client.Number, message);
        }

        private void Send(int clientNumber, RLCMessage message)
        {
            if(message is null) {
                throw new ArgumentNullException(nameof(message));
            }

            var connection = connectionsController.GetClientConnection(clientNumber);
            if(connection == null) {
                throw new InvalidOperationException("Client was not been added to the clients list");
            }

            if(connection.EndPoint == null) {
                throw new InvalidOperationException("Client was not connected");
            }

            udpServer.Send(message.ToArray(), connection.EndPoint);
        }

        private void SendToAll(RLCMessage message)
        {
            if(message is null) {
                throw new ArgumentNullException(nameof(message));
            }

            udpServer.Send(message.ToArray(), networkSettingProvider.BroadcastIPAddress);
        }

        #endregion Sending
    }
}
