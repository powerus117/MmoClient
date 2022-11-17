using System;
using MmoShared.Messages;

namespace Core.Connection.Messages
{
    public interface IMessageReceiver
    {
        void Subscribe<T>(Action<T> handler) where T : Message;
        void Unsubscribe<T>(Action<T> handler) where T : Message;
        void Send<T>(T signal) where T : Message;
    }
}