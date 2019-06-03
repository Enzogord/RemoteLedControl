using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace TCPCommunicationService
{
    public class TcpClientListener : IDisposable
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public Guid Id { get; private set; }
        private CancellationTokenSource cts = new CancellationTokenSource();
        private readonly Socket socket;
        private NetworkStream networkStream;

        public IPEndPoint IPEndPoint {
            get {
                try {
                    return socket.RemoteEndPoint as IPEndPoint;
                }
                catch(Exception) {
                    return null;
                }
            }
        }

        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;
        public event EventHandler<DataEventArgs> OnReceiveData;

        public TcpClientListener(Socket socket)
        {
            this.socket = socket;
            Id = Guid.NewGuid();
        }

        private bool connected = false;
        public bool IsConnected => CheckConnection();

        bool inClosing = false;
        private bool CheckConnection()
        {
            if(inClosing) {
                return false;
            }
            if(!socket.IsConnected() && connected) {
                inClosing = true;
                logger.Debug("Соединение было прервано. Закрываем клиент");
                Task.Run(() => Close());
                return false;
            } else if(!socket.IsConnected() && !connected) {
                return false;
            }
            return true;
        }

        public void Open()
        {
            CancellationTokenSource timeoutCancelationToken = new CancellationTokenSource(10000);
            Task.Run(() => {
                while(!timeoutCancelationToken.IsCancellationRequested && !cts.IsCancellationRequested) {
                    try {
                        if(socket.IsConnected()) {
                            try {
                                networkStream = new NetworkStream(socket);
                            }
                            catch(Exception ex) {
                                logger.Error(ex, $"Невозможно создать NetworkStream");
                                continue;
                            }
                            logger.Debug("Client connected");
                            socket.SetKeepAlive(12000, 2000);
                            connected = true;
                            OnConnected?.Invoke(this, EventArgs.Empty);
                            return;
                        }
                    }
                    catch(SocketException ex) {
                        logger.Error(ex, "Ошибка при создании подключения для нового клиента");
                    }
                    catch(ObjectDisposedException ex) {
                        logger.Error(ex, "Ошибка при создании подключения для нового клиента, подключение уже было закрыто. Процесс подключения будет прерван.");
                        Close();
                        return;
                    }
                    Thread.Sleep(10);
                }
            });
        }

        public bool ReadingInProgress { get; private set; }

        public bool ReadAvailable {
            get {
                if(!IsConnected) {
                    return false;
                }
                if(networkStream == null) {
                    return false;
                }
                try {
                    return networkStream.DataAvailable;
                }
                catch(Exception) {
                    return false;
                }
            }
        }

        public bool Read(byte[] buffer, int length)
        {
            if(!ReadAvailable) {
                return false;
            }
            ReadingInProgress = true;
            try {
                Array.Clear(buffer, 0, buffer.Length);
                networkStream.Read(buffer, 0, length);
                OnReceiveData?.Invoke(this, new DataEventArgs(buffer));
                return true;
            }
            catch(Exception ex) {
                logger.Error(ex, "Невозможно прочитать данные из NetworkStream");
            }
            finally {
                ReadingInProgress = false;
            }
            return false;
        }

        public void Write(byte[] data)
        {
            if(IsConnected && networkStream == null) {
                return;
            }
            try {
                networkStream.Write(data, 0, data.Length);
            }
            catch(Exception ex) {
                logger.Error(ex, "Невозможно записать данные в NetworkStream");
            }
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            cts.Cancel();
            if(networkStream != null) {
                networkStream.Close();
            }
            if(socket != null) {
                socket.Close();
            }
            OnDisconnected?.Invoke(this, EventArgs.Empty);
        }

        public override bool Equals(object obj)
        {
            var listener = obj as TcpClientListener;
            return listener != null &&
                   Id.Equals(listener.Id);
        }

        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<Guid>.Default.GetHashCode(Id);
        }
    }
}
