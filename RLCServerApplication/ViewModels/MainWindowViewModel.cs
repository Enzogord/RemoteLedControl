using System.Linq;
using Core;
using RLCCore;
using RLCCore.RemoteOperations;
using RLCServerApplication.Infrastructure;

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

            ConfigureBindings();
            CreateCommands();
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

            BindAction(UpdatePlayerCommands, Player,
                x => x.CanPlay,
                x => x.CanPause,
                x => x.CanStop,
                x => x.IsInitialized
            );
        }

        #region Commands

        public RelayCommand PlayCommand { get; private set; }
        public RelayCommand StopCommand { get; private set; }
        public RelayCommand PauseCommand { get; private set; }
        public RelayCommand AddAudioTrackCommand { get; private set; }
        public RelayCommand SwitchToSetupCommand { get; private set; }
        public RelayCommand SwitchToTestCommand { get; private set; }
        public RelayCommand SwitchToWorkCommand { get; private set; }

        private void CreateCommands()
        {
            CreatePlayCommand();
            CreateStopCommand();
            CreatePauseCommand();
            CreateAddAudioTrackCommand();
            CreateSwitchToSetupCommand();
            CreateSwitchToTestCommand();
            CreateSwitchToWorkCommand();
        }

        private void CreatePlayCommand()
        {
            PlayCommand = new RelayCommand(
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

        private void CreateStopCommand()
        {
            StopCommand = new RelayCommand(
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

        private void CreatePauseCommand()
        {
            PauseCommand = new RelayCommand(
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
        
        private void CreateAddAudioTrackCommand()
        {
            AddAudioTrackCommand = new RelayCommand(
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
        }

        private void CreateSwitchToSetupCommand()
        {
            SwitchToSetupCommand = new RelayCommand(
                () => {
                    Player.Stop();
                    ProjectController.SwitchToSetupMode();
                },
                () => true
            );
        }

        private void CreateSwitchToTestCommand()
        {
            SwitchToTestCommand = new RelayCommand(
                () => {
                    ProjectController.SwitchToTestMode();
                    ProjectController.RemoteClientsOperator.StateChanged -= RemoteClientsOperator_StateChanged;
                    ProjectController.RemoteClientsOperator.StateChanged += RemoteClientsOperator_StateChanged;
                },
                () => true
            );
        }

        private void CreateSwitchToWorkCommand()
        {
            SwitchToWorkCommand = new RelayCommand(
                () => {
                    ProjectController.SwitchToWorkMode();
                    ProjectController.RemoteClientsOperator.StateChanged -= RemoteClientsOperator_StateChanged;
                    ProjectController.RemoteClientsOperator.StateChanged += RemoteClientsOperator_StateChanged;
                },
                () => Player.IsInitialized
            );
        }

        private void RemoteClientsOperator_StateChanged(object sender, OperatorStateEventArgs e)
        {
            UpdatePlayerCommands();
        }

        #endregion Commands

        private void UpdatePlayerCommands()
        {
            PlayCommand?.RaiseCanExecuteChanged();
            StopCommand?.RaiseCanExecuteChanged();
            PauseCommand?.RaiseCanExecuteChanged();
        }

        private void UpdatePlayerState()
        {
            Player.IsEnabled = ProjectController.WorkMode == ProjectWorkModes.Test;
        }
    }
}
