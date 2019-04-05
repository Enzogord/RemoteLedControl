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
    public delegate void ReceivePackageEventHandler(IPAddress address, byte[] package);

    public class TCPService : IDisposable
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        private CancellationTokenSource cts = new CancellationTokenSource();
        private Dictionary<IPAddress, TcpClientListener> clientListeners;
        private BlockingCollection<TcpClientListener> listenersQueue;
        private readonly IPEndPoint localIpEndPoint;
        private readonly IEnumerable<IRemoteClient> expectedClients;
        private readonly int bufferSize;
        private bool isActive;        

        public TCPService(IPEndPoint localIpEndPoint, IEnumerable<IRemoteClient> expectedClients, int bufferSize, byte workersCount = 2)
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

            for(int i = 0; i < workersCount; i++) {
                Task.Run(() => ListenerWorker(), cts.Token);
            }

            this.localIpEndPoint = localIpEndPoint;
            this.expectedClients = expectedClients;
            this.bufferSize = bufferSize;
            clientListeners = new Dictionary<IPAddress, TcpClientListener>();
            foreach(var ec in expectedClients) {
                clientListeners.Add(ec.IPAddress, null);
            }
        }

        public void ListenerWorker()
        {
            while(!cts.IsCancellationRequested) {
                ProcessClientListener();
            }
        }

        public void ProcessClientListener()
        {
            TcpClientListener tcpClientListener;
            if(listenersQueue.TryTake(out tcpClientListener)) {
                try {
                    if(tcpClientListener.IsConnected) {
                        byte[] buffer = new byte[bufferSize];
                        if(tcpClientListener.Read(buffer, buffer.Length)) {
                            OnReceivePackage?.Invoke(tcpClientListener.IPAddress, buffer);
                        }
                    }
                }
                finally {
                    if(tcpClientListener.IsConnected) {
                        listenersQueue.Add(tcpClientListener);
                    } else {
                        Console.WriteLine($"Удаляем из очереди {tcpClientListener.IPAddress}");
                    }
                }                
            }
        }

        public void NewConnection(Socket socket)
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

            if(!clientListeners.ContainsKey(removeEndPoint.Address)) {
                socket.Close();
                return;
            }
            TcpClientListener clientListener = clientListeners[removeEndPoint.Address];
            if(clientListener != null) {
                clientListener.Close();
            }

            clientListener = new TcpClientListener(socket);
            clientListener.OnDisconnected += (sender, e) => {
                clientListeners[removeEndPoint.Address] = null;
            }; 
            clientListeners[removeEndPoint.Address] = clientListener;
            clientListener.Init();
            Task.Run(() =>
            {
                //1 sec
                int timeout = 10000000;
                var startTime = DateTime.Now.Ticks;
                while(!clientListener.IsConnected) {
                    if((DateTime.Now.Ticks - startTime) > timeout) {
                        logger.Warn($"Превышен интервал ожидания успешного соединения с клиентом при первичном подключении ({clientListener.IPAddress})");
                        return;
                    }
                    Thread.Sleep(10);
                }
                listenersQueue.Add(clientListener);
            });
        }

        public void Start()
        {
            if(isActive) {
                logger.Warn("Сервис уже запущен.");
                return;
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
        }

        public void Stop()
        {
            try {
                if(!isActive) {
                    logger.Warn("Невозможно остановить сервис, потомучто он не запущен");
                    return;
                }
                
                isActive = false;
                logger.Info("Сервис остановлен");

                return;
            }
            catch(Exception ex) {
                logger.Error(ex, "Stopping listener Failed: " + ex.Message.ToString());
            }
        }

        public void Send(IPAddress address, byte[] buffer, int length)
        {
            if(!clientListeners.ContainsKey(address)) {
                return;
            }

            var listener = clientListeners[address];
            if(listener == null) {
                return;
            }
            listener.Write(buffer, length);
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
