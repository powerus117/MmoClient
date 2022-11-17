using System;
using System.Collections.Generic;
using MmoShared.Messages;

namespace Core.Connection.Messages
{
    public class MessageSubscription<T> : IMessageSubscription
        where T : Message
    {
        private readonly HashSet<Action<T>> _handlers = new HashSet<Action<T>>();

        public void Add(Action<T> handler)
        {
            _handlers.Add(handler);
        }
        
        public void Remove(Action<T> handler)
        {
            _handlers.Remove(handler);
        }

        public void Invoke(Message message)
        {
            foreach (var handler in _handlers)
            {
                handler?.Invoke((T)message);
            }
        }
    }
}