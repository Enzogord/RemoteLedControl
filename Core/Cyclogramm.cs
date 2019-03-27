using System.Runtime.Serialization;

namespace RLCCore
{
    [DataContract]
    public class Cyclogramm
    {
        [DataMember]
        private string Name { get; set; }

        [DataMember]
        public RemoteClient Parent;

        [DataMember]
        public int FileSize { get; set; }

        [DataMember]
        public bool Converted { get; set; }
    }

}
