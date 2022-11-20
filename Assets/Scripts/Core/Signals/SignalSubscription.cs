using System;
using System.Collections.Generic;

namespace Core.Signals
{
    public class SignalSubscription<T> : ISignalSubscription
        where T : ISignal
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

        public void Invoke(ISignal message)
        {
            foreach (var handler in _handlers)
            {
                handler?.Invoke((T)message);
            }
        }
    }
}