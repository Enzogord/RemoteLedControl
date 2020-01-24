using Core.Services.UserDialog;
using System.Windows;

namespace RLCServerApplication.Services
{
    public class UserDialogService : IUserDialogService
    {
        public UserDialogResult AskQuestion(string message, string title)
        {
            return AskQuestion(message, title, UserDialogActions.YesNo, UserDialogInformationLevel.Question);
        }

        public UserDialogResult AskQuestion(string message, string title, UserDialogInformationLevel informationLevel)
        {
            return AskQuestion(message, title, UserDialogActions.YesNo, informationLevel);
        }

        public UserDialogResult AskQuestion(string message, string title, UserDialogActions actions)
        {
            return AskQuestion(message, title, actions, UserDialogInformationLevel.Question);
        }

        public UserDialogResult AskQuestion(string message, string title, UserDialogActions actions, UserDialogInformationLevel informationLevel)
        {
            MessageBoxResult result = MessageBox.Show(message, title, GetMessageBoxButtons(actions), GetMessageBoxImage(informationLevel));
            switch(result) {
                case MessageBoxResult.OK:
                    return UserDialogResult.Ok;
                case MessageBoxResult.Yes:
                    return UserDialogResult.Yes;
                case MessageBoxResult.No:
                    return UserDialogResult.No;
                case MessageBoxResult.Cancel:
                default:
                    return UserDialogResult.Cancel;
            }
        }

        public void ShowMessage(string message, string title)
        {
            ShowMessage(message, title, UserDialogInformationLevel.Info);
        }

        public void ShowMessage(string message, string title, UserDialogInformationLevel informationLevel)
        {            
            MessageBox.Show(message, title, MessageBoxButton.OK, GetMessageBoxImage(informationLevel));
        }

        private MessageBoxImage GetMessageBoxImage(UserDialogInformationLevel informationLevel)
        {
            switch(informationLevel) {
                case UserDialogInformationLevel.Warning:
                    return MessageBoxImage.Warning;
                case UserDialogInformationLevel.Error:
                    return MessageBoxImage.Error;
                case UserDialogInformationLevel.Question:
                    return MessageBoxImage.Question;
                case UserDialogInformationLevel.Info:
                default:
                    return MessageBoxImage.Information;
            }
        }

        private MessageBoxButton GetMessageBoxButtons(UserDialogActions userActions)
        {
            switch(userActions) {                
                case UserDialogActions.OkCancel:
                    return MessageBoxButton.OKCancel;
                case UserDialogActions.YesNoCancel:
                    return MessageBoxButton.YesNoCancel;
                case UserDialogActions.YesNo:
                default:
                    return MessageBoxButton.YesNo;
            }
        }
    }
}
