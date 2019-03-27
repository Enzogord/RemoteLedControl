namespace RLCCore
{
    public interface IPin
    {
        int LEDCount { get; set; }
        byte PinNumber { get; set; }
    }
}