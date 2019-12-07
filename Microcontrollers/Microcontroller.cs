using System.Collections.Generic;

namespace Microcontrollers
{
    public abstract class Microcontroller : IMicrocontroller
    {
        public string Id => $"{Type}_{Model}_{Version}";

        public abstract MicrocontrollerType Type { get; }

        public abstract string Model { get; }

        public abstract string Version { get; }

        public string Name => $"{Model} v{Version}";

        public List<MicrocontrollerPin> PinList { get; set; } = new List<MicrocontrollerPin>();

        public IEnumerable<MicrocontrollerPin> Pins => PinList;

        public virtual int DefaultZeroChargeLevel => 0;

        public virtual int DefaultFullChargeLevel => 4290;
    }
}
