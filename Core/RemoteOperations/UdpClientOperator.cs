﻿using RLCCore.Domain;
using System;
using NetworkServer.UDP;
using Core.Messages;
using RLCCore.Settings;
using NLog;
using System.Net;
using NotifiedObjectsFramework;
using RLCCore.RemoteOperations;
using System.Linq;

namespace Core.RemoteOperations
{
    public class UdpClientOperator : NotifyPropertyChangedBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly UdpServer udpServer;
        private readonly RemoteControlProject project;
        private readonly IClientConnectionsController connectionsController;
        private readonly INetworkSettingProvider networkSettingProvider;
        private readonly SequencePlayer player;
        private MessageReceiveConfirmationService confirmationService;

        public UdpClientOperator(
            UdpServer udpServer, 
            RemoteControlProject project,
            IClientConnectionsController connectionsController,
            INetworkSettingProvider networkSettingProvider,
            SequencePlayer player
            )
        {
            this.udpServer = udpServer ?? throw new ArgumentNullException(nameof(udpServer));
            this.project = project ?? throw new ArgumentNullException(nameof(project));
            this.connectionsController = connectionsController ?? throw new ArgumentNullException(nameof(connectionsController));
            this.networkSettingProvider = networkSettingProvider ?? throw new ArgumentNullException(nameof(networkSettingProvider));
            this.player = player ?? throw new ArgumentNullException(nameof(player));

            ConfigureBindings();
        }

        private void ConfigureBindings()
        {
            CreateNotificationBinding()
                .AddProperty(nameof(CanPlay))
                .SetNotifier(player)
                .BindToProperty(x => x.CanPlay)
                .End();
            CreateNotificationBinding()
                .AddProperty(nameof(CanPause))
                .SetNotifier(player)
                .BindToProperty(x => x.CanPause)
                .End();
            CreateNotificationBinding()
                .AddProperty(nameof(CanStop))
                .SetNotifier(player)
                .BindToProperty(x => x.CanStop)
                .End();
        }

        private OperatorStates state;
        public OperatorStates State {
            get => state;
            private set {
                if(SetField(ref state, value)) {
                    OnPropertyChanged(nameof(CanPlay));
                    OnPropertyChanged(nameof(CanPause));
                    OnPropertyChanged(nameof(CanStop));
                }
            }
        }

        #region Control

        public void StartOperator()
        {
            try {
                connectionsController.CreateConnections(project.Clients);
                StartMessageConfirmationService();
                udpServer.Start(networkSettingProvider.GetServerIPAddress(), project.RlcPort);
                udpServer.DataReceived += UdpServer_DataReceived;
                State = OperatorStates.Ready;
            }
            catch(Exception ex) {
                logger.Error(ex, "Не получилось запустить оператор клиентов");
                StopOperator();
            }
        }

        public void StopOperator()
        {
            StopMessageConfirmationService();
            udpServer.Stop();
            connectionsController.ClearConnections();
            State = OperatorStates.Configure;
        }

        private void StartMessageConfirmationService()
        {
            confirmationService = new MessageReceiveConfirmationService(project.Clients, (clientId, message) => Send(clientId, message), 50, 1000);
        }

        private void StopMessageConfirmationService()
        {
            confirmationService.Dispose();
            confirmationService = null;
        }

        #endregion Control

        #region Client restore

        private void CheckFirstTimeConnection(RLCMessage message)
        {
            if(message.MessageType == MessageType.RequestServerIp) {
                RestoreClientPlaying(message.ClientNumber);
            }
        }

        private void RestoreClientPlaying(int clientNumber)
        {
            var message = CreateStateMessage();
            Send(clientNumber, message);
        }

        #endregion Client restore

        #region Play control

        public bool CanPlay => player.CanPlay && new[] { OperatorStates.Ready, OperatorStates.Pause, OperatorStates.Stop }.Contains(State);

        public void Play()
        {
            var stateBackup = State;
            try {
                State = OperatorStates.Play;
                player.Play();
                SendStateToClients();
            }
            catch(Exception ex) {
                logger.Error(ex, $"Не удалось отправить команду {MessageType.State}");
                State = stateBackup;
                throw;
            }
        }

        public bool CanStop => player.CanStop && new[] { OperatorStates.Play, OperatorStates.Pause }.Contains(State);

        public void Stop()
        {
            var stateBackup = State;
            try {
                State = OperatorStates.Stop;
                player.Stop();
                SendStateToClients();
            }
            catch(Exception ex) {
                logger.Error(ex, $"Не удалось отправить команду {MessageType.State}");
                State = stateBackup;
                throw;
            }
        }

        public bool CanPause => player.CanPause && State == OperatorStates.Play;

        public void Pause()
        {
            var stateBackup = State;
            try {
                State = OperatorStates.Pause;
                player.Pause();
                SendStateToClients();
            }
            catch(Exception ex) {
                logger.Error(ex, $"Не удалось отправить команду {MessageType.State}");
                State = stateBackup;
                throw;
            }
        }

        public void PlayFrom(TimeSpan time)
        {
            player.CurrentTime = time;
            Play();
        }

        private void SendStateToClients()
        {
            var message = CreateStateMessage();
            logger.Debug($"Send {message.ClientState} state (MessageId: {message.MessageId}) to all clients");
            SendToAll(message);
        }

        private RLCMessage CreateStateMessage()
        {
            var frame = GetNextFrame();
            var frameStartTime = GetNextFrameStartTime();
            var clientState = GetClientState();
            var message = RLCMessageFactory.State(project.Key, frame, frameStartTime, clientState);
            return message;
        }

        private uint GetNextFrame()
        {
            var currentFrame = (uint)(player.CurrentTime.TotalMilliseconds) / (uint)50;
            return currentFrame + 1;
        }

        private DateTime GetNextFrameStartTime()
        {
            var timeFromCurrentFrame = (uint)(player.CurrentTime.TotalMilliseconds % (double)50);
            var timeUntilNextFrame = 50 - timeFromCurrentFrame;
            return DateTime.Now.AddMilliseconds(timeUntilNextFrame);
        }

        private ClientState GetClientState()
        {
            switch(State) {
                case OperatorStates.Play:
                    return ClientState.Playing;
                case OperatorStates.Pause:
                    return ClientState.Paused;
                default:
                    return ClientState.Stoped;
            }
        }

        #endregion Play control

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
            confirmationService?.Confirm(message);
            Receive(message);
        }

        private void Receive(RLCMessage message)
        {
            if(message.MessageType == MessageType.ClientInfo) {
                return;
            }

            if(message.MessageType == MessageType.RequestServerIp) {
                var response = RLCMessageFactory.SendServerIP(project.Key, networkSettingProvider.GetServerIPAddress());
                Send(message.ClientNumber, response);
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
            if(message.MessageType == MessageType.State) {
                confirmationService?.AddMessageToConfirm(clientNumber, message);
            }
        }

        private void SendToAll(RLCMessage message)
        {
            if(message is null) {
                throw new ArgumentNullException(nameof(message));
            }
            
            udpServer.Send(message.ToArray(), networkSettingProvider.BroadcastIPAddress);
            if(message.MessageType == MessageType.State) {
                confirmationService?.AddMessageToConfirmForAll(message);
            }
        }

        #endregion Sending
    }
}
