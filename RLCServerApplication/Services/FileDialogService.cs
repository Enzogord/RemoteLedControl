using Core.Services.FileDialog;
using Microsoft.Win32;

namespace RLCServerApplication.Services
{
    public class FileDialogService : IOpenFileService, ISaveFileService
    {
        #region IOpenFileService implementation

        public string OpenFile(string filter, string title)
        {
            return OpenFile(filter, title, null, true, true);
        }

        public string OpenFile(string filter, string title, string initialDirectory)
        {
            return OpenFile(filter, title, initialDirectory, true, true);
        }

        public string OpenFile(string filter, string title, bool checkFileExists, bool checkPathExists)
        {
            return OpenFile(filter, title, null, checkFileExists, checkPathExists);
        }

        public string OpenFile(string filter, string title, string initialDirectory, bool checkFileExists, bool checkPathExists)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = title;
            dlg.Filter = filter;
            if(!string.IsNullOrWhiteSpace(initialDirectory)) {
                dlg.InitialDirectory = initialDirectory;
            }
            dlg.CheckFileExists = checkFileExists;
            dlg.CheckPathExists = checkPathExists;
            if(dlg.ShowDialog() == true) {
                return dlg.FileName;
            }
            return null;
        }

        #endregion

        #region ISaveFileService implementation

        public string SaveFile(string filter, string title, string defaultExt)
        {
            return SaveFile(filter, title, defaultExt, true, null, false, false);
        }        

        public string SaveFile(string filter, string title, string defaultExt, string initialDirectory)
        {
            return SaveFile(filter, title, defaultExt, true, initialDirectory, false, false);
        }

        public string SaveFile(string filter, string title, string defaultExt, bool createPrompt, string initialDirectory)
        {
            return SaveFile(filter, title, defaultExt, createPrompt, initialDirectory, false, false);
        }

        public string SaveFile(string filter, string title, string defaultExt, bool checkFileExists, bool checkPathExists)
        {
            return SaveFile(filter, title, defaultExt, true, null, checkFileExists, checkPathExists);
        }

        public string SaveFile(string filter, string title, string defaultExt, bool createPrompt, bool checkFileExists, bool checkPathExists)
        {
            return SaveFile(filter, title, defaultExt, createPrompt, null, checkFileExists, checkPathExists);
        }        

        public string SaveFile(string filter, string title, string defaultExt, string initialDirectory, bool checkFileExists, bool checkPathExists)
        {
            return SaveFile(filter, title, defaultExt, true, initialDirectory, checkFileExists, checkPathExists);
        }

        public string SaveFile(string filter, string title, string defaultExt, bool createPrompt, string initialDirectory, bool checkFileExists, bool checkPathExists)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = title;
            dlg.Filter = filter;
            dlg.DefaultExt = defaultExt;
            dlg.CreatePrompt = createPrompt;
            if(!string.IsNullOrWhiteSpace(initialDirectory)) {
                dlg.InitialDirectory = initialDirectory;
            }
            dlg.CheckFileExists = checkFileExists;
            dlg.CheckPathExists = checkPathExists;
            if(dlg.ShowDialog() == true) {
                return dlg.FileName;
            }
            return null;
        }

        #endregion

    }
}
