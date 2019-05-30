using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Messages;
using NLog;
using TCPCommunicationService;

namespace Core.ClientConnectionService
{
    public class RemoteClientConnectionService : TCPService, IRemoteClientConnectionService, IRemoteClientCommunication
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IConnectorMessageService connectorMessageService;
        private readonly Dictionary<INumeredClient, RemoteClientConnector> clientConnectors;
        private readonly RemoteClientIdentificator clientsIdentificator;

        public event EventHandler<ClientMessageEventArgs> OnReceiveMessage;

        public RemoteClientConnectionService(IPAddress address, int port, IConnectorMessageService connectorMessageService, int bufferSize, byte workersCount = 1) 
            : this(new IPEndPoint(address, port), connectorMessageService, bufferSize, workersCount)
        {
        }

        public RemoteClientConnectionService(IPEndPoint localIpEndPoint, IConnectorMessageService connectorMessageService, int bufferSize, byte workersCount = 1) 
            : base(localIpEndPoint, bufferSize, workersCount)
        {
            this.connectorMessageService = connectorMessageService ?? throw new ArgumentNullException(nameof(connectorMessageService));
            clientConnectors = new Dictionary<INumeredClient, RemoteClientConnector>();
            clientsIdentificator = new RemoteClientIdentificator(this.connectorMessageService);
            clientsIdentificator.OnClientIdentify += ClientsIdentificator_OnClientIdentify;
            logger.Debug("RemoteClientConnectionService started");
        }

        public void Send(INumeredClient client, RLCMessage message)
        {
            if(clientConnectors.ContainsKey(client)){
                clientConnectors[client].ClientListener.Write(message.ToArray());
            }
        }

        public void SendToAll(RLCMessage message)
        {
            foreach(var item in clientConnectors.Select(x => x.Value.ClientListener)) {
                Task.Run(() => { item.Write(message.ToArray()); });                
            }
        }

        private RLCMessage ParseMessage(byte[] packageBytes)
        {
            RLCMessage message = new RLCMessage();
            try {
                message.FromBytes(packageBytes);
                return message;
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка при распознавании сообщения");
                return null;
            }
        }

        protected override void OnClientConnected(TcpClientListener clientListener)
        {
            clientsIdentificator.AddNewClient(clientListener);
            base.OnClientConnected(clientListener);
        }

        private void ClientsIdentificator_OnClientIdentify(object sender, ClientIdentifiedEventArgs e)
        {
            AddIdentifiedClient(e.Client, e.Listener);
        }

        private void AddIdentifiedClient(INumeredClient client, TcpClientListener listener)
        {
            if(client == null || listener == null) {
                return;
            }

            EventHandler<DataEventArgs> OnReceiveData = (sender, e) => {
                RLCMessage message = ParseMessage(e.Data);
                if(message == null) {
                    return;
                }
                OnReceiveMessage?.Invoke(this, new ClientMessageEventArgs(client, message));
            };

            RemoteClientConnector connector = new RemoteClientConnector(listener);
            if(!clientConnectors.ContainsKey(client)) {
                clientConnectors.Add(client, connector);
            }
            connector.OnDisconnected += (sender, e) => {
                if(clientConnectors.ContainsKey(client)) {
                    logger.Debug("Disconnect RemoteClientConnector");
                    clientConnectors.Remove(client);
                }
            };
            IConnectableClient connectableClient = connectorMessageService.Clients.FirstOrDefault(x => x.Number == client.Number);
            if(connectableClient != null) {
                connectableClient.Connection = new RemoteClientConnection(connector);
            }
        }

        public override void Dispose()
        {
            clientsIdentificator.Clear();
            clientConnectors.Clear();
            base.Dispose();
        }
    }
}
