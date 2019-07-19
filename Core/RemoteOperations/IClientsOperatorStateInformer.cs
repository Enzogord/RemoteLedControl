using System;

namespace RLCCore.RemoteOperations
{
    public interface IClientsOperatorStateInformer
    {
        event EventHandler<OperatorStateEventArgs> StateChanged;
    }
}
