using System;
using RLCCore;
using RLCServerApplication.Infrastructure;

namespace RLCServerApplication.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public RLCProjectController RLCProjectController { get; private set; }

        public SettingsViewModel(RLCProjectController rlcProjectController)
        {
            RLCProjectController = rlcProjectController ?? throw new ArgumentNullException(nameof(rlcProjectController));
        }
    }
}
