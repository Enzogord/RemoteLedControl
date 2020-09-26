using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Core;
using Core.IO;
using Core.Model;
using Core.RemoteOperations;
using RLCCore.Domain;
using RLCCore.Serialization;
using RLCServerApplication.Infrastructure;
using RLCServerApplication.Infrastructure.Command;

namespace RLCServerApplication.ViewModels
{
    public class RemoteClientsViewModel : ViewModelBase
    {
        private readonly WorkSession workSession;
        private readonly RemovableDrivesProvider removableDrivesProvider;
        private readonly SaveController saveController;
        private readonly IClientConnectionProvider clientConnectionProvider;
        private readonly RemoteControlProject project;

        public RemoteClientsViewModel(WorkSession workSession, RemovableDrivesProvider removableDrivesProvider, SaveController saveController, IClientConnectionProvider clientConnectionProvider)
        {
            this.workSession = workSession ?? throw new System.ArgumentNullException(nameof(workSession));
            this.removableDrivesProvider = removableDrivesProvider ?? throw new System.ArgumentNullException(nameof(removableDrivesProvider));
            this.saveController = saveController ?? throw new System.ArgumentNullException(nameof(saveController));
            this.clientConnectionProvider = clientConnectionProvider ?? throw new System.ArgumentNullException(nameof(clientConnectionProvider));
            this.project = workSession.Project;
            project.Clients.CollectionChanged += Clients_CollectionChanged;
            ConfigureBindings();
            UpdateClients();
        }

        private void Clients_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void UpdateClients()
        {
            var clientsViewModels = project.Clients.Select(x => new ClientItemViewModel(x, clientConnectionProvider));
            var obsClients = new ObservableCollection<ClientItemViewModel>(clientsViewModels);
            Clients = new ReadOnlyObservableCollection<ClientItemViewModel>(obsClients);
        }

        public bool CanEdit => workSession.State == SessionState.Setup;

        private ReadOnlyObservableCollection<ClientItemViewModel> clients;
        public ReadOnlyObservableCollection<ClientItemViewModel> Clients {
            get => clients;
            private set => SetField(ref clients, value);
        }

        private ClientItemViewModel selectedClientViewModel;
        public ClientItemViewModel SelectedItemViewModel {
            get => selectedClientViewModel;
            set {
                if(SetField(ref selectedClientViewModel, value)) {
                    SelectedClient = selectedClientViewModel.Client;
                }
            }
        }


