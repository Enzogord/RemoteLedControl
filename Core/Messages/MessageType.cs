namespace Core.Messages
{
    public enum MessageType : byte
    {
        NotSet = 0,

        //common
        State = 1,

        //to client
        SendServerIp = 5,
        RequestClientInfo = 6,

        //to server
        RequestServerIp = 101,
    }
}
