using System;

namespace RLCCore.Transport
{
    public class GlobalCommandsContextProvider : IGlobalCommandsContextProvider
    {
        public IStartTimeProvider StartTimeProvider { get; set; }
        public IServerAddressProvider ServerAddressProvider { get; set; }

        public ICommandContext GetCommandContext(GlobalCommands command)
        {
            switch(command) {
                case GlobalCommands.Play:
                    return GlobalCommandContext.Empty();
                case GlobalCommands.Stop:
                    return GlobalCommandContext.Empty();
                case GlobalCommands.Pause:
                    return GlobalCommandContext.Empty();
                case GlobalCommands.PlayFrom:
                    if(StartTimeProvider == null) {
                        throw new ApplicationException($"Не сконфигурирован поставщик стартового времени {nameof(StartTimeProvider)}");
                    }
                    return new TimeAsSecondsGlobalCommandContext(StartTimeProvider.StartTime);
                case GlobalCommands.SendServerAddress:
                    if(ServerAddressProvider == null) {
                        throw new ApplicationException($"Не сконфигурирован поставщик адреса сервера {nameof(ServerAddressProvider)}");
                    }
                    return new IPAddressGlobalCommandContext(ServerAddressProvider.GetServerIPAddress());
                default:
                    throw new NotImplementedException($"Команда не реализована {command}");
            }
        }
    }
}
