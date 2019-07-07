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

        TcpListener tcpListener = null;

        private CancellationTokenSource cancelationTokenSource;
        private ConcurrentDictionary<TcpClientListener, bool> activeConnections;

        private readonly IPEndPoint localIpEndPoint;
        private readonly int bufferSize;
        private readonly byte workersCount;

        public event ReceivePackageEventHandler OnReceivePackage;

        public bool IsActive { get; private set; }

        public TCPService(IPEndPoint localIpEndPoint, int bufferSize = 200, byte workersCount = 1)
        {
            this.localIpEndPoint = localIpEndPoint ?? throw new ArgumentNullException(nameof(localIpEndPoint));

            cancelationTokenSource = new CancellationTokenSource();

            //если value true значит в данный момент с этого клиента считываются данные
            activeConnections = new ConcurrentDictionary<TcpClientListener, bool>();

            this.bufferSize = bufferSize;
            this.workersCount = workersCount;
        }

        private void ReadingWorker()
        {
            while(!cancelationTokenSource.IsCancellationRequested) {
                foreach(var clientConnectionRecord in activeConnections) {
                    if(!clientConnectionRecord.Key.ReadingInProgress && !clientConnectionRecord.Value) {
                        TcpClientListener listener = clientConnectionRecord.Key;
                        activeConnections[listener] = true;
                        Task.Run(() =>
                        {
                            byte[] buffer = new byte[bufferSize];
                            if(listener.Read(buffer, buffer.Length)) {
                                PackageReceived(listener.IPEndPoint, buffer);
                            }
                            activeConnections[listener] = false;
                        });
                    }
                }
                Thread.Sleep(1);
            }
        }

        private void OpenNewConnection(Socket socket)
        {
            logger.Debug("Open new client connection");
            TcpClientListener clientListener = new TcpClientListener(socket);

            logger.Debug("Init client connection");
            clientListener.OnConnected += ClientListener_OnConnected;
            clientListener.OnDisconnected += ClientListener_OnDisconnected;
            clientListener.Open();

            void ClientListener_OnConnected(object sender, EventArgs e)
            {
                try {
                    if(activeConnections.TryAdd(clientListener, false)) {
                        OnClientConnected(clientListener);
                        clientListener.OnConnected -= ClientListener_OnConnected;
                    }
                }
                catch(Exception ex) {
                    logger.Error(ex, "Ошибка при добавлении клиента в список ожидающих активации");
                    return;
                }
            }

            void ClientListener_OnDisconnected(object sender, EventArgs e)
            {
                if(activeConnections.TryRemove(clientListener, out bool value)) {
                    logger.Debug("Disconnected client removed from activeConnections dictionary");
                    OnClientDisconnected(clientListener);
                    clientListener.OnDisconnected -= ClientListener_OnDisconnected;
                }
            }
        }

        protected virtual void OnClientConnected(TcpClientListener clientListener)
        {
        }

        protected virtual void OnClientDisconnected(TcpClientListener clientListener)
        {
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

            Exception serviceStartException = null;

            Task.Run(async () =>
            {
                int attemptsCounter = 1;
                ClientAddingEnabled = true;
                try{
                    CancellationTokenSource timingCancelationTokenSource = new CancellationTokenSource(5000);
                    await Task.Run(() => {
                        while (!IsActive && ClientAddingEnabled && !cancelationTokenSource.IsCancellationRequested && !timingCancelationTokenSource.IsCancellationRequested) {
                            try {
                                if(tcpListener != null) {
                                    tcpListener.Stop();
                                }
                                tcpListener = new TcpListener(localIpEndPoint);
                                tcpListener.Start();
                                IsActive = true;
                            }
                            catch(SocketException e) {
                                logger.Error(e, $"SocketException on start tcp service. Attempt number: {attemptsCounter}.");
                                attemptsCounter++;
                                if(attemptsCounter > 5) {
                                    serviceStartException = e;
                                    break;
                                }
                                Thread.Sleep(500);
                            }
                        }
                    }, timingCancelationTokenSource.Token);
                    
                    while(IsActive && ClientAddingEnabled && !cancelationTokenSource.IsCancellationRequested) {
                        Socket clientSocket = await tcpListener.AcceptSocketAsync();
                        if(ClientAddingEnabled) {
                            OpenNewConnection(clientSocket);
                        } else {
                            clientSocket.Close();
                        }
                    }
                }
                finally{
                    try{
                        tcpListener.Stop();
                    }
                    catch(Exception ex){
                        logger.Error(ex, "Исключение при остановке TCP сервиса");
                    }
                }                
            });

            while(!IsActive) {
                if(serviceStartException != null) {
                    throw serviceStartException;
                }
            }

            Task.Run(() => ReadingWorker());
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

        public virtual void Dispose()
        {
            ClientAddingEnabled = false;
            cancelationTokenSource.Cancel();
            tcpListener.Stop();
            foreach(var cl in activeConnections.Keys) {
                cl.Close();
            }
            activeConnections.Clear();
            IsActive = false;
        }
    }
}
