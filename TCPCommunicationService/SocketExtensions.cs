﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPCommunicationService
{
    public static class SocketExtensions
    {
        private const int BytesPerLong = 4; // 32 / 8
        private const int BitsPerByte = 8;

        public static bool IsConnected(this Socket socket)
        {
            try {
                return !(socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch(SocketException) {
                return false;
            }
            catch(ObjectDisposedException) {
                return false;
            }
        }


        /// <summary>
        /// Sets the keep-alive interval for the socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="time">Time between two keep alive "pings". (Milliseconds)</param>
        /// <param name="interval">Time between two keep alive "pings" when first one fails. (Milliseconds)</param>
        /// <returns>If the keep alive infos were succefully modified.</returns>
        public static bool SetKeepAlive(this Socket socket, ulong time, ulong interval)
        {
            try {
                // Array to hold input values.
                var input = new[]
            {
                (time == 0 || interval == 0) ? 0UL : 1UL, // on or off
                time,
                interval
            };

                // Pack input into byte struct.
                byte[] inValue = new byte[3 * BytesPerLong];
                for(int i = 0; i < input.Length; i++) {
                    inValue[i * BytesPerLong + 3] = (byte)(input[i] >> ((BytesPerLong - 1) * BitsPerByte) & 0xff);
                    inValue[i * BytesPerLong + 2] = (byte)(input[i] >> ((BytesPerLong - 2) * BitsPerByte) & 0xff);
                    inValue[i * BytesPerLong + 1] = (byte)(input[i] >> ((BytesPerLong - 3) * BitsPerByte) & 0xff);
                    inValue[i * BytesPerLong + 0] = (byte)(input[i] >> ((BytesPerLong - 4) * BitsPerByte) & 0xff);
                }

                // Create bytestruct for result (bytes pending on server socket).
                byte[] outValue = BitConverter.GetBytes(0);

                // Write SIO_VALS to Socket IOControl.
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                socket.IOControl(IOControlCode.KeepAliveValues, inValue, outValue);
            }
            catch(SocketException) {
                return false;
            }

            return true;
        }
    }
}
