using Service;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace RLCCore.Domain
{
    [DataContract]
    public class Pin : NotifyPropertyBase
    {
        private byte pinNumber;
        [DataMember]
        [Display(Name = "Номер пина")]
        public byte PinNumber {
            get => pinNumber;
            set => SetField(ref pinNumber, value, () => PinNumber);
        }

        private int ledCount;
        [DataMember]
        [Display(Name = "Количество светодиодов")]
        public int LEDCount {
            get => ledCount;
            set => SetField(ref ledCount, value, () => LEDCount);
        }


        public Pin(byte pin, ushort ledCount)
        {
            PinNumber = pin;
            LEDCount = ledCount;
        }
    }
}
