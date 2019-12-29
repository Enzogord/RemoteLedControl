using Core.IO;
using Core.Model;
using NotifiedObjectsFramework;
using RLCServerApplication.Infrastructure.Command;
using System;

namespace RLCServerApplication.ViewModels
{
    public class RemoteClientEditViewModel : NotifyPropertyChangedBase
    {
        private readonly SaveController saveController;

        public EventHandler<RemoteClientEditorCloseEventArgs> OnClose;
        public RemoteClientEditModel RemoteClientEditModel { get; }

        public RemoteClientEditViewModel(RemoteClientEditModel remoteClientEditModel, SaveController saveController)
        {
            RemoteClientEditModel = remoteClientEditModel ?? throw new ArgumentNullException(nameof(remoteClientEditModel));
            this.saveController = saveController ?? throw new ArgumentNullException(nameof(saveController));
            CreateNotificationBinding().AddAction(UpdateCyclogrammViewModel).SetNotifier(RemoteClientEditModel).BindToProperty(x => x.Cyclogramm).End();
            remoteClientEditModel.Load();
        }

        private void UpdateCyclogrammViewModel()
        {
            CyclogrammViewModel = new CyclogrammViewModel(RemoteClientEditModel.Cyclogramm, RemoteClientEditModel.RemoteClient, saveController);
        }

        private CyclogrammViewModel cyclogrammViewModel;
        public CyclogrammViewModel CyclogrammViewModel {
            get => cyclogrammViewModel;
            set => SetField(ref cyclogrammViewModel, value);
        }

        #region Commands

        private DelegateCommand saveCommand;
        public DelegateCommand SaveCommand {
            get {
                if(saveCommand == null) {
                    saveCommand = new DelegateCommand(
                        () => {
                            RemoteClientEditModel.Commit();
                            OnClose?.Invoke(this, new RemoteClientEditorCloseEventArgs(true));
                        },
                        () => RemoteClientEditModel.CanCommit
                    );
                    saveCommand.CanExecuteChangedWith(RemoteClientEditModel, x => x.CanCommit);
                }
                return saveCommand;
            }
        }

        private DelegateCommand closeCommand;
        public DelegateCommand CloseCommand {
            get {
                if(closeCommand == null) {
                    closeCommand = new DelegateCommand(
                        () => OnClose?.Invoke(this, new RemoteClientEditorCloseEventArgs(false)),
                        () => true
                    );
                }
                return closeCommand;
            }
        }

        private DelegateCommand selectCyclogrammFileCommand;
        public DelegateCommand SelectCyclogrammFileCommand {
            get {
                if(selectCyclogrammFileCommand == null) {
                    selectCyclogrammFileCommand = new DelegateCommand(
                        () => {
                            //FIXME убрать зависимоть от диалога
                            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                            dlg.FileName = "Cyclogramm";
                            dlg.DefaultExt = ".cyc";
                            dlg.Filter = "Unconverted cyclogramms (.csv)|*.csv";
                            if(dlg.ShowDialog() == true) {
                                RemoteClientEditModel.UnconvertedCyclogrammFile = dlg.FileName;
                            }
                        },
                        () => true
                    );
                }
                return selectCyclogrammFileCommand;
            }
        }   
        
        #endregion Commands
    }
}
