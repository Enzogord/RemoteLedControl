using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ClientConnectionService;
using Core.Messages;
using NLog;
using RLCCore;

namespace Core.ClientConnectionService
{ 
    public class ConnectorMessageService : IConnectorMessageService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly uint key;

        public IEnumerable<IConnectableClient> Clients { get; }

        public ConnectorMessageService(uint key, IEnumerable<IConnectableClient> clients)
        {            
            if(clients == null) {
                throw new ArgumentNullException(nameof(clients));
            }

            if(clients.GroupBy(x => x.Number).Where(g => g.Count() > 1).Any()) {
                throw new ArgumentException($"В коллекции {nameof(clients)} не должно быть повторяющихся клиентов");
            }            
            this.key = key;
            Clients = clients;
        }

        public RLCMessage CreateRequestClientInfoMessage()
        {
            return RLCMessageFactory.RequestClientInfo(key);
        }

        public bool TryParseClient(byte[] data, out INumeredClient clientNumber)
        {
            RLCMessage message = ParseMessage(data);
            clientNumber = Clients.FirstOrDefault(x => x.Number == message.ClientNumber);
            return clientNumber != null;
        }

        private RLCMessage ParseMessage(byte[] messageData)
        {
            RLCMessage message = new RLCMessage();
            try {
                message.FromBytes(messageData);
                return message;
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка при распознавании сообщения");
                return null;
            }
        }
    }
}
