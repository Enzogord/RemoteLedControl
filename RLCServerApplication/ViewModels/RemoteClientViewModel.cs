using System;
using System.IO;
using Core.CyclogrammUtility;
using Core.IO;
using RLCCore;
using RLCCore.Domain;
using RLCServerApplication.Infrastructure;
using RLCServerApplication.Infrastructure.Command;

namespace RLCServerApplication.ViewModels
{
    public class RemoteClientEditorCloseEventArgs : EventArgs
    {
        public bool Commited { get; }

        public RemoteClientEditorCloseEventArgs(bool commited)
        {
            Commited = commited;
        }
    }

    public class RemoteClientViewModel : ViewModelBase
    {
        public RemoteClient RemoteClient { get; }

        public event EventHandler<RemoteClientEditorCloseEventArgs> OnClose;

        public RemoteClientViewModel(RemoteClient remoteClient, RemoteControlProject project, SaveController saveController, FileHolder fileHolder)
        {
            this.RemoteClient = remoteClient ?? throw new ArgumentNullException(nameof(remoteClient));
            this.project = project ?? throw new ArgumentNullException(nameof(project));
            this.saveController = saveController ?? throw new ArgumentNullException(nameof(saveController));
            this.fileHolder = fileHolder ?? throw new ArgumentNullException(nameof(fileHolder));
            LoadValues();
            CreateCommands();
        }

        private int number;
        public int Number {
            get => number;
            set => SetField(ref number, value, () => Number);
        }

        private string name;
        public string Name {
            get => name;
            set => SetField(ref name, value, () => Name);
        }

        private Cyclogramm cyclogramm;
        public Cyclogramm Cyclogramm {
            get => cyclogramm;
            set => SetField(ref cyclogramm, value, () => Cyclogramm);
        }

        private CyclogrammViewModel cyclogrammViewModel;
        private readonly RemoteControlProject project;
        private readonly SaveController saveController;
        private readonly FileHolder fileHolder;

        public CyclogrammViewModel CyclogrammViewModel {
            get => cyclogrammViewModel;
            set => SetField(ref cyclogrammViewModel, value, () => CyclogrammViewModel);
        }


        private void LoadValues()
        {
            Number = RemoteClient.Number;
            Name = RemoteClient.Name;
            if(RemoteClient.Cyclogramm == null) {
                Cyclogramm = new Cyclogramm();
            } else {
                Cyclogramm = RemoteClient.Cyclogramm;
            }
            CyclogrammViewModel = new CyclogrammViewModel(Cyclogramm, RemoteClient, saveController);
        }

        private void CommitChanges()
        {
            if(string.IsNullOrWhiteSpace(Name)) {
                return;
            }
            string clientWorkPath = saveController.GetClientFolder(RemoteClient.Number, RemoteClient.Name);
            if(!Directory.Exists(clientWorkPath)) {
                Directory.CreateDirectory(clientWorkPath);
            }

            if(Name != RemoteClient.Name) {
                Directory.Move(clientWorkPath, Path.Combine(Directory.GetParent(clientWorkPath).FullName, $"{Number}_{Name}"));
            }

            clientWorkPath = saveController.GetClientFolder(Number, Name);

            RemoteClient.Number = Number;
            RemoteClient.Name = Name;
            RemoteClient.Cyclogramm = Cyclogramm;

            if(!string.IsNullOrWhiteSpace(Cyclogramm.FilePath)) {
                string fullSavePath = Path.Combine(clientWorkPath, "Data.cyc");
                CyclogrammCsvToCycConverter converter = new CyclogrammCsvToCycConverter();
                using(FileStream sourceStream = new FileStream(Cyclogramm.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using(FileStream destinationStream = new FileStream(fullSavePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read)) {
                    converter.Convert(sourceStream, destinationStream);
                }
                Cyclogramm.FilePath = null;
            }
            
            OnClose?.Invoke(this, new RemoteClientEditorCloseEventArgs(true));
        }

        private void Discard()
        {
            OnClose?.Invoke(this, new RemoteClientEditorCloseEventArgs(false));
        }

        #region Commands

        public DelegateCommand SaveChangesCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }

        public void CreateCommands()
        {
            SaveChangesCommand = new DelegateCommand(
                () => {
                    CommitChanges();
                },
                () => {
                    return Number > 0
                        && !string.IsNullOrWhiteSpace(Name)
                        && (
                            (!project.ClientExists(Number, Name) && (RemoteClient.Number != Number || RemoteClient.Name != Name))
                            || ((RemoteClient.Number == Number || RemoteClient.Name == Name) && !string.IsNullOrWhiteSpace(Cyclogramm.FilePath))
                        )
                        && (!string.IsNullOrWhiteSpace(Cyclogramm.FilePath) || CyclogrammViewModel.ConvertedCyclogrammExists);
                } 
            );
            SaveChangesCommand.CanExecuteChangedWith(Cyclogramm, x => x.FilePath);
            SaveChangesCommand.CanExecuteChangedWith(this, x => x.Number, x => x.Name);

            CloseCommand = new DelegateCommand(
                () => { Discard(); },
                () => true
            );
        }

        #endregion
    }
}
