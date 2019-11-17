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

        private string filePath;
        public string FilePath {
            get => filePath;
            set => SetField(ref filePath, value, () => FilePath);
        }
    }
}
