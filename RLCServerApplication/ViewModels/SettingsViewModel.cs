using System;
using Core;
using RLCCore;
using RLCServerApplication.Infrastructure;

namespace RLCServerApplication.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public RLCProjectController RLCProjectController { get; }
        public bool CanEdit => RLCProjectController.WorkMode == ProjectWorkModes.Setup;

        public SettingsViewModel(RLCProjectController rlcProjectController)
        {
            RLCProjectController = rlcProjectController ?? throw new ArgumentNullException(nameof(rlcProjectController));
            ConfigureBindings();
        }

        private void ConfigureBindings()
        {
            Bind(() => CanEdit, RLCProjectController, x => x.WorkMode);
        }
    }
}
