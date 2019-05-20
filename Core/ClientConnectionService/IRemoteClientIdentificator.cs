using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPCommunicationService;

namespace Core.ClientConnectionService
{
    public interface IRemoteClientIdentificator
    {
        event EventHandler<ClientIdentifiedEventArgs> OnClientIdentify; 
    }
}
