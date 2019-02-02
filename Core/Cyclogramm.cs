using System.Runtime.Serialization;

namespace Core
{
    [DataContract]
    public class Cyclogramm
    {
        [DataMember]
        private string Name { get; set; }

        [DataMember]
        public Client Parent;

        [DataMember]
        public int FileSize { get; set; }

        [DataMember]
        public bool Converted { get; set; }

        public string ConvertedStr => Converted ? "Да" : "Нет";

        private bool saved;

        public bool Saved
        {
            get { return saved; }
            set
            {
                saved = value;
                if (saved == false)
                {
                    Parent.Saved = false;
                }
            }
        }

    }

}
