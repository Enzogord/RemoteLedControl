namespace RLCCore.Transport
{
    public interface IGlobalCommandsContextProvider
    {
        ICommandContext GetCommandContext(GlobalCommands command);
    }
}