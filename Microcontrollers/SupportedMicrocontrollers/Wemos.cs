namespace Microcontrollers.SupportedMicrocontrollers
{
    public static class Wemos
    {
        public static Microcontroller D1ProMiniv110 => WemosD1ProMini.Instance;
    }

    public class WemosD1ProMini : Microcontroller
    {
        private static WemosD1ProMini instance;
        public static WemosD1ProMini Instance {
            get {
                if(instance == null) {
                    instance = new WemosD1ProMini();
                }
                return instance;
            }
        }

        public override MicrocontrollerType Type => MicrocontrollerType.Esp8266;

        public override string Model => "Wemos D1 Pro mini";

        public override string Version => "1.1.0";

        private WemosD1ProMini()
        {
            PinList.Add(new MicrocontrollerPin(5, 1, "D1"));
            PinList.Add(new MicrocontrollerPin(4, 2, "D2"));
            PinList.Add(new MicrocontrollerPin(0, 3, "D3"));
            PinList.Add(new MicrocontrollerPin(2, 4, "D4"));
        }
    }
}
