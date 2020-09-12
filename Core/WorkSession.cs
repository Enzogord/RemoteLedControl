using Core.IO;
using Core.RemoteOperations;
using NLog;
using NotifiedObjectsFramework;
using RLCCore.Domain;
using RLCCore.Settings;
using System;
using System.IO;

namespace Core
{
    public sealed class WorkSession : NotifyPropertyChangedBase
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        private readonly SntpServer sntpServer;
        private readonly SaveController saveController;
        private readonly NetworkController networkController;

        public RemoteControlProject Project { get; private set; }
        public UdpClientOperator ClientOperator { get; private set; }

        private SessionState state;
        public SessionState State {
            get => state;
            private set {
                SetField(ref state, value);
                OnPropertyChanged(nameof(CanSwitchToSetupMode));
                OnPropertyChanged(nameof(CanSwitchToTestMode));
                OnPropertyChanged(nameof(CanSwitchToWorkMode));
            }
        }

        public SequencePlayer Player { get; private set; }

        public WorkSession(
            RemoteControlProject project,
            SequencePlayer player,
            UdpClientOperator clientOperator,
            SntpServer sntpServer,
            SaveController saveController,
            NetworkController networkController)
        {
            Project = project ?? throw new ArgumentNullException(nameof(project));
            Player = player ?? throw new ArgumentNullException(nameof(player));
            ClientOperator = clientOperator ?? throw new ArgumentNullException(nameof(clientOperator));
            this.sntpServer = sntpServer ?? throw new ArgumentNullException(nameof(sntpServer));
            this.saveController = saveController ?? throw new ArgumentNullException(nameof(saveController));
            this.networkController = networkController ?? throw new ArgumentNullException(nameof(networkController));
            Player.PropertyChanged += Player_PropertyChanged;
            Player.ChannelPositionUserChanged += Player_ChannelPositionUserChanged;
            OpenAudioFile(Project.SoundtrackFileName);
        }

        private void Player_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName) {
                case nameof(Player.IsInitialized):
                    OnPropertyChanged(nameof(CanSwitchToWorkMode));
                    break;
                default:
                    break;
            }
        }

        private void Player_ChannelPositionUserChanged(object sender, System.EventArgs e)
        {
            if(State != SessionState.Test) {
                return;
            }
            ClientOperator.PlayFrom();
        }

        public void SetAudioFile(string filePath)
        {
            string oldFileName = Project.SoundtrackFileName;
            saveController.UpdateSoundTrackFile(oldFileName, filePath);
            Project.SoundtrackFileName = Path.GetFileName(filePath);
            OpenAudioFile(Project.SoundtrackFileName);
        }

        private void OpenAudioFile(string fileName)
        {
            if(string.IsNullOrWhiteSpace(fileName)) {
                return;
            }

            string filePath = Path.Combine(saveController.WorkDirectory, fileName);
            using(var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
                Player.Open(fileStream);
            }
        }

        public void SetNewTime(TimeSpan time)
        {
            Player.CurrentTime = time;
        }

        #region Modes

        private bool servicesIsReady;

        public bool ServicesIsReady {
            get => servicesIsReady;
            set => SetField(ref servicesIsReady, value);
        }

        public bool CanSwitchToSetupMode => State != SessionState.Setup;

        public void SwitchToSetupMode()
        {
            if(State == SessionState.Setup) {
                return;
            }
            StopServices();
            State = SessionState.Setup;
            Player.IsEnabled = false;
        }

        public bool CanSwitchToTestMode => State == SessionState.Setup;

        public void SwitchToTestMode()
        {
            if(State == SessionState.Test) {
                return;
            }
            StartServices();
            State = SessionState.Test;
            Player.IsEnabled = true;
        }

        public bool CanSwitchToWorkMode => State == SessionState.Setup && Player.IsInitialized;

        public void SwitchToWorkMode()
        {
            if(State == SessionState.Work) {
                return;
            }
            StartServices();
            State = SessionState.Work;
            Player.IsEnabled = false;
        }

        #endregion Modes

        #region Services

        public void StartServices()
        {
            if(ServicesIsReady) {
                return;
            }

            try {
                StartSNTP();
                StartUDP();
            }
            catch(Exception) {
                StopServices();
                throw;
            }

            ServicesIsReady = true;
        }

        private void StartSNTP()
        {
            try {
                sntpServer.Start();
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка при запуске службы синхронизации времени");
                throw;
            }
        }

        private void StartUDP()
        {
            try {
                ClientOperator.StartOperator();
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка при запуске Udp сервера");
                throw;
            }
        }

        public void StopServices()
        {
            ClientOperator.StopOperator();
            sntpServer.Stop();

            ServicesIsReady = false;
        }


        #endregion Services

    }
}
