using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLCCore.Transport
{
    public class MessageEventArgs : EventArgs
    {
        public IRemoteClientMessage Message { get; }

        public MessageEventArgs(IRemoteClientMessage message)
        {
            Message = message;
        }

    }
}
