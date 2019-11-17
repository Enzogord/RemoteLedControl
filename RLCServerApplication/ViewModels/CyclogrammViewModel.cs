using Core.IO;
using RLCCore.Domain;
using RLCServerApplication.Infrastructure;
using RLCServerApplication.Infrastructure.Command;
using System;
using System.IO;

namespace RLCServerApplication.ViewModels
{
    public class CyclogrammViewModel : ViewModelBase
    {
        private readonly RemoteClient client;
        private readonly SaveController saveController;
        public Cyclogramm Cyclogramm { get; }

        public CyclogrammViewModel(Cyclogramm cyclogramm, RemoteClient client, SaveController saveController)
        {
            Cyclogramm = cyclogramm ?? throw new ArgumentNullException(nameof(cyclogramm));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.saveController = saveController ?? throw new ArgumentNullException(nameof(saveController));
            Editable = true;
            CreateCommands();
        }

        private bool editable;
        public bool Editable {
            get => editable;
            private set => SetField(ref editable, value, () => Editable);
        }

        public bool ConvertedCyclogrammExists {
            get {
                string clientPath = saveController.GetClientFolder(client.Number, client.Name);
                return File.Exists(Path.Combine(clientPath, "Data.cyc"));
            }
        }

        #region Commands

        public DelegateCommand SelectCyclogrammFile { get; private set; }

        private void CreateCommands()
        {
            SelectCyclogrammFile = new DelegateCommand(
                () => {
                    //FIXME убрать зависимоть от диалога
                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                    dlg.FileName = "Cyclogramm";
                    dlg.DefaultExt = ".cyc";
                    dlg.Filter = "Unconverted cyclogramms (.csv)|*.csv";
                    if(dlg.ShowDialog() == true) {
                        Cyclogramm.FilePath = dlg.FileName;
                    }
                },
                () => Editable
            );
        }

        #endregion
    }
}
