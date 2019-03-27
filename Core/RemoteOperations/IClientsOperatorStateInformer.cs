using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLCCore.RemoteOperations
{
    public interface IClientsOperatorStateInformer
    {
        event EventHandler<OperatorStateEventArgs> StateChanged;
    }
}
