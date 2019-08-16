using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNTPService
{
    public static class DateTimeSerializator
    {
        /// <summary>
        /// Represents the EPOCH date in DateTime format.
        /// </summary>
        public static readonly DateTime Epoch = new DateTime(1900, 1, 1);

        /// <summary>
        /// Represents the number of ticks in 1 second.
        /// </summary>
        public  const long TicksPerSecond = TimeSpan.TicksPerSecond;

        public static DateTime TimestampToDateTime(byte[] data)
        {
            ulong seconds = 0;
            for(int i = 0; i <= 3; i++)
                seconds = (seconds << 8) | data[0 + i];
            ulong fractions = 0;
            for(int i = 4; i <= 7; i++)
                fractions = (fractions << 8) | data[0 + i];
            ulong ticks = (seconds * TicksPerSecond) + ((fractions * TicksPerSecond) / 0x100000000L);
            return (Epoch + TimeSpan.FromTicks((long)ticks));
        }

        public static void DateTimeToTimestamp(DateTime dateTime, byte[] data)
        {
            ulong ticks = (ulong)(dateTime - Epoch).Ticks;
            ulong seconds = ticks / TicksPerSecond;
            Console.WriteLine(seconds);
            ulong fractions = ((ticks % TicksPerSecond) * 0x100000000L) / TicksPerSecond;
            for(int i = 3; i >= 0; i--) {
                data[0 + i] = (byte)seconds;
                seconds = seconds >> 8;
            }
            for(int i = 7; i >= 4; i--) {
                data[0 + i] = (byte)fractions;
                fractions = fractions >> 8;
            }
        }
    }
}
