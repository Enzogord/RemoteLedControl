namespace Core.ClientConnectionService
{
    public interface IConnectableClient : INumeredClient
    {
        IClientConnection Connection { get; set; }
    }
}
