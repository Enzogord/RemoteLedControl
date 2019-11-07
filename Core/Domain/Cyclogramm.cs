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

        private string fileFullName;        
        public string FileFullName {
            get => fileFullName;
            set {
                SetField(ref fileFullName, value);
                OnPropertyChanged(nameof(CyclogrammFileInfo));
            }
        }

        private FileInfo cyclogrammFileInfo;
        public FileInfo CyclogrammFileInfo {
            get {
                if(cyclogrammFileInfo == null || cyclogrammFileInfo.FullName != FileFullName) {
                    cyclogrammFileInfo = new FileInfo(FileFullName);
                }
                cyclogrammFileInfo.Refresh();
                return cyclogrammFileInfo;
            }
        }
    }
}
