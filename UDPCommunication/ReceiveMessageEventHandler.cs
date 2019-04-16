﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UDPCommunication
{
    public delegate void ReceiveMessageEventHandler<TEventArgs>(object sender, IPEndPoint endPoint, TEventArgs e);
}