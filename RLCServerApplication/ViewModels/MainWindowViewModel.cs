using System.Linq;
using Autofac;
using RLCCore;
using RLCCore.RemoteOperations;
using RLCServerApplication.Infrastructure;

namespace RLCServerApplication.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private RLCProjectController projectController;

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

        public MainWindowViewModel()
        {
            projectController = Bootstrapper.RootScope.Resolve<RLCProjectController>();
            projectController.PropertyChanged += (sender, e) =>
            {
                if(e.PropertyName == nameof(projectController.ServicesIsReady)) {
                    if(StartServicesCommand != null) {
                        StartServicesCommand.RaiseCanExecuteChanged();
                    }
                    if(StopServicesCommand != null) {
                        StopServicesCommand.RaiseCanExecuteChanged();
                    }
                }
            };

            PlayCommand = new RelayCommand(() =>
            {
                projectController.RemoteClientsOperator.Play();
            },
            () =>
            {
                if(!projectController.ServicesIsReady) {
                    return false;
                }
                var playStates = new[]{ OperatorStates.Ready, OperatorStates.Stop, OperatorStates.Pause };
                return playStates.Contains(projectController.RemoteClientsOperator.State);
            });

            StopCommand = new RelayCommand(() =>
            {
                projectController.RemoteClientsOperator.Stop();

            },
            () =>
            {
                if(!projectController.ServicesIsReady) {
                    return false;
                }
                var stopStates = new[]{ OperatorStates.Play, OperatorStates.Pause };
                return stopStates.Contains(projectController.RemoteClientsOperator.State);
            });

            PauseCommand = new RelayCommand(() =>
            {
                projectController.RemoteClientsOperator.Pause();

            },
            () =>
            {
                if(!projectController.ServicesIsReady) {
                    return false;
                }
                return projectController.RemoteClientsOperator.State == OperatorStates.Play;
            });

            StartServicesCommand = new RelayCommand(
            () =>
            {
                projectController.StartServices();
                projectController.RemoteClientsOperator.StateChanged += RemoteClientsOperator_StateChanged;
                UpdatePlayerCommands();
            },
            () => !projectController.ServicesIsReady
            );

            StopServicesCommand = new RelayCommand(
            () =>
            {
                projectController.StopServices();
                projectController.RemoteClientsOperator.StateChanged -= RemoteClientsOperator_StateChanged;
                UpdatePlayerCommands();
            },
            () => projectController.ServicesIsReady
            );
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
