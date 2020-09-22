using System;
using System.Net;
using Core;
using Core.Services.FileDialog;
using RLCCore.Settings;
using RLCServerApplication.Infrastructure;
using RLCServerApplication.Infrastructure.Command;

namespace RLCServerApplication.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public WorkSession WorkSession { get; }
        private readonly IOpenFileService openFileService;
        public NetworkController NetworkController { get; }

        public bool CanEdit => WorkSession.State == SessionState.Setup;

        public SettingsViewModel(WorkSession workSession, IOpenFileService openFileService, NetworkController networkController)
        {
            this.WorkSession = workSession ?? throw new ArgumentNullException(nameof(workSession));
            this.openFileService = openFileService ?? throw new ArgumentNullException(nameof(openFileService));
            this.NetworkController = networkController ?? throw new ArgumentNullException(nameof(networkController));
            ConfigureBindings();
        }

        private void ConfigureBindings()
        {
            CreateNotificationBinding().AddProperty(nameof(CanEdit))
                .SetNotifier(WorkSession)
                .BindToProperty(x => x.State)
                .End();
        }

        #region Commands

        #region AddAudioTrackCommand

        private DelegateCommand addAudioTrackCommand;

        public DelegateCommand AddAudioTrackCommand {
            get {
                if(addAudioTrackCommand == null) {
                    CreateAddAudioTrackCommand();
                }
                return addAudioTrackCommand;
            }
        }

        private void CreateAddAudioTrackCommand()
        {
            addAudioTrackCommand = new DelegateCommand(
                () => {
                    string filter = "Mp3 files|*.mp3";
                    string filePath = openFileService.OpenFile(filter, "Открыть", true, true);
                    if(string.IsNullOrWhiteSpace(filePath)) {
                        return;
                    }
                    WorkSession.SetAudioFile(filePath);
                },
                () => CanEdit
            );
            addAudioTrackCommand.CanExecuteChangedWith(this, x => x.CanEdit);
        }

        #endregion AddAudioTrackCommand

        #endregion Commands
    }
}
