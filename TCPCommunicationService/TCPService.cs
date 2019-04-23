using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace TCPCommunicationService
{
    public delegate void ReceivePackageEventHandler(IPEndPoint ipEndPoint, byte[] package);

    public class TCPService : IDisposable
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        private CancellationTokenSource cts = new CancellationTokenSource();
        private ConcurrentDictionary<IPAddress, TcpClientListener> clientListeners;
        private BlockingCollection<TcpClientListener> listenersQueue;
        private readonly IPEndPoint localIpEndPoint;
        private readonly IEnumerable<IRemoteClient> expectedClients;
        private readonly int bufferSize;
        private readonly byte workersCount;

        public bool IsActive { get; private set; }

        public TCPService(IPEndPoint localIpEndPoint, IEnumerable<IRemoteClient> expectedClients, int bufferSize, byte workersCount = 1)
        {
            if(localIpEndPoint == null) {
                throw new ArgumentNullException(nameof(localIpEndPoint));
            }
            if(expectedClients == null) {
                throw new ArgumentNullException(nameof(expectedClients));
            }
            if(expectedClients.GroupBy(x => x.IPAddress).Where(g => g.Count() > 1).Any()) {
                throw new ArgumentException($"В коллекции {nameof(expectedClients)} не должно быть повторяющихся адресов");
            }

            listenersQueue = new BlockingCollection<TcpClientListener>();            

            this.localIpEndPoint = localIpEndPoint;
            this.expectedClients = expectedClients;
            this.bufferSize = bufferSize;
            this.workersCount = workersCount;
            clientListeners = new ConcurrentDictionary<IPAddress, TcpClientListener>();
            foreach(var ec in expectedClients) {
                ec.OnBeforeAddressUpdated += Ec_OnBeforeAddressUpdated;
                if(ec.IPAddress != null) {
                    clientListeners.TryAdd(ec.IPAddress, null);
                }
            }
        }

        private void Ec_OnBeforeAddressUpdated(IRemoteClient client, IPAddress address)
        {
            if(client.IPAddress != null) {
                if(clientListeners.ContainsKey(client.IPAddress)) {
                    var listener = clientListeners[client.IPAddress];
                    if(listener != null) {
                        listener.Close();
                    }
                    clientListeners.TryRemove(client.IPAddress, out TcpClientListener clientListener);
                }
                clientListeners.TryAdd(address, null);
            } else {
                if(!clientListeners.ContainsKey(address)) {
                    TcpClientListener tcpClientListener = listenersQueue.FirstOrDefault(x => x.IPEndPoint.Address == address);
                    clientListeners.TryAdd(address, tcpClientListener);
                }
            }
        }

        private void ListenerWorker()
        {
            while(!cts.IsCancellationRequested) {
                ProcessClientListener();
            }
        }

        private void ProcessClientListener()
        {
            TcpClientListener tcpClientListener;
            if(listenersQueue.TryTake(out tcpClientListener)) {
                try {
                    if(tcpClientListener.IsConnected) {
                        byte[] buffer = new byte[bufferSize];
                        if(tcpClientListener.Read(buffer, buffer.Length)) {
                            PackageReceived(tcpClientListener.IPEndPoint, buffer);
                        }
                        IPAddress ipAddress = tcpClientListener.IPEndPoint.Address;
                        if(clientListeners.ContainsKey(ipAddress) && clientListeners[ipAddress] == null) {
                            clientListeners[ipAddress] = tcpClientListener;
                        }
                    }
                }
                finally {
                    if(tcpClientListener.IsConnected) {
                        listenersQueue.Add(tcpClientListener);
                    } else {
                        Console.WriteLine($"Удаляем из очереди {tcpClientListener.IPEndPoint}");
                    }
                }                
            }
        }

        private void NewConnection(Socket socket)
        {
            Console.WriteLine("New connected client!");
            if(socket == null) {
                return;
            }
            if(!socket.Connected) {
                socket.Close();
                return;
            }

            var removeEndPoint = socket.RemoteEndPoint as IPEndPoint;
            if(removeEndPoint == null) {
                socket.Close();
                return;
            }
            
            TcpClientListener clientListener;

            if(clientListeners.ContainsKey(removeEndPoint.Address)) {
                clientListener = clientListeners[removeEndPoint.Address];
                if(clientListener != null) {
                    clientListener.Close();
                }
            }

            clientListener = new TcpClientListener(socket);
            clientListener.OnDisconnected += (sender, e) => {
                if(clientListeners.ContainsKey(removeEndPoint.Address)) {
                    clientListeners[removeEndPoint.Address] = null;
                }
            };
            
            clientListener.Init();
            Task.Run(() =>
            {
                //1 sec
                int timeout = 10000000;
                var startTime = DateTime.Now.Ticks;
                while(!clientListener.IsConnected) {
                    if((DateTime.Now.Ticks - startTime) > timeout) {
                        logger.Warn($"Превышен интервал ожидания успешного соединения с клиентом при первичном подключении ({clientListener.IPEndPoint})");
                        return;
                    }
                    Thread.Sleep(10);
                }
                listenersQueue.Add(clientListener);
            });
        }

        public void Start()
        {
            if(IsActive) {
                logger.Warn("Сервис уже запущен.");
                return;
            }

            cts = new CancellationTokenSource();
            for(int i = 0; i < workersCount; i++) {
                Task.Run(() => ListenerWorker(), cts.Token);
            }

            Task.Run(async () =>
            {
                TcpListener tcpListener = null;
                try {
                    tcpListener = new TcpListener(localIpEndPoint);
                    tcpListener.Start();
                    while(!cts.IsCancellationRequested) {
                        Socket clientSocket = await tcpListener.AcceptSocketAsync();
                        NewConnection(clientSocket);
                    }
                }
                catch(SocketException e) {
                    logger.Warn(e, "SocketException: {0}");
                }
                finally {
                    tcpListener.Stop();
                }
            }, cts.Token);

            IsActive = true;
        }

        public void Stop()
        {
            try {
                if(!IsActive) {
                    logger.Warn("Невозможно остановить сервис, потомучто он не запущен");
                    return;
                }
                
                cts.Cancel();
                IsActive = false;
                logger.Info("Сервис остановлен");

                return;
            }
            catch(Exception ex) {
                logger.Error(ex, "Stopping listener Failed: " + ex.Message.ToString());
            }
        }

        public void Send(IPAddress address, byte[] buffer)
        {
            if(!IsActive) {
                return;
            }
            if(!clientListeners.ContainsKey(address)) {
                return;
            }

            var listener = clientListeners[address];
            if(listener == null) {
                return;
            }
            listener.Write(buffer, buffer.Length);
        }

        public void SendToAll(byte[] buffer)
        {
            if(!IsActive) {
                return;
            }
            var listeners = clientListeners.Values.ToList();
            foreach(var item in listeners) {
                if(item == null) {
                    continue;
                }
                Task.Run(() => { item.Write(buffer, buffer.Length); });                
            }
        }

        protected virtual void PackageReceived(IPEndPoint ipEndPoint, byte[] buffer)
        {
            OnReceivePackage?.Invoke(ipEndPoint, buffer);
        }

        public event ReceivePackageEventHandler OnReceivePackage;

        public void Dispose()
        {
            cts.Cancel();
            foreach(var cl in clientListeners) {
                if(cl.Value != null) {
                    cl.Value.Close();
                }
            }            
        }
    }
}
