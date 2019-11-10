using System;
using Core;
using RLCCore;
using RLCServerApplication.Infrastructure;
using RLCServerApplication.Infrastructure.Command;

namespace RLCServerApplication.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public RLCProjectController RLCProjectController { get; }
        public bool CanEdit => RLCProjectController.WorkMode == ProjectWorkModes.Setup && RLCProjectController.CurrentProject != null;

        public SettingsViewModel(RLCProjectController rlcProjectController)
        {
            RLCProjectController = rlcProjectController ?? throw new ArgumentNullException(nameof(rlcProjectController));
            CreateCommands();
            ConfigureBindings();
        }

        private void ConfigureBindings()
        {
            CreateNotificationBinding().AddProperty(nameof(CanEdit))
                .SetNotifier(RLCProjectController)
                .BindToProperty(x => x.WorkMode)
                .BindToProperty(x => x.CurrentProject)
                .End();
        }

        #region Commands

        private void CreateCommands()
        {
            CreateAddAudioTrackCommand();
        }

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
                        RLCProjectController.CurrentProject.SoundtrackFilePath = dlg.FileName;
                    }
                },
                () => CanEdit
            );
            AddAudioTrackCommand.CanExecuteChangedWith(this, x => x.CanEdit);
        }

        #endregion AddAudioTrackCommand

        #endregion Commands
    }
}
