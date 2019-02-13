using Service;
using System.Runtime.Serialization;

namespace Core
{
    [DataContract]
    public class Pin : NotifyPropertyBase
    {
        private byte pinNumber;
        [DataMember]
        public byte PinNumber {
            get => pinNumber;
            set => SetField(ref pinNumber, value, () => PinNumber);
        }

        private int ledCount;
        [DataMember]
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
