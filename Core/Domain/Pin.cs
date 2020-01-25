using NotifiedObjectsFramework;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace RLCCore.Domain
{
    [DataContract]
    public class Pin : NotifyPropertyChangedBase
    {
        private byte pinNumber;
        [DataMember]
        [Display(Name = "Номер пина")]
        public byte PinNumber {
            get => pinNumber;
            set => SetField(ref pinNumber, value);
        }

        private int ledCount;
        [DataMember]
        [Display(Name = "Количество светодиодов")]
        public int LEDCount {
            get => ledCount;
            set => SetField(ref ledCount, value);
        }

        public Pin(byte pin, ushort ledCount)
        {
            PinNumber = pin;
            LEDCount = ledCount;
        }
    }
}
