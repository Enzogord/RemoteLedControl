using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NLog;

namespace UDPTransmission
{
    public class UDPTransmiter : IDisposable
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();

        private UdpClient udpClient;
        private bool stop;
        private bool isRun;

        public bool IsRun {
            get => isRun;
            private set {
                isRun = value;
                OnChangeStatus?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler OnChangeStatus;
        public event EventHandler<ReceivingDataEventArgs> OnReceivePackage;

        public UDPTransmiter(IPAddress ipAddress, int port)
        {
            var ipEndPoint = new IPEndPoint(ipAddress, port);
            udpClient = new UdpClient(ipEndPoint);
        }

        public void StartReceiving()
        {
            stop = false;
            IsRun = true;
            Receive();
        }

        public void StopReceiving()
        {
            stop = true;
            udpClient.Client.Close();
            Thread.Sleep(500);
            IsRun = false;
        }

        private void Receive()
        {
            udpClient.BeginReceive(new AsyncCallback(MyReceiveCallback), null);
        }

        private void MyReceiveCallback(IAsyncResult result)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
            if(!stop) {
                Byte[] receiveBytes = udpClient.EndReceive(result, ref ip);
                OnReceivePackage?.Invoke(this, new ReceivingDataEventArgs(receiveBytes, ip));
                Receive();
            }
        }

        public void UDPSend(byte[] bytes, IPAddress remoteIPAddress, int remotePort)
        {
            UDPSend(bytes, new IPEndPoint(remoteIPAddress, remotePort));
        }

        public void UDPSend(byte[] bytes, IPEndPoint ipEndPoint)
        {
            udpClient.EnableBroadcast = true;
            try {
                udpClient.Send(bytes, bytes.Length, ipEndPoint);
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка при отправке UDP пакета");
            }
        }

        public void Dispose()
        {
            udpClient.Dispose();
        }
    }
}
