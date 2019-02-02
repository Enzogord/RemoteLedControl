using System.Runtime.Serialization;

namespace Core
{
    [DataContract]
    public class Pin
    {
        [DataMember]
        public byte PinNumber { get; set; }

        [DataMember]
        public ushort LEDCount { get; set; }

        public Pin(byte pin, ushort ledCount)
        {
            PinNumber = pin;
            LEDCount = ledCount;
        }
    }
}
