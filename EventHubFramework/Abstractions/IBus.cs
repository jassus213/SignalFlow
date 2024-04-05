using System;
using EventHubFramework.Abstractions.Firing;

namespace EventHubFramework.Abstractions
{
    public interface IBus : ISubscriptionManager, ISignalManager, IFireManager
    {
    }
}