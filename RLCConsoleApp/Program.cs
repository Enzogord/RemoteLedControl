﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Messages;
using RLCCore;
using RLCCore.RemoteOperations;
using SNTPService;
using UDPCommunication;

namespace RLCConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {
            //controller.Server = new UDPServer(controller.CurrentProject.Key);
            //controller.Server.Initialize(controller.NetworkController.GetServerIPAddress(), 11010, controller.NetworkController.BroadcastIPAddress);

            //var commandContextProvider = new GlobalCommandsContextProvider();
            //commandContextProvider.ServerAddressProvider = controller.NetworkController;
            //commandContextProvider.StartTimeProvider = new StartTimeProvider();






            //udpService = new UDPService<RLCMessage>(IPAddress.Any/*controller.NetworkController.GetServerIPAddress()*/, controller.NetworkController.Port, new MessageParser<RLCMessage>());

            //RemoteClientsOperator rlcOperator = new RemoteClientsOperator(controller.CurrentProject, controller.NetworkController, udpService);
            //udpService.OnReceiveMessage += UdpService_OnReceiveMessage;
            //udpService.StartReceiving();





            TestOperator testOperator = new TestOperator();

            Task.Run(() =>
            {
                while(true) {
                    testOperator.SendPlay();
                    Thread.Sleep(5000);
                }
            });

            //controller.Server.StartReceiving();
            //rlcOperator.


            Console.ReadKey();
        }

        
    }

    public class TestOperator
    {
        UDPService<RLCMessage> udpService;
        RLCProjectController controller;
        RemoteClientsOperator rlcOperator;
        SntpService sntpService;

        public TestOperator()
        {
            controller = new RLCProjectController();

            sntpService = new SntpService(11011);
            sntpService.InterfaceAddress = controller.NetworkController.GetServerIPAddress();
            sntpService.Start();
            //controller.CurrentProject.AddClient(new RemoteClient("Test1", 1) { IPAddress = IPAddress.Parse("192.168.1.166") });
            rlcOperator = new RemoteClientsOperator(controller.CurrentProject, controller.NetworkController);
            udpService = new UDPService<RLCMessage>(IPAddress.Any/*controller.NetworkController.GetServerIPAddress()*/, controller.NetworkController.Port, new MessageParser<RLCMessage>());
            udpService.OnReceiveMessage += UdpService_OnReceiveMessage;
            udpService.StartReceiving();
            rlcOperator.Start();
        }

        internal void SendPlay()
        {
            rlcOperator.Play();
        }

        private void UdpService_OnReceiveMessage(object sender, IPEndPoint endPoint, RLCMessage e)
        {
            Console.WriteLine($"Receive message: {e.MessageType}");
            if(e.MessageType == MessageType.RequestServerIp) {
                controller.CurrentProject.UpdateClientIPAddress(e.ClientNumber, endPoint.Address);
                udpService.Send(RLCMessageFactory.SendServerIP(controller.CurrentProject.Key, controller.NetworkController.GetServerIPAddress()), controller.NetworkController.BroadcastIPAddress, controller.NetworkController.Port);
            }
        }
    }
}