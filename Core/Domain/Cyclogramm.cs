using System.IO;
using System.Runtime.Serialization;
using NotifiedObjectsFramework;

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

        private string fileName;
        [DataMember]
        public string FileName {
            get => fileName;
            set {
                SetField(ref fileName, value);
                OnPropertyChanged(nameof(CyclogrammFileInfo));
            }
        }

        private string filePath;
        public string FilePath {
            get => filePath;
            set => SetField(ref filePath, value, () => FilePath);
        }

        private FileInfo cyclogrammFileInfo;
        public FileInfo CyclogrammFileInfo {
            get {
                if(cyclogrammFileInfo == null || cyclogrammFileInfo.FullName != FileName) {
                    cyclogrammFileInfo = new FileInfo(FileName);
                }
                cyclogrammFileInfo.Refresh();
                return cyclogrammFileInfo;
            }
        }
    }
}
