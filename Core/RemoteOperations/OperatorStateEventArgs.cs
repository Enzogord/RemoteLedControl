﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
