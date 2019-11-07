using Microsoft.Win32;
using System;
using System.Windows.Input;

namespace CyclogrammConverter
{
    public class OpenDialogCommand : ICommand
    {
        private readonly Action<string> openFileAction;
        private readonly string fileFilter;

        public OpenDialogCommand(Action<string> openFileAction, string fileFilter)
        {
            this.openFileAction = openFileAction;
            this.fileFilter = fileFilter;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if(!CanExecute(parameter)) {
                return;
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = fileFilter;
            if(dlg.ShowDialog() == true) {
                openFileAction(dlg.FileName);
            }
        }
    }
}
