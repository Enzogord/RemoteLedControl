using Microsoft.Win32;
using System;
using System.Windows.Input;

namespace CyclogrammConverter
{
    public class SaveDialogCommand : ICommand
    {
        private readonly Action<string> saveFileAction;
        private readonly string fileFilter;

        public SaveDialogCommand(Action<string> saveFileAction, string fileFilter)
        {
            this.saveFileAction = saveFileAction;
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

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = fileFilter;
            dlg.CreatePrompt = true;
            if(dlg.ShowDialog() == true) {
                saveFileAction(dlg.FileName);
            }
        }
    }
}
