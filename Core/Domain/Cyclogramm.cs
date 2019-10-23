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
            set => SetField(ref name, value, () => Name);
        }

        [DataMember]
        private int fileSize;
        public int FileSize {
            get => fileSize;
            set => SetField(ref fileSize, value, () => FileSize);
        }

        private string filePath;
        public string FilePath {
            get => filePath;
            set => SetField(ref filePath, value, () => FilePath);
        }
    }

}
