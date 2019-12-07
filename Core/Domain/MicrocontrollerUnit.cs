using Microcontrollers;
using NotifiedObjectsFramework;
using System.Runtime.Serialization;

namespace Core.Domain
{
    [DataContract]
    public class MicrocontrollerUnit : NotifyPropertyChangedBase
    {
        private string microcontrollerId;
        [DataMember]
        public string MicrocontrollerId {
            get => microcontrollerId;
            set {
                if(SetField(ref microcontrollerId, value, () => MicrocontrollerId)) {
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
                if(SetField(ref microcontroller, value, () => Microcontroller)) {
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
    }
}
