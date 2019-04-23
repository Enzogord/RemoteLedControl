using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using RLCCore;
using RLCServerApplication.Infrastructure;

namespace RLCServerApplication.ViewModels
{
    public class RemoteClientsViewModel : ViewModelBase
    {
        private readonly RemoteControlProject remoteControlProject;

        public RemoteClientsViewModel(RLCProjectController projectController)
        {
            //var projectController = lifetimeScope.Resolve<RLCProjectController>();
            this.remoteControlProject = projectController.CurrentProject;
            Clients = new ReadOnlyObservableCollection<RemoteClient>(remoteControlProject.Clients);
        }

        private ReadOnlyObservableCollection<RemoteClient> clients;
        public ReadOnlyObservableCollection<RemoteClient> Clients {
            get => clients;
            set => SetField(ref clients, value, () => Clients);
        }
    }
}
