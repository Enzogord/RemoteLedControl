﻿using System.Collections.ObjectModel;
using System.Linq;
using RLCCore;          
using RLCServerApplication.Infrastructure;

namespace RLCServerApplication.ViewModels
{
    public class RemoteClientsViewModel : ViewModelBase
    {
        private readonly RemoteControlProject remoteControlProject;

        public RemoteClientsViewModel(RLCProjectController projectController)
        {
            this.remoteControlProject = projectController.CurrentProject;
            Clients = new ReadOnlyObservableCollection<RemoteClient>(remoteControlProject.Clients);
            CreateCommands();
        }

        private ReadOnlyObservableCollection<RemoteClient> clients;
        public ReadOnlyObservableCollection<RemoteClient> Clients {
            get => clients;
            set => SetField(ref clients, value, () => Clients);
        }

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

        public RelayCommand OpenClientEditorCommand { get; private set; }
        public RelayCommand AddNewClientCommand { get; private set; }
        public RelayCommand DeleteClientCommand { get; private set; }

        private void CreateCommands()
        {
            OpenClientEditorCommand = new RelayCommand(
                () => {
                    if(SelectedClient != null) {
                        RemoteClientViewModel = new RemoteClientViewModel(SelectedClient);
                        RemoteClientViewModel.OnClose += (sender, e) => { RemoteClientViewModel = null; };
                    }
                },
                () => SelectedClient != null
            );

            AddNewClientCommand = new RelayCommand(
                () => {
                    RemoteClient newRemoteClient = new RemoteClient("Новый клиент", Clients.Max(x => x.Number) + 1);
                    RemoteClientViewModel = new RemoteClientViewModel(newRemoteClient);
                    RemoteClientViewModel.OnClose += (sender, e) => {
                        if(e.Commited) {
                            remoteControlProject.AddClient(newRemoteClient);
                        }
                        RemoteClientViewModel = null;
                    };
                },
                () => true
            );

            DeleteClientCommand = new RelayCommand(
                () => { remoteControlProject.DeleteClient(SelectedClient); },
                () => SelectedClient != null
            );
        }
    }
}