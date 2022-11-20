using System;

namespace Core.Signals
{
    public interface ISignalManager
    {
        void Subscribe<T>(Action<T> handler) where T : ISignal;
        void Unsubscribe<T>(Action<T> handler) where T : ISignal;
        void Send<T>(T signal) where T : ISignal;
    }
}