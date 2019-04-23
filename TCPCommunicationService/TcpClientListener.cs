using System;
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

        private CancellationTokenSource cts = new CancellationTokenSource();
        private readonly Socket socket;
        private NetworkStream networkStream;
        private bool wasConnected;
        private bool initialized;

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
        public event EventHandler OnDisconnected;        

        public TcpClientListener(Socket socket)
        {
            this.socket = socket;            
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
                            wasConnected = true;
                            return;
                        }
                    }
                    catch(SocketException) {
                    }
                    catch(ObjectDisposedException) {
                        wasConnected = true;
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

        public bool Read(byte[] buffer, int length)
        {
            if(networkStream == null) {
                return false;
            }
            try {
                if(networkStream.DataAvailable) {
                    Array.Clear(buffer, 0, buffer.Length);
                    networkStream.Read(buffer, 0, length);
                    return true;
                }
            }
            catch(Exception ex) {
                logger.Error(ex, "Невозможно прочитать данные из NetworkStream");
            }
            return false;
        }

        public void Write(byte[] buffer, int length)
        {
            if(networkStream == null) {
                return;
            }
            try {
                networkStream.Write(buffer, 0, length);
            }
            catch(Exception ex) {
                logger.Error(ex, "Невозможно записать данные в NetworkStream");
            }
        }

        public void Close()
        {
            OnDisconnected?.Invoke(this, EventArgs.Empty);
            Dispose();
        }

        public void Dispose()
        {
            cts.Cancel();
            if(networkStream != null) {
                networkStream.Dispose();
            }
            if(socket != null) {
                socket.Dispose();
            }
        }
    }
}
