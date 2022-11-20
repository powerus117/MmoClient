using System;
using System.Collections.Generic;
using Core.Connection.Messages;

namespace Core.Signals
{
    public class SignalManager : ISignalManager
    {
        private readonly Dictionary<Type, ISignalSubscription> _subscriptions = new Dictionary<Type, ISignalSubscription>();

        public void Subscribe<T>(Action<T> handler)
            where T : ISignal
        {
            if (!_subscriptions.TryGetValue(typeof(T), out var subscriptionInfo))
            {
                _subscriptions.Add(typeof(T), subscriptionInfo = new SignalSubscription<T>());
            }
            
            ((SignalSubscription<T>)subscriptionInfo).Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
            where T : ISignal
        {
            if (_subscriptions.TryGetValue(typeof(T), out var subscriptionInfo))
            {
                ((SignalSubscription<T>)subscriptionInfo).Remove(handler);
            }
        }

        public void Send<T>(T signal)
            where T : ISignal
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