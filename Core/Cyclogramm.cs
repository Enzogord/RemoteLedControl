using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Информация о циклограмме воспроизводимой клиентом
    /// </summary>
    [DataContract]
    public class Cyclogramm
    {
        // Fields
        [DataMember]
        private string FName;
        [DataMember]
        public Client Parent;
        [DataMember]
        public int FileSize { get; set; }
        //private string FOutputFileName;
        [DataMember]
        private bool FConverted;
        [DataMember]
        private string FConvertedStr;
        private bool FSaved;

        // Properties
        public string Name
        {
            get { return FName; }
            set
            {
                FName = value;
                OnChange?.Invoke();
            }
        }

        public bool Saved
        {
            get { return FSaved; }
            set
            {
                FSaved = value;
                if (FSaved == false)
                {
                    Parent.Saved = false;
                }
            }
        }
        public bool Converted
        {
            get { return FConverted; }
            set
            {
                FConverted = value;
                if (FConverted == true)
                {
                    FConvertedStr = "Да";
                }
                else
                {
                    FConvertedStr = "Нет";
                }
                OnConverted?.Invoke();
            }
        }
        public string ConvertedStr
        {
            get { return FConvertedStr; }
            set
            {
                FConvertedStr = value;
                if (FConverted == true)
                {
                    FConvertedStr = "Да";
                }
                else
                {
                    if (FConvertedStr == "")
                    {
                        FConvertedStr = "Нет";
                    }
                }
                OnConverted?.Invoke();
            }
        }

        // Events
        public delegate void ChangeCyclogram();
        public event ChangeCyclogram OnChange;
        public delegate void ConvertedCyclogramm();
        public event ConvertedCyclogramm OnConverted;
    }

}
