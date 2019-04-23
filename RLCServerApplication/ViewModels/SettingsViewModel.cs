using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using RLCCore;
using RLCServerApplication.Infrastructure;

namespace RLCServerApplication.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public RLCProjectController RLCProjectController { get; private set; }

        public SettingsViewModel()
        {
            RLCProjectController = Bootstrapper.RootScope.Resolve<RLCProjectController>();
        }
    }
}
