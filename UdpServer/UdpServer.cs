﻿using NLog;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkServer.UDP
{
    public class UdpServer
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        private Socket socket;
        private Task task;
        private CancellationTokenSource cts;

        public UdpServer(int bufferSize = 1460)
        {            
            BufferSize = bufferSize;
        }

        ~UdpServer()
        {            
            socket?.Dispose();
        }

        #region Settings

        /// <summary>
        /// Ip address for new socket
        /// </summary>
        public IPAddress Address { get; set; }

        /// <summary>
        /// Port for new socket
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Receiving package buffer size
        /// </summary>
        private int BufferSize { get; set; }

        /// <summary>
        /// Enable broadcast sending
        /// </summary>
        public bool EnableBroadcast { get; set; }

        #endregion Settings

        #region Statuses

        public IPAddress LocalAddress {
            get {
                try {
                    return (socket.LocalEndPoint as IPEndPoint).Address;
                }
                catch(Exception ex) {
                    logger.Error(ex, "Ошибка при получении локального IP адреса сокета");
                    return null;
                }
            }
        }

        public int LocalPort {
            get {
                try {
                    return (socket.LocalEndPoint as IPEndPoint).Port;
                }
                catch(Exception ex) {
                    logger.Error(ex, "Ошибка при получении локального порта сокета");
                    return 0;
                }
            }
        }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set {
                if(isActive != value) {
                    isActive = value;
                    StatusChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler StatusChanged;

        #endregion Statuses

        #region Actions

        private void CreateSocket()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            if(EnableBroadcast) {
                socket.EnableBroadcast = true;
            }
            socket.ExclusiveAddressUse = true;
            socket.Blocking = true;
            socket.ReceiveBufferSize = BufferSize;
            socket.Bind(new IPEndPoint(Address, Port));
        }

        private void CloseSocket()
        {            
            socket?.Dispose();
            socket = null;
        }

        public void Start(IPAddress address, int port)
        {
            if(address is null) {
                throw new ArgumentNullException(nameof(address));
            }

            if(port <= 0 || port > 65535) {
                throw new ArgumentException("Некорректный порт", nameof(port));
            }

            if(IsActive) {
                throw new InvalidOperationException("Udp server is already active now");
            }
            try {
                Address = address;
                Port = port;
                CreateSocket();
                StartListening();
                IsActive = true;
            }
            catch(Exception) {
                IsActive = false;
                throw;
            }
        }

        public void Stop()
        {
            try {
                StopListening();
                CloseSocket();
            }
            finally {
                IsActive = false;
            }
        }

        #endregion Actions

        #region Send actions

        private void CheckSendingAvailable()
        {
            if(socket == null || !IsActive) {
                throw new InvalidOperationException("Udp server is not active");
            }

            if(socket == null) {
                throw new InvalidOperationException("Socket is null");
            }
        }

        public void Send(byte[] data, IPAddress address)
        {
            if(address is null) {
                throw new ArgumentNullException(nameof(address));
            }

            CheckSendingAvailable();

            socket.SendTo(data, new IPEndPoint(address, LocalPort));
        }

        public void Send(byte[] data, IPAddress address, int port)
        {
            if(address is null) {
                throw new ArgumentNullException(nameof(address));
            }

            CheckSendingAvailable();

            socket.SendTo(data, new IPEndPoint(address, port));
        }

        public void Send(byte[] data, IPEndPoint endPoint)
        {
            if(endPoint is null) {
                throw new ArgumentNullException(nameof(endPoint));
            }

            CheckSendingAvailable();

            socket.SendTo(data, endPoint);
        }

        #endregion Send actions

        #region Receive actions

        public event EventHandler<ReceivedDataEventArgs> DataReceived;

        private void StartListening()
        {
            cts = new CancellationTokenSource();
            task = new Task(() => ReadingSocketLoop(cts.Token), cts.Token);
            task.Start();
            logger.Debug($"Udp server start listening socket on {socket.LocalEndPoint}");
        }

        private void StopListening()
        {
            cts?.Cancel();
            cts = null;
            task = null;
            logger.Debug($"Udp server stop listening socket on {socket?.LocalEndPoint}");
        }

        private void ReadingSocketLoop(CancellationToken token)
        {
            while(!token.IsCancellationRequested) {
                ReadSocket();
            }
        }

        private void ReadSocket()
        {
            try {
                if(!socket.Poll(-1, SelectMode.SelectRead)) {
                    return;
                }

                if(socket.Available == 0) {
                    return;
                }

                byte[] buffer = new byte[BufferSize];
                EndPoint remoteEndPoint = new IPEndPoint(0, 0);

                var receivedBytes = socket.ReceiveFrom(buffer, BufferSize, SocketFlags.None, ref remoteEndPoint);
                if(receivedBytes > 0) {
                    logger.Debug($"Udp server receive {receivedBytes} bytes from {remoteEndPoint}");
                    byte[] data = new byte[receivedBytes];
                    Array.Copy(buffer, 0, data, 0, receivedBytes);
                    RaiseDataReceived(data, receivedBytes, remoteEndPoint);
                }
            }
            catch(SocketException ex) {
                if(ex.ErrorCode == (int)SocketError.ConnectionReset)
                    return;
            }
            catch(Exception ex) {
                DisconnectedWithError(ex);
            }
        }

        private void RaiseDataReceived(byte[] data, int length, EndPoint remoteEndPoint)
        {
            DataReceived?.Invoke(this, new ReceivedDataEventArgs(data, length, remoteEndPoint));
        }

        #endregion Receive actions

        private void DisconnectedWithError(Exception ex)
        {
            logger.Error(ex, "Udp server was stopped with error");
            Stop();
        }
    }
}