        private RemoteClient selectedClient;
        public RemoteClient SelectedClient {
            get => selectedClient;
            private set {
                if(SetField(ref selectedClient, value)) {
                    if(selectedClient == null) {
                        RemoteClientViewModel = null;
                    }
                    DeleteClientCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private RemoteClientEditViewModel remoteClientViewModel;
        public RemoteClientEditViewModel RemoteClientViewModel {
            get => remoteClientViewModel;
            set => SetField(ref remoteClientViewModel, value);
        }

        public IEnumerable<string> RemovableDrives => removableDrivesProvider.GetRemovebleDrives();

        public void UpdateRemovableDrives()
        {
            OnPropertyChanged(nameof(RemovableDrives));
        }

        private string selectedRemovableDrive;
        public string SelectedRemovableDrive {
            get => selectedRemovableDrive;
            set {
                SetField(ref selectedRemovableDrive, value);
                ExportClientDataToSDCommand.RaiseCanExecuteChanged();
            }
        }

        private void ConfigureBindings()
        {
            CreateNotificationBinding().AddProperty(nameof(CanEdit))
                .SetNotifier(workSession)
                .BindToProperty(x => x.State)
                .End();

            CreateNotificationBinding().AddAction(UpdateClients)
                .SetNotifier(project)
                .BindToProperty(x => x.Clients)
                .End();
        }

        #region OpenRemovableDriveCommand

        private DelegateCommand openRemovableDriveCommand;
        public DelegateCommand OpenRemovableDriveCommand {
            get {
                if(openRemovableDriveCommand == null) {
                    openRemovableDriveCommand = new DelegateCommand(
                        () => {
                            System.Diagnostics.Process.Start("explorer", SelectedRemovableDrive);
                        },
                        () => !string.IsNullOrWhiteSpace(SelectedRemovableDrive) && RemovableDrives.Contains(SelectedRemovableDrive)
                    );
                    openRemovableDriveCommand.CanExecuteChangedWith(this, x => x.SelectedRemovableDrive);
                }
                return openRemovableDriveCommand;
            }
        }

        #endregion OpenRemovableDriveCommand	

        #region ExportClientData

        private void ExportClientData(string exportPath)
        {
            SettingWriter settingWriter = new SettingWriter();
            using(Stream outputStream = new FileStream(Path.Combine(exportPath, "set.txt"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read)) {
                outputStream.SetLength(0);
                string clientFolder = saveController.GetClientFolder(SelectedClient.Number, SelectedClient.Name);
                string[] files = Directory.GetFiles(clientFolder);
                foreach(var file in files) {
                    File.Copy(file, Path.Combine(exportPath, Path.GetFileName(file)), true);
                }
                settingWriter.WriteSettings(outputStream, project, SelectedClient);
            }
        }

        private DelegateCommand<IList> exportClientDataToSDCommand;
        public DelegateCommand<IList> ExportClientDataToSDCommand {
            get {
                if(exportClientDataToSDCommand == null) {
                    exportClientDataToSDCommand = new DelegateCommand<IList>(
                        (selectedClients) => {
                            ExportClientData(SelectedRemovableDrive);                            
                        },
                        (selectedClients) => !string.IsNullOrWhiteSpace(SelectedRemovableDrive) && RemovableDrives.Contains(SelectedRemovableDrive) && SelectedClient != null && selectedClients.Count == 1
                    );
                    exportClientDataToSDCommand.CanExecuteChangedWith(this, x => x.SelectedClient);
                }
                return exportClientDataToSDCommand;
            }
        }

        private DelegateCommand<IList> exportClientDataCommand;
        public DelegateCommand<IList> ExportClientDataCommand {
            get {
                if(exportClientDataCommand == null) {
                    exportClientDataCommand = new DelegateCommand<IList>(
                        (selectedClients) => {
                            using(var fbd = new System.Windows.Forms.FolderBrowserDialog()) {
                                System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                                if(result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath)) {
                                    ExportClientData(fbd.SelectedPath);
                                }
                            }
                        },
                        (selectedClients) => SelectedClient != null && selectedClients.Count == 1
                    );
                    exportClientDataCommand.CanExecuteChangedWith(this, x => x.SelectedClient);
                }
                return exportClientDataCommand;
            }
        }

        #endregion ExportClientData

        #region OpenClientEditorCommand

        private DelegateCommand openClientEditorCommand;
        public DelegateCommand OpenClientEditorCommand {
            get {
                if(openClientEditorCommand == null) {
                    openClientEditorCommand = new DelegateCommand(
                        () => {
                            if(SelectedClient != null) {
                                var clientModel = new RemoteClientEditModel(SelectedClient, project, saveController);
                                RemoteClientViewModel = new RemoteClientEditViewModel(clientModel, saveController);
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

        private DelegateCommand addNewClientCommand;
        public DelegateCommand AddNewClientCommand {
            get {
                if(addNewClientCommand == null) {
                    addNewClientCommand = new DelegateCommand(
                        () => {
                            var clientModel = new RemoteClientEditModel(project, saveController);
                            RemoteClientViewModel = new RemoteClientEditViewModel(clientModel, saveController);
                            RemoteClientViewModel.OnClose += (sender, e) => {
                                if(e.Commited) {
                                    project.AddClient(RemoteClientViewModel.RemoteClientEditModel.RemoteClient);
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

        private DelegateCommand<IList> deleteClientCommand;
        public DelegateCommand<IList> DeleteClientCommand {
            get {
                if(deleteClientCommand == null) {
                    deleteClientCommand = new DelegateCommand<IList>(
                        (selectedClients) => {
                            var selectedClientsClone = selectedClients.OfType<RemoteClient>().ToList();
                            foreach(var selectedClient in selectedClientsClone) {
                                project.DeleteClient(selectedClient);
                                saveController.DeleteClientFolder(selectedClient);
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
