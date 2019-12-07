using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcontrollers
{
    public class MicrocontrollerPin
    {
        public int GPIO { get; set; }
        public int Pin { get; set; }
        public string Name { get; set; }

        public MicrocontrollerPin(int hardPin, int userPin, string name)
        {
            GPIO = hardPin;
            Pin = userPin;
            Name = name;
        }
    }
}
