using System;

namespace RLCServerApplication.Infrastructure
{ 
    public sealed class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute)
            : base(x => execute(), x => true)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute) : base(x => execute(), x => canExecute())
        {
        }
    }

    public class RelayCommand<T> : IRelayCommand<T>
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;
        
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _execute((T)parameter);    
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}