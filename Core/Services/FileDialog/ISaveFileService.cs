using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.FileDialog
{
    public interface ISaveFileService
    {
        string SaveFile(string filter, string title, string defaultExt);
        string SaveFile(string filter, string title, string defaultExt, bool createPrompt, string initialDirectory);
        string SaveFile(string filter, string title, string defaultExt, string initialDirectory);
        string SaveFile(string filter, string title, string defaultExt, bool checkFileExists, bool checkPathExists);
        string SaveFile(string filter, string title, string defaultExt, bool createPrompt, bool checkFileExists, bool checkPathExists);
        string SaveFile(string filter, string title, string defaultExt, string initialDirectory, bool checkFileExists, bool checkPathExists);
        string SaveFile(string filter, string title, string defaultExt, bool createPrompt, string initialDirectory, bool checkFileExists, bool checkPathExists);
    }
}
