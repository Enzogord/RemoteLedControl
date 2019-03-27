using System;
using System.Net;
using RLCCore.Transport;

namespace RLCCore.RemoteOperations
{
    public static class CommandContextFactory
    {
        public static ICommandContext CreateGlobalContext(GlobalCommands command)
        {
            switch(command) {
                case GlobalCommands.Play:
                    return GlobalCommandContext.Empty();
                case GlobalCommands.Stop:
                    return GlobalCommandContext.Empty();
                case GlobalCommands.Pause:
                    return GlobalCommandContext.Empty();
                case GlobalCommands.PlayFrom:
                    //Тест.
                    //TODO Взять правильное время
                    return new TimeAsSecondsGlobalCommandContext(TimeSpan.FromSeconds(100));
                case GlobalCommands.SendServerAddress:
                    //Тест.
                    //TODO Взять правильное время
                    return new IPAddressGlobalCommandContext(IPAddress.Any);
                default:
                    return GlobalCommandContext.Empty();
            }
        }
    }
}
