using System;
using System.Windows.Input;

namespace CyclogrammConverter
{
    public class Command : ICommand
    {
        private readonly Action executeAction;
        private readonly Func<bool> canExecuteAction;

        public Command(Action executeAction, Func<bool> canExecuteAction)
        {
            this.executeAction = executeAction;
            this.canExecuteAction = canExecuteAction;
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteAction();
        }

        public void Execute(object parameter)
        {
            if(!CanExecute(parameter)) {
                return;
            }
            executeAction.Invoke();
        }
    }
}
