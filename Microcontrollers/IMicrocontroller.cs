using System.Collections.Generic;

namespace Microcontrollers
{
    public interface IMicrocontroller
    {
        string Id { get; }
        MicrocontrollerType Type { get; }
        string Model { get; }
        string Version { get; }
        string Name { get; }
        IEnumerable<MicrocontrollerPin> Pins { get; }
        int DefaultZeroChargeLevel { get; }
        int DefaultFullChargeLevel { get; }
    }
}
