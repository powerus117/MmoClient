using MmoShared.Messages;

namespace Core.Connection.Messages
{
    public interface IMessageSubscription
    {
        public void Invoke(Message message);
    }
}