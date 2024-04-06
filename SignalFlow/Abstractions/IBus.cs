using SignalFlow.Abstractions.Firing;

namespace SignalFlow.Abstractions
{
    public interface IBus : ISubscriptionManager, ISignalManager, IFireManager
    {
    }
}