using System;
using System.Net;

namespace RLCCore.Transport
{
    public abstract class GlobalCommandContext : ICommandContext
    {
        public static GlobalCommandContext Empty()
        {
            return new EmptyGlobalCommandContext();
        }

        public abstract byte[] GetBytes();
    }

    public class EmptyGlobalCommandContext : GlobalCommandContext
    {
        public override byte[] GetBytes()
        {
            return new byte[0];
        }
    }

    public class IPAddressGlobalCommandContext : GlobalCommandContext
    {
        private readonly IPAddress ipAddress;

        public IPAddressGlobalCommandContext(IPAddress ipAddress)
        {
            this.ipAddress = ipAddress;
        }

        public override byte[] GetBytes()
        {
            return ipAddress.GetAddressBytes();
        }
    }

    public class TimeAsSecondsGlobalCommandContext : GlobalCommandContext
    {
        private readonly TimeSpan time;

        public TimeAsSecondsGlobalCommandContext(TimeSpan time)
        {
            this.time = time;
        }

        public override byte[] GetBytes()
        {
            return BitConverter.GetBytes((int)time.TotalSeconds);
        }
    }
}
