using System.Runtime.Serialization;
using Service;

namespace RLCCore
{
    [DataContract]
    public class Cyclogramm : NotifyPropertyBase
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
