﻿using System;
using System.Linq;
using Core;
using RLCCore;
using RLCCore.RemoteOperations;
using RLCServerApplication.Infrastructure;
using RLCServerApplication.Infrastructure.Command;

namespace RLCServerApplication.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public RLCProjectController ProjectController { get; private set; }

        public SequencePlayer Player { get; }

        private SettingsViewModel settingsViewModel;
        public SettingsViewModel SettingsViewModel {
            get => settingsViewModel;
            set => SetField(ref settingsViewModel, value, () => SettingsViewModel);
        }

        private RemoteClientsViewModel remoteClientsViewModel;
        public RemoteClientsViewModel RemoteClientsViewModel {
            get => remoteClientsViewModel;
            set => SetField(ref remoteClientsViewModel, value, () => RemoteClientsViewModel);
        }

        public bool CanEdit => ProjectController.WorkMode == ProjectWorkModes.Setup;

        public MainWindowViewModel()
        {
            ProjectController = new RLCProjectController();
            SettingsViewModel = new SettingsViewModel(ProjectController);
            RemoteClientsViewModel = new RemoteClientsViewModel(ProjectController);
            Player = new SequencePlayer();
            Player.ChannelPositionUserChanged += Player_ChannelPositionUserChanged;
            ConfigureBindings();
            CreateCommands();
        }

        private void Player_ChannelPositionUserChanged(object sender, System.EventArgs e)
        {
            PlayFromTimelineCommand.Execute();
        }

        public void Close()
        {
            ProjectController.Dispose();
        }

        private void ConfigureBindings()
        {
            Bind(() => CanEdit, ProjectController, x => x.WorkMode);

            BindAction(UpdatePlayerState, ProjectController, x => x.WorkMode);            

            BindAction(UpdatePlayerCommands, ProjectController,
                   x => x.ServicesIsReady,
                   x => x.WorkMode
            );

            BindAction(() => {
                if(ProjectController.RemoteClientsOperator != null) {
                    BindAction(UpdatePlayerCommands, ProjectController.RemoteClientsOperator, x => x.State);                    
                    Bind(() => CanSwitchToSetup, ProjectController.RemoteClientsOperator, x => x.State);
                    Bind(() => CanSwitchToTest, ProjectController.RemoteClientsOperator, x => x.State);
                    Bind(() => CanSwitchToWork, ProjectController.RemoteClientsOperator, x => x.State);
                }
            }, ProjectController, x => x.RemoteClientsOperator);

            BindAction(UpdatePlayerCommands, Player,
                x => x.CanPlay,
                x => x.CanPause,
                x => x.CanStop,
                x => x.IsInitialized
            );

            Bind(() => CanSwitchToWork, Player,
                x => x.IsInitialized
            );
        }

        #region Commands

        private void CreateCommands()
        {
            CreatePlayCommand();
            CreatePlayFromCommand();
            CreatePlayFromTimelineCommand();
            CreateStopCommand();
            CreatePauseCommand();
            CreateAddAudioTrackCommand();
            CreateSwitchToSetupCommand();
            CreateSwitchToTestCommand();
            CreateSwitchToWorkCommand();
        }


        #region PlayCommand

        public DelegateCommand PlayCommand { get; private set; }

        private void CreatePlayCommand()
        {
            PlayCommand = new DelegateCommand(
                () => {
                    if(ProjectController.WorkMode == ProjectWorkModes.Setup) {
                        return;
                    }
                    ProjectController.RemoteClientsOperator.Play();
                    if(!Player.IsInitialized || !Player.CanPlay) {
                        return;
                    }
                    Player.Play();
                },
                () => {
                    if(!ProjectController.ServicesIsReady) {
                        return false;
                    }
                    var playStates = new[]{ OperatorStates.Ready, OperatorStates.Stop, OperatorStates.Pause };
                    return ProjectController.WorkMode != ProjectWorkModes.Setup && playStates.Contains(ProjectController.RemoteClientsOperator.State);
                }
            );
        }

        #endregion PlayCommand

        #region PlayFromButtonCommand

        private TimeSpan playFromTime;
        public TimeSpan PlayFromTime {
            get => playFromTime;
            set => SetField(ref playFromTime, value, () => PlayFromTime);
        }

        public DelegateCommand PlayFromButtonCommand { get; private set; }

        private void CreatePlayFromCommand()
        {
            PlayFromButtonCommand = new DelegateCommand(
                () => {
                    if(ProjectController.WorkMode == ProjectWorkModes.Setup) {
                        return;
                    }
                    ProjectController.RemoteClientsOperator.PlayFrom(PlayFromTime);
                    if(!Player.IsInitialized || !Player.CanPlay) {
                        return;
                    }
                    Player.ChannelPosition = PlayFromTime.TotalSeconds;
                    Player.Play();
                },
                () => {
                    if(!ProjectController.ServicesIsReady) {
                        return false;
                    }
                    var playStates = new[]{ OperatorStates.Ready, OperatorStates.Stop, OperatorStates.Pause };
                    return ProjectController.WorkMode == ProjectWorkModes.Test 
                        && playStates.Contains(ProjectController.RemoteClientsOperator.State)
                        && PlayFromTime.TotalMilliseconds > 0;
                }
            );
            PlayFromButtonCommand.CanExecuteChangedWith(this, x => x.PlayFromTime);
        }

        #endregion PlayFromButtonCommand

        #region PlayFromTimelineCommand

        public DelegateCommand PlayFromTimelineCommand { get; private set; }

        private void CreatePlayFromTimelineCommand()
        {
            PlayFromTimelineCommand = new DelegateCommand(
                () => {
                    if(ProjectController.WorkMode == ProjectWorkModes.Setup) {
                        return;
                    }
                    ProjectController.RemoteClientsOperator.PlayFrom(Player.CurrentTime);
                    if(!Player.IsInitialized || !Player.CanPlay) {
                        return;
                    }
                    Player.Play();
                },
                () => {
                    if(!ProjectController.ServicesIsReady) {
                        return false;
                    }
                    return ProjectController.WorkMode == ProjectWorkModes.Test
                        && Player.IsEnabled;
                }
            );
        }

        #endregion PlayFromTimelineCommand

        #region StopCommand

        public DelegateCommand StopCommand { get; private set; }

        private void CreateStopCommand()
        {
            StopCommand = new DelegateCommand(
                () => {
                    if(ProjectController.WorkMode == ProjectWorkModes.Setup) {
                        return;
                    }
                    ProjectController.RemoteClientsOperator.Stop();
                    if(!Player.IsInitialized || !Player.CanStop) {
                        return;
                    }
                    Player.Stop();
                },
                () => {
                    if(!ProjectController.ServicesIsReady) {
                        return false;
                    }
                    var stopStates = new[]{ OperatorStates.Play, OperatorStates.Pause };
                    return ProjectController.WorkMode != ProjectWorkModes.Setup && stopStates.Contains(ProjectController.RemoteClientsOperator.State);
                }
            );
        }


        #endregion StopCommand

        #region PauseCommand

        public DelegateCommand PauseCommand { get; private set; }

        private void CreatePauseCommand()
        {
            PauseCommand = new DelegateCommand(
                () => {
                    if(ProjectController.WorkMode == ProjectWorkModes.Setup) {
                        return;
                    }
                    ProjectController.RemoteClientsOperator.Pause();
                    if(!Player.IsInitialized || !Player.CanPause) {
                        return;
                    }
                    Player.Pause();
                },
                () => {
                    if(!ProjectController.ServicesIsReady) {
                        return false;
                    }
                    return ProjectController.WorkMode != ProjectWorkModes.Setup && ProjectController.RemoteClientsOperator.State == OperatorStates.Play;
                }
            );
        }

        #endregion PauseCommand

        #region AddAudioTrackCommand

        public DelegateCommand AddAudioTrackCommand { get; private set; }

        private void CreateAddAudioTrackCommand()
        {
            AddAudioTrackCommand = new DelegateCommand(
                () => {
                    //FIXME убрать зависимоть от диалога
                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                    dlg.DefaultExt = ".mp3";
                    dlg.Filter = "Mp3 files|*.mp3";
                    if(dlg.ShowDialog() == true) {
                        Player.OpenFile(dlg.FileName);
                        Player.Volume = 0.1f;
                    }
                },
                () => CanEdit
            );
            AddAudioTrackCommand.CanExecuteChangedWith(this, x => x.CanEdit);
        }

        #endregion AddAudioTrackCommand

        #region SwitchToSetupCommand

        public bool IsSetupMode => ProjectController.WorkMode == ProjectWorkModes.Setup;

        public DelegateCommand SwitchToSetupCommand { get; private set; }

        private void CreateSwitchToSetupCommand()
        {
            SwitchToSetupCommand = new DelegateCommand(
                () => {
                    try {
                        Player.Stop();
                        ProjectController.SwitchToSetupMode();
                    }
                    finally {
                        UpdateWorkModeProperties();
                    }                    
                },
                () => CanSwitchToSetup
            );
            SwitchToSetupCommand.CanExecuteChangedWith(this, x => x.CanSwitchToSetup);
        }

        public bool CanSwitchToSetup {
            get {
                if(ProjectController.RemoteClientsOperator == null) {
                    return true;
                }
                return (ProjectController.RemoteClientsOperator.State != OperatorStates.Play
                    && ProjectController.RemoteClientsOperator.State != OperatorStates.Pause
                );
            }
        }

        #endregion SwitchToSetupCommand

        #region SwitchToTestCommand

        public bool IsTestMode => ProjectController.WorkMode == ProjectWorkModes.Test;

        public DelegateCommand SwitchToTestCommand { get; private set; }

        private void CreateSwitchToTestCommand()
        {
            SwitchToTestCommand = new DelegateCommand(
                () => {
                    try {
                        ProjectController.SwitchToTestMode();
                    }
                    finally {
                        UpdateWorkModeProperties();
                    }
                },
                () => CanSwitchToTest
            );
            SwitchToTestCommand.CanExecuteChangedWith(this, x => x.CanSwitchToTest);
        }

        public bool CanSwitchToTest {
            get {
                if(ProjectController.RemoteClientsOperator == null) {
                    return true;
                }
                return (ProjectController.RemoteClientsOperator.State != OperatorStates.Play
                    && ProjectController.RemoteClientsOperator.State != OperatorStates.Pause
                );
            }
        }

        #endregion SwitchToTestCommand

        #region SwitchToWorkCommand

        public bool IsWorkMode => ProjectController.WorkMode == ProjectWorkModes.Work;

        public DelegateCommand SwitchToWorkCommand { get; private set; }

        private void CreateSwitchToWorkCommand()
        {
            SwitchToWorkCommand = new DelegateCommand(
                () => {                    
                    try {
                        ProjectController.SwitchToWorkMode();
                    }
                    finally {
                        UpdateWorkModeProperties();
                    }
                },
                () => CanSwitchToWork
            );

            SwitchToWorkCommand.CanExecuteChangedWith(this, x => x.CanSwitchToWork);
        }

        public bool CanSwitchToWork {
            get {
                if(ProjectController.RemoteClientsOperator == null) {
                    return Player.IsInitialized;
                }
                return Player.IsInitialized
                    && ProjectController.RemoteClientsOperator.State != OperatorStates.Play
                    && ProjectController.RemoteClientsOperator.State != OperatorStates.Pause;
            }
        }
        #endregion SwitchToWorkCommand

        #endregion Commands

        private void UpdateWorkModeProperties()
        {
            OnPropertyChanged(() => IsSetupMode);
            OnPropertyChanged(() => IsTestMode);
            OnPropertyChanged(() => IsWorkMode);
        }

        private void UpdatePlayerCommands()
        {
            PlayCommand?.RaiseCanExecuteChanged();
            PlayFromButtonCommand?.RaiseCanExecuteChanged();
            StopCommand?.RaiseCanExecuteChanged();
            PauseCommand?.RaiseCanExecuteChanged();
        }

        private void UpdatePlayerState()
        {
            Player.IsEnabled = ProjectController.WorkMode == ProjectWorkModes.Test;
        }
    }
}
