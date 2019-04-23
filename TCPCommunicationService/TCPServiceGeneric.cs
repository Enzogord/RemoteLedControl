using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace TCPCommunicationService
{
    public class TCPService<TMessage> : TCPService
        where TMessage : class, ITcpMessage, new()
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public TCPService(IPEndPoint localIpEndPoint, IEnumerable<IRemoteClient> expectedClients, int bufferSize, byte workersCount = 1) : base(localIpEndPoint, expectedClients, bufferSize, workersCount)
        {
        }

        public void Send(IPAddress address, TMessage message)
        {
            Send(address, message.ToArray());
        }

        public void SendToAll(TMessage message)
        {
            SendToAll(message.ToArray());
        }

        public event ReceiveMessageEventHandler<TMessage> OnReceiveMessage;

        protected override void PackageReceived(IPEndPoint ipEndPoint, byte[] buffer)
        {
            TMessage message = ParseMessage(buffer);
            OnReceiveMessage?.Invoke(this, ipEndPoint, message);
        }

        private TMessage ParseMessage(byte[] packageBytes)
        {
            TMessage message = new TMessage();
            try {
                message.FromBytes(packageBytes);
                return message;
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка при распознавании сообщения");
                return null;
            }
        }
    }
}
