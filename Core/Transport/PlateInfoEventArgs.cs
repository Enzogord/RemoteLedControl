using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class PlateInfoEventArgs : EventArgs
    {
        public byte ClientNumber { get; private set; }
        public IPEndPoint IpEndPoint { get; private set; }
        public ClientState ClientState { get; private set; }

        public PlateInfoEventArgs(byte clientNumber, IPEndPoint ipEndPoint, ClientState clientState)
        {
            ClientNumber = clientNumber;
            IpEndPoint = ipEndPoint;
            ClientState = clientState;
        }
    }
}
