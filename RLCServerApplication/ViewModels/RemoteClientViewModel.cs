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

        public RemoteClientViewModel(RemoteClient remoteClient, SaveController saveController, FileHolder fileHolder)
        {
            this.RemoteClient = remoteClient ?? throw new ArgumentNullException(nameof(remoteClient));
            this.saveController = saveController ?? throw new ArgumentNullException(nameof(saveController));
            this.fileHolder = fileHolder ?? throw new ArgumentNullException(nameof(fileHolder));
            LoadValues();
            CreateCommands();
        }

        private int number;
        public int Number {
            get => number;
            set {
                SetField(ref number, value, () => Number);
                OnPropertyChanged(nameof(CanCommitChanges));
            }
        }

        private string name;
        public string Name {
            get => name;
            set {
                SetField(ref name, value, () => Name);
                OnPropertyChanged(nameof(CanCommitChanges));
            }
        }

        private Cyclogramm cyclogramm;
        public Cyclogramm Cyclogramm {
            get => cyclogramm;
            set => SetField(ref cyclogramm, value, () => Cyclogramm);
        }

        private CyclogrammViewModel cyclogrammViewModel;
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
            CyclogrammViewModel = new CyclogrammViewModel(Cyclogramm);
        }

        public bool CanCommitChanges => Number >= 0 && !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Cyclogramm.FilePath);

        private void CommitChanges()
        {
            RemoteClient.Number = Number;
            RemoteClient.Name = Name;
            RemoteClient.Cyclogramm = Cyclogramm;
            string fileName = Path.GetFileNameWithoutExtension(Cyclogramm.FilePath);
            string workFilePath = saveController.GetClientWorkFullPath($"{Number}_{Name}", fileName + ".cyc");
            Directory.CreateDirectory(Path.GetDirectoryName(workFilePath));
            CyclogrammCsvToCycConverter converter = new CyclogrammCsvToCycConverter();
            using(FileStream sourceStream = new FileStream(Cyclogramm.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                FileStream workedStream = new FileStream(workFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                fileHolder.HoldFile(workedStream);
                converter.Convert(sourceStream, workedStream);                
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
                () => !string.IsNullOrWhiteSpace(Cyclogramm.FilePath)
            );
            SaveChangesCommand.CanExecuteChangedWith(Cyclogramm, x => x.FilePath);

            CloseCommand = new DelegateCommand(
                () => { Discard(); },
                () => true
            );
        }

        #endregion
    }
}
