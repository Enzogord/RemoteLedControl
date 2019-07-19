using Core.Messages;

namespace Core.RemoteOperations
{
    public interface IMessageReceiver
    {
        void Receive(RLCMessage message);
    }
}
