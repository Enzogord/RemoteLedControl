using Core.Domain;
using Microcontrollers;
using NotifiedObjectsFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLCServerApplication.ViewModels
{
    public class MicrocontrollerUnitViewModel : NotifyPropertyChangedBase
    {
        public MicrocontrollerUnitViewModel(MicrocontrollerUnit microcontrollerUnit)
        {
            MicrocontrollerUnit = microcontrollerUnit ?? throw new ArgumentNullException(nameof(microcontrollerUnit));
            AvailableMicrocontrollers = MicrocontrollersLibrary.GetMicrocontrollers();
        }

        public MicrocontrollerUnit MicrocontrollerUnit { get; }

        public IEnumerable<IMicrocontroller> AvailableMicrocontrollers { get; }
    }
}
