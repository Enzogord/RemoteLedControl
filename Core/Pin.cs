using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    ///  Информация о пине используемом на плате клиента
    /// </summary>
    [DataContract]
    public class Pin
    {
        // Fields
        [DataMember]
        private byte PPinNumber;
        [DataMember]
        private ushort PLEDCount;

        // Propeties
        public byte PinNumber
        {
            get { return PPinNumber; }
            set
            {
                PPinNumber = value;
                OnChange?.Invoke();
            }
        }
        public ushort LEDCount
        {
            get { return PLEDCount; }
            set
            {
                PLEDCount = value;
                OnChange?.Invoke();
            }
        }

        // Events
        public delegate void Change();
        public event Change OnChange;

        // Methods
        public Pin(string _Pin, string _LEDCount)
        {
            byte TmpPin;
            if (!byte.TryParse(_Pin, out TmpPin))
            {
                return;
            }
            ushort TmpLEDCount;
            if (!ushort.TryParse(_LEDCount, out TmpLEDCount))
            {
                return;
            }
            PinNumber = TmpPin;
            LEDCount = TmpLEDCount;
        }

    }

}
