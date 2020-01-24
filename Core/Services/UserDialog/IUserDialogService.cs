namespace Core.Services.UserDialog
{
    public interface IUserDialogService
    {
        void ShowMessage(string message, string title);
        void ShowMessage(string message, string title, UserDialogInformationLevel informationLevel);
        UserDialogResult AskQuestion(string message, string title);
        UserDialogResult AskQuestion(string message, string title, UserDialogInformationLevel informationLevel);
        UserDialogResult AskQuestion(string message, string title, UserDialogActions actions);
        UserDialogResult AskQuestion(string message, string title, UserDialogActions actions, UserDialogInformationLevel informationLevel);
    }
}
