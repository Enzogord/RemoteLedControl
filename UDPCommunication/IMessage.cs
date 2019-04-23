﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPCommunication
{
    public interface IUdpMessage
    {
        byte[] ToArray();
        void FromBytes(byte[] bytes);
    }
}
