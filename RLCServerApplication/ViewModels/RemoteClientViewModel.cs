using System;
using RLCCore;
using RLCServerApplication.Infrastructure;

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

        public RemoteClientViewModel(RemoteClient remoteClient)
        {
            this.RemoteClient = remoteClient ?? throw new ArgumentNullException(nameof(remoteClient));
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

        private void CommitChanges()
        {
            RemoteClient.Number = Number;
            RemoteClient.Name = Name;
            RemoteClient.Cyclogramm = Cyclogramm;
            OnClose?.Invoke(this, new RemoteClientEditorCloseEventArgs(true));
        }

        private void Discard()
        {
            OnClose?.Invoke(this, new RemoteClientEditorCloseEventArgs(false));
        }

        #region Commands

        public RelayCommand SaveChangesCommand { get; private set; }
        public RelayCommand CloseCommand { get; private set; }

        public void CreateCommands()
        {
            SaveChangesCommand = new RelayCommand(
                () => {
                    CommitChanges();
                },
                () => true
            );

            CloseCommand = new RelayCommand(
                () => { Discard(); },
                () => true
            );
        }

        #endregion
    }
}
