namespace RLCCore.Transport
{
    public interface IRemoteClientMessage
    {
        int ProjectKey { get; }
        int ClientNumber { get; }
        ClientMessages MessageType { get; }
        byte[] Data { get; }
    }
}