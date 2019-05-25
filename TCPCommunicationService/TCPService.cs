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
        private ConcurrentDictionary<TcpClientListener, int> activeConnections;
        //private ConcurrentDictionary<IPAddress, TcpClientListener> activeConnections;

        private readonly IPEndPoint localIpEndPoint;
        private readonly int bufferSize;
        private readonly byte workersCount;

        public event ReceivePackageEventHandler OnReceivePackage;

        public bool IsActive { get; private set; }

        public TCPService(IPEndPoint localIpEndPoint, int bufferSize = 200, byte workersCount = 1)
        {
            this.localIpEndPoint = localIpEndPoint ?? throw new ArgumentNullException(nameof(localIpEndPoint));

            cancelationTokenSource = new CancellationTokenSource();
            //activeConnections = new ConcurrentDictionary<IPAddress, TcpClientListener>();
            activeConnections = new ConcurrentDictionary<TcpClientListener, int>();

            this.bufferSize = bufferSize;
            this.workersCount = workersCount;
        }

        private void ReadingWorker()
        {
            while(!cancelationTokenSource.IsCancellationRequested) {
                foreach(var clientConnection in activeConnections.Keys) {
                    if(!clientConnection.IsConnected) {
                        /*IPAddress address = clientConnection?.IPEndPoint?.Address;
                        if(address == null) {
                            continue;
                        }*/
                        activeConnections.TryRemove(clientConnection, out int value);
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
            logger.Debug("Open new client connection");
            //IPAddress clientAddress;
            TcpClientListener clientListener = null;

            try {
                var clientEndPoint = socket.RemoteEndPoint as IPEndPoint;
                if(clientEndPoint == null) {
                    socket.Close();
                    return;
                }
                //clientAddress = clientEndPoint.Address;
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка при создании нового подключения клиента");
                return;
            }

            clientListener = new TcpClientListener(socket);
            logger.Debug("Init client connection");
            clientListener.Init();

            Task clientConnectionTask = new Task(() => {
                //1 sec
                logger.Debug("Start client connection Task");
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
                logger.Debug("Succesfully end client connection task");
            });

            //При успешном подключении
            clientConnectionTask.ContinueWith((baseTask) => {
                logger.Debug("On Succesfully client connection");
                try {
                    activeConnections.TryAdd(clientListener, 0);
                }
                catch(Exception ex) {
                    logger.Error(ex, "Ошибка при добавлении клиента в список ожидающих активации");
                    return;
                }

                OnClientConnected(clientListener);

                logger.Debug("Added to activeConnections dictionary");
                clientListener.OnDisconnected += (sender, e) => {
                    logger.Debug("On client disconnected");
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
            /*IPAddress address = clientListener?.IPEndPoint?.Address;
            if(address == null) {
                activeConnections.
                return;
            }*/
            activeConnections.TryRemove(clientListener, out int value);
            logger.Debug("Disconnected client removed from activeConnections dictionary");
        }

        private bool ClientAddingEnabled = true;

        public void Start()
        {
            logger.Debug("Start TCP service");
            if(IsActive) {
                logger.Warn("Сервис уже запущен.");
                return;
            }

            cancelationTokenSource = new CancellationTokenSource();
            Task.Run(() => ReadingWorker());

            ClientAddingEnabled = true;

            Task.Run(async () =>
            {
                TcpListener tcpListener = null;
                try {
                    tcpListener = new TcpListener(localIpEndPoint);
                    tcpListener.Start();
                    while(ClientAddingEnabled && !cancelationTokenSource.IsCancellationRequested) {
                        Socket clientSocket = await tcpListener.AcceptSocketAsync();
                        if(ClientAddingEnabled) {
                            OpenNewConnection(clientSocket);
                        } else {
                            clientSocket.Close();
                        }
                    }
                }
                catch(SocketException e) {
                    logger.Warn(e, "SocketException: {0}");
                }
                finally {
                    tcpListener.Stop();
                }
            });

            IsActive = true;
        }

        public void Stop()
        {
            logger.Debug("Stop TCP service");
            try {
                if(!IsActive) {
                    logger.Warn("Невозможно остановить сервис, потомучто он не запущен");
                    return;
                }

                Dispose();
                IsActive = false;
                logger.Info("Сервис остановлен");

                return;
            }
            catch(Exception ex) {
                logger.Error(ex, "Stopping listener Failed: " + ex.Message.ToString());
            }
        }

        public void SendToAll(byte[] data)
        {
            if(!IsActive) {
                return;
            }
            var connections = activeConnections.Keys.ToList();
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
            ClientAddingEnabled = false;
            cancelationTokenSource.Cancel();
            foreach(var cl in activeConnections.Keys) {
                cl.Close();
            }
        }
    }
}
