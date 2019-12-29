using Core.Infrastructure;
using Microcontrollers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Core.Domain
{
    [DataContract]
    public class MicrocontrollerUnit : ValidatableNotifierObjectBase
    {
        private string microcontrollerId;
        [DataMember]
        public string MicrocontrollerId {
            get => microcontrollerId;
            set {
                if(SetField(ref microcontrollerId, value)) {
                    if(MicrocontrollersLibrary.TryGetMicrocontroller(microcontrollerId, out IMicrocontroller mc)) {
                        Microcontroller = mc;
                    }
                }
            }
        }

        private IMicrocontroller microcontroller;
        public IMicrocontroller Microcontroller {
            get => microcontroller;
            set {
                if(ValidatableSetField(ref microcontroller, value)) {
                    if(microcontroller != null) {
                        microcontrollerId = microcontroller.Id;
                    }
                }
            }
        }

        private int? zeroCharge;
        [DataMember]
        public int ZeroCharge {
            get => zeroCharge ?? Microcontroller?.DefaultZeroChargeLevel ?? 0;
            set {
                int val = value;
                if(value >= 4498) {
                    val = 4498;
                }
                if(value > FullCharge) {
                    val = FullCharge - 1;
                }
                if(value <= 0) {
                    val = 0;
                }
                SetField(ref zeroCharge, val);
            }
        }

        private int? fullCharge;
        [DataMember]
        public int FullCharge {
            get => fullCharge ?? Microcontroller?.DefaultFullChargeLevel ?? 4500;
            set {
                int val = value;
                if(value >= 4500) {
                    val = 4500;
                }
                if(value <= 2) {
                    val = 2;
                }
                if(value < ZeroCharge) {
                    val = ZeroCharge + 1;
                }
                SetField(ref fullCharge, val);
            }
        }

        public MicrocontrollerUnit()
        {
            zeroCharge = 0;
            fullCharge = 4290;
        }

        protected override IEnumerable<ValidationResult> ValidateWithDataErrorNotification(ValidationContext validationContext)
        {
            if(Microcontroller == null) {
                yield return new PropertyValidationResult("Необходимо выбрать микроконтроллер", nameof(Microcontroller));
            }
        }
    }
}
