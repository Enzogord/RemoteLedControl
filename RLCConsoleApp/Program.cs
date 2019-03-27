using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLCCore;
using RLCCore.RemoteOperations;
using RLCCore.Transport;

namespace RLCConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            RLCProjectController controller = new RLCProjectController();
            controller.Server = new UDPServer(controller.CurrentProject.Key);
            controller.Server.Initialize(controller.NetworkController.GetServerIPAddress(), 11010, controller.NetworkController.BroadcastIPAddress);

            var commandContextProvider = new GlobalCommandsContextProvider();
            commandContextProvider.ServerAddressProvider = controller.NetworkController;
            commandContextProvider.StartTimeProvider = new StartTimeProvider();

            RemoteClientsOperator rlcOperator = new RemoteClientsOperator(controller.CurrentProject, controller.Server, commandContextProvider);
            controller.Server.StartReceiving();


            Console.ReadKey();
        }
    }
}
