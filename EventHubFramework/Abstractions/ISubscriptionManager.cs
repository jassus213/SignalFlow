using System;
using System.Threading.Tasks;

namespace EventHubFramework.Abstractions
{
    public interface ISubscriptionManager
    {
        public void Subscribe<TSignal>(Action<TSignal> method)
            where TSignal : new();
        public void Subscribe<TSignal>(Func<TSignal, Task> method) where TSignal : new();
        public void UnSubscribe<TSignal>(Action<TSignal> method) where TSignal : new();
        public void UnSubscribe<TSignal>(Func<TSignal, Task> method) where TSignal : new();
    }
}