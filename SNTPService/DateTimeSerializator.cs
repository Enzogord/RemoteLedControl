using System;

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

        #region DateTime to NTP time

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
            ulong seconds = 0;
            ulong fractions = 0;
            SeparateToNtpTime(dateTime - Epoch, ref seconds, ref fractions);
            for(int i = 3; i >= 0; i--) {
                data[0 + i] = (byte)seconds;
                seconds = seconds >> 8;
            }
            for(int i = 7; i >= 4; i--) {
                data[0 + i] = (byte)fractions;
                fractions = fractions >> 8;
            }
        }

        public static DateTime DeserializeFromNtpTime(byte[] data)
        {
            ulong seconds = 0;
            for(int i = 0; i <= 3; i++) {
                seconds = (seconds << 8) | data[0 + i];
            }
            ulong fractions = 0;
            for(int i = 4; i <= 7; i++) {
                fractions = (fractions << 8) | data[0 + i];
            }
            return CompileFromNtpTime(seconds, fractions);
        }

        public static void SerializeToNtpTime(DateTime dateTime, byte[] data)
        {
            ulong seconds = 0;
            ulong fractions = 0;
            SeparateToNtpTime(dateTime, ref seconds, ref fractions);
            for(int i = 3; i >= 0; i--) {
                data[0 + i] = (byte)seconds;
                seconds = seconds >> 8;
            }
            for(int i = 7; i >= 4; i--) {
                data[0 + i] = (byte)fractions;
                fractions = fractions >> 8;
            }
        }

        #endregion DateTime to NTP time

        public static TimeSpan DeserializeTimeSpanFromNtpTime(byte[] data)
        {
            ulong seconds = 0;
            for(int i = 0; i <= 3; i++) {
                seconds = (seconds << 8) | data[0 + i];
            }
            ulong fractions = 0;
            for(int i = 4; i <= 7; i++) {
                fractions = (fractions << 8) | data[0 + i];
            }
            return CompileTimeSpanFromNtpTime(seconds, fractions);
        }

        public static void SerializeTimeSpanToNtpTime(TimeSpan dateTime, byte[] data)
        {
            ulong seconds = 0;
            ulong fractions = 0;
            SeparateToNtpTime(dateTime, ref seconds, ref fractions);
            for(int i = 3; i >= 0; i--) {
                data[0 + i] = (byte)seconds;
                seconds = seconds >> 8;
            }
            for(int i = 7; i >= 4; i--) {
                data[0 + i] = (byte)fractions;
                fractions = fractions >> 8;
            }
        }

        #region TimeSpan to NTP time

        #endregion TimeSpan to NTP time

        private static DateTime CompileFromNtpTime(ulong seconds, ulong fractions)
        {
            ulong ticks = (seconds * TicksPerSecond) + ((fractions * TicksPerSecond) / 0x100000000L);
            return new DateTime((long)ticks);
        }

        private static TimeSpan CompileTimeSpanFromNtpTime(ulong seconds, ulong fractions)
        {
            ulong ticks = (seconds * TicksPerSecond) + ((fractions * TicksPerSecond) / 0x100000000L);
            return new TimeSpan((long)ticks);
        }

        private static void SeparateToNtpTime(DateTime time, ref ulong seconds, ref ulong fractions)
        {
            ulong ticks = (ulong)time.Ticks;
            SeparateToNtpTime(ticks, ref seconds, ref fractions);
        }

        private static void SeparateToNtpTime(TimeSpan time, ref ulong seconds, ref ulong fractions)
        {
            ulong ticks = (ulong)time.Ticks;
            SeparateToNtpTime(ticks, ref seconds, ref fractions);
        }

        private static void SeparateToNtpTime(ulong ticks, ref ulong seconds, ref ulong fractions)
        {            
            seconds = ticks / TicksPerSecond;
            fractions = ((ticks % TicksPerSecond) * 0x100000000L) / TicksPerSecond;
        }
    }
}
