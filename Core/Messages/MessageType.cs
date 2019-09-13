namespace Core.Messages
{
    public enum MessageType : byte
    {
        NotSet = 0,
        //to client
        Play = 1,
        Stop = 2,
        Pause = 3,
        PlayFrom = 4,
        SendServerIp = 5,
        RequestClientInfo = 6,
        Rewind = 7,

        //to server
        ClientInfo = 100,
        RequestServerIp = 101

    }
}
