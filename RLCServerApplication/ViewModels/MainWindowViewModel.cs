﻿using Core;
using Core.IO;
using RLCCore;
using RLCCore.RemoteOperations;
using RLCServerApplication.Infrastructure;
using RLCServerApplication.Infrastructure.Command;
using System;
using System.IO;
using System.Linq;
using System.Windows;

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

        public MainWindowViewModel(RLCProjectController projectController, SequencePlayer player, RemovableDrivesProvider removableDrivesProvider)
        {
            ProjectController = projectController ?? throw new ArgumentNullException(nameof(projectController));
            Player = player ?? throw new ArgumentNullException(nameof(player));
            this.removableDrivesProvider = removableDrivesProvider ?? throw new ArgumentNullException(nameof(removableDrivesProvider));
            SettingsViewModel = new SettingsViewModel(ProjectController);
            RemoteClientsViewModel = new RemoteClientsViewModel(ProjectController, removableDrivesProvider);
            ProjectController.TimeProvider = Player;
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
            CreateNotificationBinding()
                .AddProperty(nameof(CanEdit))
                .AddAction(UpdatePlayerState)
                .SetNotifier(ProjectController)
                .BindToProperty(x => x.WorkMode)
                .End();

            CreateNotificationBinding()
                .AddAction(UpdateProjectCommands)
                .AddAction(UpdatePlayerCommands)
                .SetNotifier(ProjectController)
                .BindToProperty(x => x.ServicesIsReady)
                .BindToProperty(x => x.WorkMode)
                .End();

            CreateNotificationBinding()
                .AddAction(() => {
                    if(ProjectController.RemoteClientsOperator != null) {
                        CreateNotificationBinding()
                            .AddAction(UpdateProjectCommands)
                            .AddAction(UpdatePlayerCommands)
                            .AddProperty(nameof(CanSwitchToSetup), nameof(CanSwitchToTest), nameof(CanSwitchToWork))
                            .SetNotifier(ProjectController.RemoteClientsOperator)
                            .BindToProperty(x => x.State)
                            .End();
                    }
                })
                .SetNotifier(ProjectController)
                .BindToProperty(x => x.RemoteClientsOperator)
                .End();

            CreateNotificationBinding()
                .AddAction(() => {
                    if(ProjectController.CurrentProject != null) {
                        CreateNotificationBinding()
                            .AddAction(ReloadAudioTrack)
                            .SetNotifier(ProjectController.CurrentProject)
                            .BindToProperty(x => x.SoundtrackFileName)
                            .End();
                    }
                })
                .SetNotifier(ProjectController)
                .BindToProperty(x => x.CurrentProject)
                .End();

            CreateNotificationBinding().AddAction(() => RemoteClientsViewModel = new RemoteClientsViewModel(ProjectController, removableDrivesProvider))
                .SetNotifier(ProjectController)
                .BindToProperty(x => x.CurrentProject)
                .End();

            CreateNotificationBinding().AddAction(UpdatePlayerCommands)
                .SetNotifier(Player)
                .BindToProperty(x => x.CanPlay)
                .BindToProperty(x => x.CanPause)
                .BindToProperty(x => x.CanStop)
                .BindToProperty(x => x.IsInitialized)
                .End();

            CreateNotificationBinding().AddProperty(nameof(CanSwitchToWork))
                .SetNotifier(Player)
                .BindToProperty(x => x.IsInitialized)
                .End();
        }

        #region Commands

        private void CreateCommands()
        {
            CreatePlayCommand();
            CreatePlayFromCommand();
            CreatePlayFromTimelineCommand();
            CreateStopCommand();
            CreatePauseCommand();
            CreateSwitchToSetupCommand();
            CreateSwitchToTestCommand();
            CreateSwitchToWorkCommand();
            CreateLoadCommand();
            CreateSaveCommand();
            CreateSaveAsCommand();
            CreateMuteCommand();
            CreateCreateCommand();
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
                    var playStates = new[] { OperatorStates.Ready, OperatorStates.Stop, OperatorStates.Pause };
                    return ProjectController.WorkMode != ProjectWorkModes.Setup && playStates.Contains(ProjectController.RemoteClientsOperator.State);
                }
            );
        }

        #endregion PlayCommand

        #region PlayFromButtonCommand

        private TimeSpan playFromTime;
        private readonly RemovableDrivesProvider removableDrivesProvider;

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
                    var playStates = new[] { OperatorStates.Ready, OperatorStates.Stop, OperatorStates.Pause };
                    return ProjectController.WorkMode == ProjectWorkModes.Test
                        && playStates.Contains(ProjectController.RemoteClientsOperator.State)
                        && PlayFromTime.TotalMilliseconds > 0;
                }
            );
            PlayFromButtonCommand.CanExecuteChangedWith(this, x => x.PlayFromTime);
        }

        #endregion PlayFromButtonCommand

        #region MuteCommand

        public DelegateCommand MuteCommand { get; private set; }

        private void CreateMuteCommand()
        {
            MuteCommand = new DelegateCommand(
                () => Player.Volume = 0f,
                () => Player != null
            );
            MuteCommand.CanExecuteChangedWith(this, x => x.Player);
        }

        #endregion MuteCommand

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
                    var stopStates = new[] { OperatorStates.Play, OperatorStates.Pause };
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

        #region CreateCommand

        public DelegateCommand CreateCommand { get; private set; }

        private void CreateCreateCommand()
        {
            CreateCommand = new DelegateCommand(
                () => {
                    if(ProjectController.CurrentProject != null) {
                        if(MessageBox.Show("Уже есть открытый проект, не сохраненные изменения будут утеряны, все равно открыть новый?", "Внимание!" , MessageBoxButton.YesNo) == MessageBoxResult.No) {
                            return;
                        }
                    }

                    ProjectController.CreateProject();
                },
                () => ProjectController.CanCreateNewProject
            );
            CreateCommand.CanExecuteChangedWith(ProjectController, x => x.CanCreateNewProject);
        }

        #endregion CreateCommand	

        #region SaveCommand

        public DelegateCommand SaveCommand { get; private set; }

        private void CreateSaveCommand()
        {
            SaveCommand = new DelegateCommand(
                () => {
                    if(!ProjectController.NeedSelectSavePath) {
                        ProjectController.SaveProject();
                        return;
                    }
                    SaveAsCommand.Execute();
                },
                () => ProjectController.WorkMode == ProjectWorkModes.Setup && ProjectController.CurrentProject != null
            );
            SaveCommand.CanExecuteChangedWith(ProjectController, x => x.WorkMode);
            SaveCommand.CanExecuteChangedWith(ProjectController, x => x.CurrentProject);
        }

        #endregion SaveCommand

        #region SaveAsCommand

        public DelegateCommand SaveAsCommand { get; private set; }

        private void CreateSaveAsCommand()
        {
            SaveAsCommand = new DelegateCommand(
                () => {
                    //FIXME убрать зависимоть от диалога
                    
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.DefaultExt = ".rlcsave";
                    dlg.Filter = "RemoteLedControl save file|*.rlcsave";
                    dlg.CreatePrompt = true;
                    if(dlg.ShowDialog() == true) {
                        //Stream сохраняется, не закрывать
                        ProjectController.SaveProjectAs(new FileStream(dlg.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read));
                    }                    
                },
                () => ProjectController.WorkMode == ProjectWorkModes.Setup && ProjectController.CurrentProject != null
            );
            SaveAsCommand.CanExecuteChangedWith(ProjectController, x => x.WorkMode);
            SaveAsCommand.CanExecuteChangedWith(ProjectController, x => x.CurrentProject);
        }

        #endregion SaveAsCommand

        #region LoadCommand

        public DelegateCommand LoadCommand { get; private set; }

        private void CreateLoadCommand()
        {
            LoadCommand = new DelegateCommand(
                () => {
                    //FIXME убрать зависимоть от диалога
                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                    dlg.InitialDirectory = Environment.CurrentDirectory;
                    dlg.DefaultExt = ".rlcsave";
                    dlg.Filter = "RemoteLedControl save file|*.rlcsave";
                    dlg.CheckFileExists = true;
                    dlg.CheckPathExists = true;
                    dlg.Multiselect = false;
                    if(dlg.ShowDialog() == true) {
                        //Stream сохраняется, не закрывать
                        ProjectController.LoadProject(new FileStream(dlg.FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read));
                        ReloadAudioTrack();
                    }
                },
                () => ProjectController.WorkMode == ProjectWorkModes.Setup
            );
            LoadCommand.CanExecuteChangedWith(ProjectController, x => x.WorkMode);
        }

        #endregion LoadCommand	

        #endregion Commands

        private void ReloadAudioTrack()
        {
            if(ProjectController.CurrentProject == null || Player == null || ProjectController.WorkMode != ProjectWorkModes.Setup || string.IsNullOrWhiteSpace(ProjectController.CurrentProject.SoundtrackFileName)) {
                return;
            }
            string soundtrackFilePath = Path.Combine(ProjectController.SaveController.WorkDirectory, ProjectController.CurrentProject.SoundtrackFileName);
            if(!File.Exists(soundtrackFilePath)) {
                return;
            }
            Player.OpenFile(soundtrackFilePath);
            Player.Volume = 0.1f;
        }

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

        private void UpdateProjectCommands()
        {
            LoadCommand?.RaiseCanExecuteChanged();
            SaveCommand?.RaiseCanExecuteChanged();
        }

        private void UpdatePlayerState()
        {
            Player.IsEnabled = ProjectController.WorkMode == ProjectWorkModes.Test;
        }
    }
}
