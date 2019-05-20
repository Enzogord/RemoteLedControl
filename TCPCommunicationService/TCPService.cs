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

        private CancellationTokenSource cancelationTokenSource;
        private ConcurrentDictionary<IPAddress, TcpClientListener> activeConnections;

        private readonly IPEndPoint localIpEndPoint;
        private readonly int bufferSize;
        private readonly byte workersCount;

        public event ReceivePackageEventHandler OnReceivePackage;

        public bool IsActive { get; private set; }

        public TCPService(IPEndPoint localIpEndPoint, int bufferSize = 200, byte workersCount = 1)
        {
            this.localIpEndPoint = localIpEndPoint ?? throw new ArgumentNullException(nameof(localIpEndPoint));

            cancelationTokenSource = new CancellationTokenSource();
            activeConnections = new ConcurrentDictionary<IPAddress, TcpClientListener>();

            this.bufferSize = bufferSize;
            this.workersCount = workersCount;
        }

        private void ReadingWorker()
        {
            while(!cancelationTokenSource.IsCancellationRequested) {
                foreach(var clientConnection in activeConnections.Values) {
                    if(!clientConnection.IsConnected) {
                        activeConnections.TryRemove(clientConnection.IPEndPoint.Address, out TcpClientListener value);
                        continue;
                    }

                    if(clientConnection.ReadAvailable && !clientConnection.ReadingInProgress) {
                        Task.Run(() => {
                            byte[] buffer = new byte[bufferSize];
                            if(clientConnection.Read(buffer, buffer.Length)) {
                                PackageReceived(clientConnection.IPEndPoint, buffer);
                            }
                        });
                    }
                }
            }
        }

        private void OpenNewConnection(Socket socket)
        {
            IPAddress clientAddress;
            TcpClientListener clientListener = null;

            try {
                var clientEndPoint = socket.RemoteEndPoint as IPEndPoint;
                if(clientEndPoint == null) {
                    socket.Close();
                    return;
                }
                clientAddress = clientEndPoint.Address;
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка при создании нового подключения клиента");
                return;
            }

            clientListener = new TcpClientListener(socket);
            clientListener.Init();

            Task clientConnectionTask = new Task(() => {
                //1 sec
                int timeout = 10000000;
                var startTime = DateTime.Now.Ticks;
                while(!clientListener.IsConnected) {
                    if((DateTime.Now.Ticks - startTime) > timeout) {
                        string message = $"Превышен интервал ожидания успешного соединения с клиентом при первичном подключении ({clientListener.IPEndPoint})";
                        logger.Warn(message);
                        throw new OperationCanceledException(message);
                    }
                    Thread.Sleep(10);
                }
            });

            //При успешном подключении
            clientConnectionTask.ContinueWith((baseTask) => {
                try {
                    activeConnections.TryAdd(clientAddress, clientListener);
                }
                catch(Exception ex) {
                    logger.Error(ex, "Ошибка при добавлении клиента в список ожидающих активации");
                    return;
                }
                OnClientConnected(clientListener);
                clientListener.OnDisconnected += (sender, e) => {
                    OnClientDisconnected(clientListener);
                };
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            //Если превышен интервал при подключении
            clientConnectionTask.ContinueWith((baseTask) => {
                clientListener.Close();
            }, TaskContinuationOptions.NotOnRanToCompletion);

            clientConnectionTask.Start();
        }

        protected virtual void OnClientConnected(TcpClientListener clientListener)
        {            
        }

        protected virtual void OnClientDisconnected(TcpClientListener clientListener)
        {
            if(clientListener == null) {
                return;
            }
            activeConnections.TryRemove(clientListener.IPEndPoint.Address, out TcpClientListener value);
        }

        public void Start()
        {
            if(IsActive) {
                logger.Warn("Сервис уже запущен.");
                return;
            }

            cancelationTokenSource = new CancellationTokenSource();
            Task.Run(() => ReadingWorker(), cancelationTokenSource.Token);

            Task.Run(async () =>
            {
                TcpListener tcpListener = null;
                try {
                    tcpListener = new TcpListener(localIpEndPoint);
                    tcpListener.Start();
                    while(!cancelationTokenSource.IsCancellationRequested) {
                        Socket clientSocket = await tcpListener.AcceptSocketAsync();
                        OpenNewConnection(clientSocket);
                    }
                }
                catch(SocketException e) {
                    logger.Warn(e, "SocketException: {0}");
                }
                finally {
                    tcpListener.Stop();
                }
            }, cancelationTokenSource.Token);

            IsActive = true;
        }

        public void Stop()
        {
            try {
                if(!IsActive) {
                    logger.Warn("Невозможно остановить сервис, потомучто он не запущен");
                    return;
                }
                
                cancelationTokenSource.Cancel();
                IsActive = false;
                logger.Info("Сервис остановлен");

                return;
            }
            catch(Exception ex) {
                logger.Error(ex, "Stopping listener Failed: " + ex.Message.ToString());
            }
        }

        public void Send(IPAddress address, byte[] data)
        {
            if(!IsActive) {
                return;
            }
            if(!activeConnections.ContainsKey(address)) {
                return;
            }
            var listener = activeConnections[address];
            if(listener == null) {
                return;
            }
            listener.Write(data);
        }

        public void SendToAll(byte[] data)
        {
            if(!IsActive) {
                return;
            }
            var connections = activeConnections.Values.ToList();
            foreach(var connection in connections) {
                if(connection == null) {
                    continue;
                }
                Task.Run(() => { connection.Write(data); });                
            }
        }

        protected virtual void PackageReceived(IPEndPoint ipEndPoint, byte[] buffer)
        {
            OnReceivePackage?.Invoke(ipEndPoint, buffer);
        }

        public void Dispose()
        {
            cancelationTokenSource.Cancel();
            foreach(var cl in activeConnections) {
                if(cl.Value != null) {
                    cl.Value.Close();
                }
            }            
        }
    }
}
