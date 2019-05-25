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
        private bool wasConnected;
        private bool initialized;

        public bool ReadingInProgress { get; private set; }

        public bool ReadAvailable {
            get {
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

        private void Connected()
        {
            logger.Debug("Client connected");
            wasConnected = true;
            OnConnected?.Invoke(this, EventArgs.Empty);
        }

        public void Init()
        {
            Task.Run(() => {
                for(int i = 0; i < 1000; i++) {
                    try {
                        if(socket.Connected) {
                            try {
                                networkStream = new NetworkStream(socket);
                            }
                            catch(Exception ex) {
                                logger.Error(ex, $"Невозможно создать NetworkStream");
                            }
                            Connected();
                            return;
                        }
                    }
                    catch(SocketException) {
                    }
                    catch(ObjectDisposedException) {
                        return;
                    }

                    Thread.Sleep(10);
                }
            }, 
            cts.Token);
            initialized = true;
        }

        public bool IsConnected {
            get {
                if(!initialized) {
                    Init();
                }
                if(!wasConnected) {
                    return false;
                }
                if(!socket.IsConnected()) {
                    Close();
                    return false;
                }
                return true;
            }
        }

        private bool Available()
        {
            if(wasConnected && !IsConnected) {
                OnDisconnected?.Invoke(this, EventArgs.Empty);
                return false;
            }
            return true;
        }

        public bool Read(byte[] buffer, int length)
        {
            if(!Available() || networkStream == null) {
                return false;
            }
            ReadingInProgress = true;
            try {
                if(networkStream.DataAvailable) {
                    Array.Clear(buffer, 0, buffer.Length);
                    networkStream.Read(buffer, 0, length);
                    OnReceiveData?.Invoke(this, new DataEventArgs(buffer));
                    return true;
                }
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
            if(!Available() || networkStream == null) {
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
