using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using Core;
using RLCCore;
using RLCCore.Domain;
using RLCServerApplication.Infrastructure;
using RLCServerApplication.Infrastructure.Command;

namespace RLCServerApplication.ViewModels
{
    public class RemoteClientsViewModel : ViewModelBase
    {
        private readonly RemoteControlProject remoteControlProject;
        private readonly RLCProjectController projectController;

        public RemoteClientsViewModel(RLCProjectController projectController)
        {
            this.projectController = projectController ?? throw new System.ArgumentNullException(nameof(projectController));
            this.remoteControlProject = projectController.CurrentProject;
            ConfigureBindings();
        }

        public bool CanEdit => projectController.WorkMode == ProjectWorkModes.Setup;

        public ObservableCollection<RemoteClient> Clients => projectController.CurrentProject.Clients;

        private RemoteClient selectedClient;
        public RemoteClient SelectedClient {
            get => selectedClient;
            set {
                if(SetField(ref selectedClient, value, () => SelectedClient)) {
                    if(selectedClient == null) {
                        RemoteClientViewModel = null;
                    }
                    DeleteClientCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private RemoteClientViewModel remoteClientViewModel;
        public RemoteClientViewModel RemoteClientViewModel {
            get => remoteClientViewModel;
            set => SetField(ref remoteClientViewModel, value, () => RemoteClientViewModel);
        }

        private void ConfigureBindings()
        {
            CreateNotificationBinding().AddProperty(nameof(CanEdit))
                .SetNotifier(projectController)
                .BindToProperty(x => x.WorkMode)
                .End();

            CreateNotificationBinding().AddProperty(nameof(Clients))
                .SetNotifier(projectController)
                .BindToProperty(x => x.CurrentProject)
                .End();
        }

        #region OpenClientEditorCommand

        public DelegateCommand openClientEditorCommand;
        public DelegateCommand OpenClientEditorCommand {
            get {
                if(openClientEditorCommand == null) {
                    openClientEditorCommand = new DelegateCommand(
                        () => {
                            if(SelectedClient != null) {
                                RemoteClientViewModel = new RemoteClientViewModel(SelectedClient, projectController.SaveController, projectController.FileHolder);
                                RemoteClientViewModel.OnClose += (sender, e) => { RemoteClientViewModel = null; };
                            }
                        },
                        () => SelectedClient != null
                    );
                }
                return openClientEditorCommand;
            }
        }

        #endregion OpenClientEditorCommand

        #region AddNewClientCommand

        public DelegateCommand addNewClientCommand;
        public DelegateCommand AddNewClientCommand {
            get {
                if(addNewClientCommand == null) {
                    addNewClientCommand = new DelegateCommand(
                        () => {
                            int newClientNumber = Clients.Count != 0 ? Clients.Max(x => x.Number) + 1 : 1;
                            RemoteClient newRemoteClient = new RemoteClient("Новый клиент", newClientNumber);
                            RemoteClientViewModel = new RemoteClientViewModel(newRemoteClient, projectController.SaveController, projectController.FileHolder);
                            RemoteClientViewModel.OnClose += (sender, e) => {
                                if(e.Commited) {
                                    remoteControlProject.AddClient(newRemoteClient);
                                }
                                RemoteClientViewModel = null;
                            };
                        },
                        () => true
                    );
                }
                return addNewClientCommand;
            }
        }

        #endregion AddNewClientCommand

        #region DeleteClientCommand

        public DelegateCommand<IList> deleteClientCommand;
        public DelegateCommand<IList> DeleteClientCommand {
            get {
                if(deleteClientCommand == null) {
                    deleteClientCommand = new DelegateCommand<IList>(
                        (selectedClients) => {
                            var selectedClientsClone = selectedClients.OfType<RemoteClient>().ToList();
                            foreach(var selectedClient in selectedClientsClone) {
                                remoteControlProject.DeleteClient(selectedClient);
                            }
                        },
                        (selectedClients) => { 
                            return SelectedClient != null; 
                        }
                    );
                }
                return deleteClientCommand;
            }
        }

        #endregion DeleteClientCommand	
    }
}
