﻿using System.Collections.Generic;
using Core.Messages;

namespace Core.ClientConnectionService
{
    public interface IConnectorMessageService
    {
        IEnumerable<IConnectableClient> Clients { get; }
        RLCMessage CreateRequestClientInfoMessage();
        bool TryParseClient(byte[] data, out INumeredClient clientNumber);
    }
}
