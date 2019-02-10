using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Service
{
    public static class ServiceFunctions
    {
        public static IEnumerable<DriveInfo> GetRemovableDrives()
        {
            return DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Removable);
        }

        public static IEnumerable<IPAddress> GetCurrentMachineIPAddresses()
        {
            string myHost = Dns.GetHostName();
            return Dns.GetHostEntry(myHost).AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork);           
        }
    }
}
