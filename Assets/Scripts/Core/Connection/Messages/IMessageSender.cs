using MmoShared.Messages;

namespace Core.Connection.Messages
{
    public interface IMessageSender
    {
        void Send(Message message);
    }
}