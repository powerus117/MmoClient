using System;
using System.Collections.Generic;
using MmoShared.Messages;

namespace Core.Connection.Messages
{
    public class MessageReceiver : IMessageReceiver
    {
        private readonly Dictionary<Type, IMessageSubscription> _subscriptions = new Dictionary<Type, IMessageSubscription>();

        public void Subscribe<T>(Action<T> handler)
            where T : Message
        {
            if (!_subscriptions.TryGetValue(typeof(T), out var subscriptionInfo))
            {
                _subscriptions.Add(typeof(T), subscriptionInfo = new MessageSubscription<T>());
            }
            
            ((MessageSubscription<T>)subscriptionInfo).Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
            where T : Message
        {
            if (_subscriptions.TryGetValue(typeof(T), out var subscriptionInfo))
            {
                ((MessageSubscription<T>)subscriptionInfo).Remove(handler);
            }
        }

        public void Send<T>(T signal)
            where T : Message
        {
            if (_subscriptions.TryGetValue(signal.GetType(), out var subscriptionInfo))
            {
                try
                {
                    subscriptionInfo.Invoke(signal);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}