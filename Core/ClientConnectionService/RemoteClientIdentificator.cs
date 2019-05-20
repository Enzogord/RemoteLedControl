using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Messages;
using TCPCommunicationService;

namespace Core.ClientConnectionService
{
    public class RemoteClientIdentificator : IRemoteClientIdentificator
    {
        private int identificationTimeout = 10000;
        private Dictionary<TcpClientListener, int> unidentifiedClients;
        private readonly IConnectorMessageService messageService;

        public RemoteClientIdentificator(IConnectorMessageService messageService)
        {
            if(messageService == null) {
                throw new ArgumentNullException(nameof(messageService));
            }

            unidentifiedClients = new Dictionary<TcpClientListener, int>();
            this.messageService = messageService;
        }

        public void AddNewClient(TcpClientListener clientListener)
        {
            if(clientListener == null || unidentifiedClients.ContainsKey(clientListener)) {
                return;
            }
            unidentifiedClients.Add(clientListener, 0);
            StartClientIdentification(clientListener);
        }

        public void RemoveClient(TcpClientListener clientListener)
        {
            if(unidentifiedClients.ContainsKey(clientListener)) {
                unidentifiedClients.Remove(clientListener);
            }
        }

        private void StartClientIdentification(TcpClientListener clientListener)
        {
            INumeredClient client = null;
            RLCMessage requestMessage = messageService.CreateRequestClientInfoMessage();

            clientListener.OnReceiveData += OnReceiveClientData;
            void OnReceiveClientData(object sender, DataEventArgs e)
            {
                if(messageService.TryParseClient(e.Data, out client)) {
                    clientListener.OnReceiveData -= OnReceiveClientData;
                }
            }

            //отправка запроса с ожиданием номера клиента, в течении identificationTimeout времени
            CancellationTokenSource timeoutCancelation = new CancellationTokenSource(identificationTimeout);
            Task.Run(() => {
                clientListener.Write(requestMessage.ToArray());
                while(!timeoutCancelation.IsCancellationRequested) {
                    if(client != null) {
                        OnClientIdentify?.Invoke(this, new ClientIdentifiedEventArgs(client, clientListener));
                        return;
                    }
                }
                RemoveClient(clientListener);
            });
        }

        #region IRemoteClientIdentificator

        public event EventHandler<ClientIdentifiedEventArgs> OnClientIdentify;

        #endregion IRemoteClientIdentificator
    }
}
