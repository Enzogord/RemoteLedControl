using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Core;
using Core.IO;
using Core.Model;
using RLCCore;
using RLCCore.Domain;
using RLCCore.Serialization;
using RLCServerApplication.Infrastructure;
using RLCServerApplication.Infrastructure.Command;

namespace RLCServerApplication.ViewModels
{
    public class RemoteClientsViewModel : ViewModelBase
    {
        private readonly RemoteControlProject remoteControlProject;
        private readonly RLCProjectController projectController;
        private readonly RemovableDrivesProvider removableDrivesProvider;

        public RemoteClientsViewModel(RLCProjectController projectController, RemovableDrivesProvider removableDrivesProvider)
        {
            this.projectController = projectController ?? throw new System.ArgumentNullException(nameof(projectController));
            this.removableDrivesProvider = removableDrivesProvider ?? throw new System.ArgumentNullException(nameof(removableDrivesProvider));
            this.remoteControlProject = projectController.CurrentProject;
            ConfigureBindings();
        }

        public bool CanEdit => projectController.WorkMode == ProjectWorkModes.Setup;

        public ObservableCollection<RemoteClient> Clients => projectController.CurrentProject?.Clients;

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

        private RemoteClientEditViewModel remoteClientViewModel;
        public RemoteClientEditViewModel RemoteClientViewModel {
            get => remoteClientViewModel;
            set => SetField(ref remoteClientViewModel, value, () => RemoteClientViewModel);
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
                SetField(ref selectedRemovableDrive, value, () => SelectedRemovableDrive);
                ExportClientDataToSDCommand.RaiseCanExecuteChanged();
            }
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

        #region OpenRemovableDriveCommand

        public DelegateCommand openRemovableDriveCommand;
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
                string clientFolder = projectController.SaveController.GetClientFolder(SelectedClient.Number, SelectedClient.Name);
                string[] files = Directory.GetFiles(clientFolder);
                foreach(var file in files) {
                    File.Copy(file, Path.Combine(exportPath, Path.GetFileName(file)), true);
                }
                settingWriter.WriteSettings(outputStream, remoteControlProject, SelectedClient);
            }
        }

        public DelegateCommand<IList> exportClientDataToSDCommand;
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

        public DelegateCommand<IList> exportClientDataCommand;
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

        public DelegateCommand openClientEditorCommand;
        public DelegateCommand OpenClientEditorCommand {
            get {
                if(openClientEditorCommand == null) {
                    openClientEditorCommand = new DelegateCommand(
                        () => {
                            if(SelectedClient != null) {
                                var clientModel = new RemoteClientEditModel(SelectedClient, remoteControlProject, projectController.SaveController);
                                RemoteClientViewModel = new RemoteClientEditViewModel(clientModel, projectController.SaveController);
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
                            var clientModel = new RemoteClientEditModel(remoteControlProject, projectController.SaveController);
                            RemoteClientViewModel = new RemoteClientEditViewModel(clientModel, projectController.SaveController);
                            RemoteClientViewModel.OnClose += (sender, e) => {
                                if(e.Commited) {
                                    remoteControlProject.AddClient(RemoteClientViewModel.RemoteClientEditModel.RemoteClient);
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
                                projectController.SaveController.DeleteClientFolder(selectedClient);
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
