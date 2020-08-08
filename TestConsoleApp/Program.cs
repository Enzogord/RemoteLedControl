using System;
using System.Net;
using System.Threading;
using System.Timers;
using UdpServer;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //var address = IPAddress.Parse("192.168.1.217");
            //var address = IPAddress.Parse("255.255.255.255");
            timer = new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 1000;

            var address = IPAddress.Any;
            var port = 11011;
            UdpServer.UdpServer server = new UdpServer.UdpServer(address, port, 1460);
            server.EnableBroadcast = true;
            server.Start();
            timer.Start();

            server.DataReceived += Server_DataReceived;

            while(true) {
                string cmd = Console.ReadLine();
                if(cmd == "exit") {
                    return;
                }
                string testMessage = "test";
                var bytes = System.Text.Encoding.UTF8.GetBytes(testMessage);

                server.Send(bytes, new IPEndPoint(IPAddress.Parse("192.168.1.217"), 11011));
            }
        }

        

        private static int receivingBytes = 0;
        private static System.Timers.Timer timer;

        private static void Server_DataReceived(object sender, ReceivedDataEventArgs e)
        {
            Interlocked.Add(ref receivingBytes, e.Size);
        }

        private static int lastReceivingBytes = 0;

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var bytesInSecond = receivingBytes - lastReceivingBytes;
            lastReceivingBytes = receivingBytes;
            Console.WriteLine($"Receiving in second: {bytesInSecond} bytes");
        }


    }
}
