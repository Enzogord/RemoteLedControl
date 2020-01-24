using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.FileDialog
{
    public interface IOpenFileService
    {
        string OpenFile(string filter, string title);
        string OpenFile(string filter, string title, string initialDirectory);
        string OpenFile(string filter, string title, bool checkFileExists, bool checkPathExists);
        string OpenFile(string filter, string title, string initialDirectory, bool checkFileExists, bool checkPathExists);
    }
}
