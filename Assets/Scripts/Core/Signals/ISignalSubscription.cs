namespace Core.Signals
{
    public interface ISignalSubscription
    {
        public void Invoke(ISignal signal);
    }
}