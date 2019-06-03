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
            this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            unidentifiedClients = new Dictionary<TcpClientListener, int>();
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
            clientListener.OnDisconnected += OnDisconnected;
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

            void OnDisconnected(object sender, EventArgs e)
            {
                timeoutCancelation.Cancel();
                clientListener.OnDisconnected -= OnDisconnected;
            }
        }

        #region IRemoteClientIdentificator

        public event EventHandler<ClientIdentifiedEventArgs> OnClientIdentify;

        #endregion IRemoteClientIdentificator

        public void Clear()
        {
            unidentifiedClients.Clear();
        }
    }
}
