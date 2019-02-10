using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class NetworkAddressSetting
    {
        private IPAddress ipAddress;
        public IPAddress IPAddress => ipAddress;

        private IPAddress subNetMask;
        public IPAddress SubNetMask => subNetMask;

        private IPAddress broadCastAddress;
        public IPAddress BroadCastAddress => broadCastAddress;

        public NetworkAddressSetting(IPAddress ipAddress, IPAddress subNetMask)
        {
            this.ipAddress = ipAddress;
            this.subNetMask = subNetMask;
            this.broadCastAddress = GetBroadcastAddress(ipAddress, subNetMask);
        }

        /// <summary>
        /// Находит широковещательный адрес подсети к которой принадлежит адрес с указанной маской подсети
        /// </summary>
        /// <param name="address">IP адрес по которому необходимо найти широковещательный адрес подсети</param>
        /// <param name="subnetMask">Маска подсети указанного в первом параметре адреса</param>
        /// <returns>Широковещательынй адрес подсети</returns>
        /// <exception cref="ArgumentException"></exception>
        private IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if(ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];

            for(int i = 0; i < broadcastAddress.Length; i++) {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }

            return new IPAddress(broadcastAddress);
        }
    }
}
