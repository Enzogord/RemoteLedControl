using System;
using System.Net;

namespace RLCCore.Settings
{
    public class NetworkAddressSetting
    {
        public IPAddress IPAddress { get; }
        public IPAddress SubNetMask { get; }
        public IPAddress BroadcastAddress { get; }

        public NetworkAddressSetting(IPAddress ipAddress, IPAddress subNetMask)
        {
            this.IPAddress = ipAddress;
            this.SubNetMask = subNetMask;
            this.BroadcastAddress = GetBroadcastAddress(ipAddress, subNetMask);
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

        public override string ToString()
        {
            return IPAddress.ToString();
        }
    }
}
