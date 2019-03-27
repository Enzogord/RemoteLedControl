using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLCCore.Transport
{
    public class ClientMessageParser
    {
        public IRemoteClientMessage ParseMessage(byte[] package)
        {
            if(package.Length != 200) {
                throw new ArgumentException("Невозможно спарсить пакет, длина пакета не соответствует стандарту");
            }
            RemoteClientMessage message = new RemoteClientMessage();

            message.ProjectKey = ((package[0] << 24) + (package[1] << 16) + (package[2] << 8) + (package[3] << 0));

            try {
                message.MessageType = (ClientMessages)package[4];
            }
            catch(Exception ex) {
                throw new ArgumentException($"Невозможно спарсить пакет, тип сообщения неизвестен: {package[4]}.", ex);
            }
            message.ClientNumber = package[5];

            ushort contentLength = (ushort)((package[6] << 8) + (package[7] << 0));
            message.Data = new byte[contentLength];
            Array.Copy(package, 8, message.Data, 0, contentLength);

            return message;
        }
    }
}
