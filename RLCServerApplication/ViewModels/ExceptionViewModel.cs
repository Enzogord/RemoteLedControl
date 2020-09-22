using RLCServerApplication.Infrastructure;
using RLCServerApplication.Infrastructure.Command;
using System;

namespace RLCServerApplication.ViewModels
{
    public class ExceptionViewModel : ViewModelBase
    {
        public ExceptionViewModel(Exception exception)
        {
            if(exception is null) {
                throw new ArgumentNullException(nameof(exception));
            }

            ExceptionMessage = exception.ToString();
        }

        private string exceptionMessage;
        public string ExceptionMessage {
            get => exceptionMessage;
            set => SetField(ref exceptionMessage, value);
        }


        public event EventHandler Close;

        #region CloseCommand

        private DelegateCommand closeCommand;
        public DelegateCommand CloseCommand {
            get {
                if(closeCommand == null) {
                    closeCommand = new DelegateCommand(() => Close?.Invoke(this, EventArgs.Empty));
                }
                return closeCommand;
            }
        }

        #endregion CloseCommand	
    }
}
