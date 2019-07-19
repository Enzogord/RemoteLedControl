using System;

namespace RLCCore.RemoteOperations
{
    public class OperatorStateEventArgs : EventArgs
    {
        public OperatorStates CurrentState { get; private set; }

        public OperatorStateEventArgs(OperatorStates currentState)
        {
            CurrentState = currentState;
        }
    }
}
