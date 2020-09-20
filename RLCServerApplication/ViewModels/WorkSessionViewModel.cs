using Core;
using Core.RemoteOperations;
using RLCServerApplication.Infrastructure;
using RLCServerApplication.Infrastructure.Command;
using System;
using System.Threading;

namespace RLCServerApplication.ViewModels
{
    public class WorkSessionViewModel : ViewModelBase
    {
        private readonly WorkSession workSession;
        private readonly UdpClientOperator clientOperator;

        public SettingsViewModel SettingsViewModel { get; }
        public RemoteClientsViewModel RemoteClientsViewModel { get; }

        public WorkSessionViewModel(WorkSession workSession, SettingsViewModel settingsViewModel, RemoteClientsViewModel remoteClientsViewModel)
        {
            this.workSession = workSession ?? throw new ArgumentNullException(nameof(workSession));
            clientOperator = workSession.ClientOperator;

            SettingsViewModel = settingsViewModel ?? throw new ArgumentNullException(nameof(settingsViewModel));
            RemoteClientsViewModel = remoteClientsViewModel ?? throw new ArgumentNullException(nameof(remoteClientsViewModel));

            System.Threading.Tasks.Task.Run(
                () => {
                    Thread.Sleep(5000);
                    Player = workSession.Player;
                }
            );
        }

        private SequencePlayer player;
        public SequencePlayer Player {
            get => player;
            set => SetField(ref player, value);
        }

        private TimeSpan playFromTime;
        public TimeSpan PlayFromTime {
            get => playFromTime;
            set => SetField(ref playFromTime, value);
        }        

        #region Commands


        #region PlayCommand

        public DelegateCommand playCommand;
        public DelegateCommand PlayCommand {
            get {
                if(playCommand == null) {
                    CreatePlayCommand();
                }
                return playCommand;
            }
        }

        private void CreatePlayCommand()
        {
            playCommand = new DelegateCommand(
                clientOperator.Play,
                () => clientOperator.CanPlay
            );
            playCommand.CanExecuteChangedWith(clientOperator, x => x.CanPlay);
        }

        #endregion PlayCommand

        #region PlayFromButtonCommand        

        private DelegateCommand playFromButtonCommand;
        public DelegateCommand PlayFromButtonCommand {
            get {
                if(playFromButtonCommand == null) {
                    CreatePlayFromCommand();
                }
                return playFromButtonCommand;
            }
        }

        private void CreatePlayFromCommand()
        {
            playFromButtonCommand = new DelegateCommand(
                () =>  {
                    clientOperator.PlayFrom(PlayFromTime);
                },
                () => clientOperator.CanPlay && workSession.State == SessionState.Test && PlayFromTime.TotalMilliseconds > 0
            );
            playFromButtonCommand.CanExecuteChangedWith(this, x => x.PlayFromTime);
            playFromButtonCommand.CanExecuteChangedWith(clientOperator, x => x.CanPlay);
            playFromButtonCommand.CanExecuteChangedWith(workSession, x => x.State);
        }

        #endregion PlayFromButtonCommand

        #region StopCommand

        private DelegateCommand stopCommand;
        public DelegateCommand StopCommand {
            get {
                if(stopCommand == null) {
                    CreateStopCommand();
                }
                return stopCommand;
            }
        }

        private void CreateStopCommand()
        {
            stopCommand = new DelegateCommand(
                () => clientOperator.Stop(),
                () => clientOperator.CanStop
            );
            stopCommand.CanExecuteChangedWith(clientOperator, x => x.CanStop);
        }


        #endregion StopCommand

        #region PauseCommand

        private DelegateCommand pauseCommand;
        public DelegateCommand PauseCommand {
            get {
                if(pauseCommand == null) {
                    CreatePauseCommand();
                }
                return pauseCommand;
            }
        }

        private void CreatePauseCommand()
        {
            pauseCommand = new DelegateCommand(
                () => clientOperator.Pause(),
                () => clientOperator.CanPause
            );
            pauseCommand.CanExecuteChangedWith(clientOperator, x => x.CanPause);
        }

        #endregion PauseCommand

        #region MuteCommand

        public DelegateCommand muteCommand;
        public DelegateCommand MuteCommand {
            get {
                if(muteCommand == null) {
                    CreateMuteCommand();
                }
                return muteCommand;
            }
        }

        private void CreateMuteCommand()
        {
            muteCommand = new DelegateCommand(
                () => workSession.Player.Volume = 0f
            );
        }

        #endregion MuteCommand

        #region SwitchToSetupCommand

        private DelegateCommand switchToSetupCommand;
        public DelegateCommand SwitchToSetupCommand {
            get {
                if(switchToSetupCommand == null) {
                    CreateSwitchToSetupCommand();
                }
                return switchToSetupCommand;
            }
        }

        private void CreateSwitchToSetupCommand()
        {
            switchToSetupCommand = new DelegateCommand(
                () => {
                    try {
                        clientOperator.Stop();
                    }
                    finally {
                        workSession.SwitchToSetupMode();
                    }
                },
                () => workSession.CanSwitchToSetupMode
            );
            switchToSetupCommand.CanExecuteChangedWith(workSession, x => x.CanSwitchToSetupMode);
        }

        #endregion SwitchToSetupCommand

        #region SwitchToTestCommand

        private DelegateCommand switchToTestCommand;
        public DelegateCommand SwitchToTestCommand {
            get {
                if(switchToTestCommand == null) {
                    CreateSwitchToTestCommand();
                }
                return switchToTestCommand;
            }
        }

        private void CreateSwitchToTestCommand()
        {
            switchToTestCommand = new DelegateCommand(
                () => workSession.SwitchToTestMode(),
                () => workSession.CanSwitchToTestMode
            );
            switchToTestCommand.CanExecuteChangedWith(workSession, x => x.CanSwitchToTestMode);
        }

        #endregion SwitchToTestCommand

        #region SwitchToWorkCommand

        private DelegateCommand switchToWorkCommand;
        public DelegateCommand SwitchToWorkCommand {
            get {
                if(switchToWorkCommand == null) {
                    CreateSwitchToWorkCommand();
                }
                return switchToWorkCommand;
            }
        }

        private void CreateSwitchToWorkCommand()
        {
            switchToWorkCommand = new DelegateCommand(
                () => workSession.SwitchToWorkMode(),
                () => workSession.CanSwitchToWorkMode
            );

            switchToWorkCommand.CanExecuteChangedWith(workSession, x => x.CanSwitchToWorkMode);
        }

        #endregion SwitchToWorkCommand


        #endregion Commands
    }
}
