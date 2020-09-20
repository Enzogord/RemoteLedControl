namespace Core.Messages
{
    public enum MessageType : byte
    {
        NotSet = 0,
        //to client
        State = 1,
        SendServerIp = 5,
        RequestClientInfo = 6,

        //to server
        ClientInfo = 100,
        RequestServerIp = 101,
        BatteryCharge = 102
    }
}
