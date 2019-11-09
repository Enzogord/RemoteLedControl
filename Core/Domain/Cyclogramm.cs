using System.IO;
using System.Runtime.Serialization;
using NotifiedObjectsFramework;
using Service;

namespace RLCCore.Domain
{
    [DataContract]
    public class Cyclogramm : NotifyPropertyChangedBase
    {
        [DataMember]
        private string name;
        public string Name {
            get => name;
            set => SetField(ref name, value);
        }

        private string filePath;        
        public string FilePath {
            get => filePath;
            set {
                SetField(ref filePath, value);
                OnPropertyChanged(nameof(CyclogrammFileInfo));
            }
        }

        private FileInfo cyclogrammFileInfo;
        public FileInfo CyclogrammFileInfo {
            get {
                if(cyclogrammFileInfo == null || cyclogrammFileInfo.FullName != FilePath) {
                    cyclogrammFileInfo = new FileInfo(FilePath);
                }
                cyclogrammFileInfo.Refresh();
                return cyclogrammFileInfo;
            }
        }
    }
}
