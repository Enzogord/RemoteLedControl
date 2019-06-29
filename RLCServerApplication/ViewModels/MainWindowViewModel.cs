using System.Linq;
using Core;
using NAudioPlayer;
using RLCCore;
using RLCCore.RemoteOperations;
using RLCServerApplication.Infrastructure;

namespace RLCServerApplication.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private RLCProjectController projectController;

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

        public RelayCommand PlayCommand { get; private set; }
        public RelayCommand StopCommand { get; private set; }
        public RelayCommand PauseCommand { get; private set; }
        public RelayCommand StartServicesCommand { get; private set; }
        public RelayCommand StopServicesCommand { get; private set; }
        public RelayCommand AddAudioTrackCommand { get; private set; }

        public MainWindowViewModel()
        {
            projectController = new RLCProjectController();
            projectController.PropertyChanged += (sender, e) => {
                if(e.PropertyName == nameof(projectController.ServicesIsReady)) {
                    if(StartServicesCommand != null) {
                        StartServicesCommand.RaiseCanExecuteChanged();
                    }
                    if(StopServicesCommand != null) {
                        StopServicesCommand.RaiseCanExecuteChanged();
                    }
                }
            };

            SettingsViewModel = new SettingsViewModel(projectController);
            RemoteClientsViewModel = new RemoteClientsViewModel(projectController);

            Player = new SequencePlayer();
            Player.PropertyChanged += Player_PropertyChanged;

            CreateCommands();
        }

        #region Commands

        private void CreateCommands()
        {
            CreatePlayCommand();
            CreateStopCommand();
            CreatePauseCommand();
            CreateStartServicesCommand();
            CreateStopServicesCommand();
            CreateAddAudioTrackCommand();
        }

        private void CreatePlayCommand()
        {
            PlayCommand = new RelayCommand(
                () => {
                    if(Player.CanPlay) {
                        projectController.RemoteClientsOperator.Play();
                        Player.Play();
                    }
                },
                () => {
                    if(!projectController.ServicesIsReady) {
                        return false;
                    }
                    var playStates = new[]{ OperatorStates.Ready, OperatorStates.Stop, OperatorStates.Pause };
                    return playStates.Contains(projectController.RemoteClientsOperator.State) && Player.CanPlay;
                }
            );
        }

        private void CreateStopCommand()
        {
            StopCommand = new RelayCommand(
                () => {
                    if(Player.CanStop) {
                        projectController.RemoteClientsOperator.Stop();
                        Player.Stop();
                    }
                },
                () => {
                    if(!projectController.ServicesIsReady) {
                        return false;
                    }
                    var stopStates = new[]{ OperatorStates.Play, OperatorStates.Pause };
                    return stopStates.Contains(projectController.RemoteClientsOperator.State) && Player.CanStop;
                }
            );
        }

        private void CreatePauseCommand()
        {
            PauseCommand = new RelayCommand(
                () => {
                    if(Player.CanPause) {
                        projectController.RemoteClientsOperator.Pause();
                        Player.Pause();
                    }
                },
                () => {
                    if(!projectController.ServicesIsReady) {
                        return false;
                    }
                    return projectController.RemoteClientsOperator.State == OperatorStates.Play && Player.CanPause;
                }
            );
        }

        private void CreateStartServicesCommand()
        {
            StartServicesCommand = new RelayCommand(
                () => {
                    projectController.StartServices();
                    projectController.RemoteClientsOperator.StateChanged += RemoteClientsOperator_StateChanged;
                    UpdatePlayerCommands();
                },
                () => !projectController.ServicesIsReady
            );
        }

        private void CreateStopServicesCommand()
        {
            StopServicesCommand = new RelayCommand(
                () => {
                    projectController.RemoteClientsOperator.StateChanged -= RemoteClientsOperator_StateChanged;
                    projectController.StopServices();
                    UpdatePlayerCommands();
                },
                () => projectController.ServicesIsReady
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

        #endregion Commands

        private void Player_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName) {
                case nameof(Player.CanPlay):
                case nameof(Player.CanPause):
                case nameof(Player.CanStop):
                    UpdatePlayerCommands();
                    break;
                default:
                    break;
            }
        }

        private void RemoteClientsOperator_StateChanged(object sender, OperatorStateEventArgs e)
        {
            UpdatePlayerCommands();
        }

        private void UpdatePlayerCommands()
        {
            if(PlayCommand != null) {
                PlayCommand.RaiseCanExecuteChanged();
            }
            if(StopCommand != null) {
                StopCommand.RaiseCanExecuteChanged();
            }
            if(PauseCommand != null) {
                PauseCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
