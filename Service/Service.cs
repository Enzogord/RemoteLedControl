using System;
using System.Net;

namespace Service
{
    public static class Service
    {
        /// <summary>
        /// Находит к какой подсети принадлежит адрес с указанной маской подсети
        /// </summary>
        /// <param name="address">IP адрес по которому необходимо найти адрес подсети</param>
        /// <param name="subnetMask">Маска подсети указанного в первом параметре адреса</param>
        /// <returns>Адрес подсети</returns>
        /// <exception cref="ArgumentException"></exception>
        private static IPAddress GetNetworkAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] networkAddress = new byte[ipAdressBytes.Length];

            for (int i = 0; i < networkAddress.Length; i++)
            {
                networkAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }

            return new IPAddress(networkAddress);
        }

        /// <summary>
        /// Находит широковещательный адрес подсети к которой принадлежит адрес с указанной маской подсети
        /// </summary>
        /// <param name="address">IP адрес по которому необходимо найти широковещательный адрес подсети</param>
        /// <param name="subnetMask">Маска подсети указанного в первом параметре адреса</param>
        /// <returns>Широковещательынй адрес подсети</returns>
        /// <exception cref="ArgumentException"></exception>
        private static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];

            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }

            return new IPAddress(broadcastAddress);
        }
    }
}
