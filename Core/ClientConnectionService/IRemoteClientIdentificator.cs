using System;

namespace Core.ClientConnectionService
{
    public interface IRemoteClientIdentificator
    {
        event EventHandler<ClientIdentifiedEventArgs> OnClientIdentify; 
    }
}
