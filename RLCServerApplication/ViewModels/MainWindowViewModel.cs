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
            ProjectController.PropertyChanged += (sender, e) => {
                if(e.PropertyName == nameof(ProjectController.ServicesIsReady)) {
                    if(StartServicesCommand != null) {
                        StartServicesCommand.RaiseCanExecuteChanged();
                    }
                    if(StopServicesCommand != null) {
                        StopServicesCommand.RaiseCanExecuteChanged();
                    }
                }
            };

            SettingsViewModel = new SettingsViewModel(ProjectController);
            RemoteClientsViewModel = new RemoteClientsViewModel(ProjectController);

            Player = new SequencePlayer();

            ConfigureBindings();
            CreateCommands();
        }

        private void ConfigureBindings()
        {
            BindAction(UpdatePlayerCommands, ProjectController,
                   x => x.ServicesIsReady
               );

            BindAction(UpdatePlayerCommands, Player,
                x => x.CanPlay,
                x => x.CanPause,
                x => x.CanStop
            );
        }

        #region Commands

        public RelayCommand PlayCommand { get; private set; }
        public RelayCommand StopCommand { get; private set; }
        public RelayCommand PauseCommand { get; private set; }
        public RelayCommand StartServicesCommand { get; private set; }
        public RelayCommand StopServicesCommand { get; private set; }
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
                    if(Player.CanPlay) {
                        ProjectController.RemoteClientsOperator.Play();
                        Player.Play();
                    }
                },
                () => {
                    if(!ProjectController.ServicesIsReady) {
                        return false;
                    }
                    var playStates = new[]{ OperatorStates.Ready, OperatorStates.Stop, OperatorStates.Pause };
                    return playStates.Contains(ProjectController.RemoteClientsOperator.State) && Player.CanPlay;
                }
            );
        }

        private void CreateStopCommand()
        {
            StopCommand = new RelayCommand(
                () => {
                    if(Player.CanStop) {
                        ProjectController.RemoteClientsOperator.Stop();
                        Player.Stop();
                    }
                },
                () => {
                    if(!ProjectController.ServicesIsReady) {
                        return false;
                    }
                    var stopStates = new[]{ OperatorStates.Play, OperatorStates.Pause };
                    return stopStates.Contains(ProjectController.RemoteClientsOperator.State) && Player.CanStop;
                }
            );
        }

        private void CreatePauseCommand()
        {
            PauseCommand = new RelayCommand(
                () => {
                    if(Player.CanPause) {
                        ProjectController.RemoteClientsOperator.Pause();
                        Player.Pause();
                    }
                },
                () => {
                    if(!ProjectController.ServicesIsReady) {
                        return false;
                    }
                    return ProjectController.RemoteClientsOperator.State == OperatorStates.Play && Player.CanPause;
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
                () => true
            );
        }

        private void CreateSwitchToSetupCommand()
        {
            SwitchToSetupCommand = new RelayCommand(
                () => {
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
                },
                () => true
            );
        }

        private void CreateSwitchToWorkCommand()
        {
            SwitchToWorkCommand = new RelayCommand(
                () => {
                    ProjectController.SwitchToWorkMode();
                },
                () => true
            );
        }

        #endregion Commands

        private void UpdatePlayerCommands()
        {
            PlayCommand?.RaiseCanExecuteChanged();
            StopCommand?.RaiseCanExecuteChanged();
            PauseCommand?.RaiseCanExecuteChanged();
        }
    }
}
