using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLCCore;
using RLCServerApplication.Infrastructure;

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
        public bool Editable {
            get => editable;
            private set => SetField(ref editable, value, () => Editable);
        }

        #region Commands

        public RelayCommand SelectCyclogrammFile { get; private set; }

        private void CreateCommands()
        {
            SelectCyclogrammFile = new RelayCommand(
                () => {
                    //FIXME убрать зависимоть от диалога
                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                    dlg.FileName = "Cyclogramm";
                    dlg.DefaultExt = ".cyc";
                    dlg.Filter = "CSV unconverted cyclogramms (.csv)|*.csv|Cyc converted cyclogramms (.cyc)|*.cyc";
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
