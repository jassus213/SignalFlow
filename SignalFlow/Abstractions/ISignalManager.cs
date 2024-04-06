namespace SignalFlow.Abstractions
{
    public interface ISignalManager
    {
        public TSignal Register<TSignal>() where TSignal : new();
        public TSignal UnRegister<TSignal>() where TSignal : new();
    }
}