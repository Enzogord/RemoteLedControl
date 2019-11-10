using RLCCore.Domain;
using RLCServerApplication.Infrastructure;
using RLCServerApplication.Infrastructure.Command;
using System;
using System.IO;

namespace RLCServerApplication.ViewModels
{
    public class CyclogrammViewModel : ViewModelBase
    {
        public Cyclogramm Cyclogramm { get; }

        public CyclogrammViewModel(Cyclogramm cyclogramm)
        {
            Cyclogramm = cyclogramm ?? throw new ArgumentNullException(nameof(cyclogramm));
            Editable = true;
            CreateCommands();
        }

        private bool editable;
        private readonly RemoteClient client;

        public bool Editable {
            get => editable;
            private set => SetField(ref editable, value, () => Editable);
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
                        Cyclogramm.FileName = Path.GetFileNameWithoutExtension(dlg.FileName);
                    }
                },
                () => Editable
            );
        }

        #endregion
    }
}
