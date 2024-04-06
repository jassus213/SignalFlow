using System;
using System.Threading.Tasks;

namespace SignalFlow.Abstractions.Firing
{
    public interface IFireManager
    {
        public void Fire<TSignal>() where TSignal : new();
        public void Fire<TSignal>(Func<TSignal> action) where TSignal : new();

        public Task FireAsync<TSignal>() where TSignal : new();
        public Task FireAsync<TSignal>(Func<TSignal> currentSignal) where TSignal : new();

        void TryFireAsync<TSignal>() where TSignal : new();
        void TryFireAsync<TSignal>(Func<TSignal> currentSignal) where TSignal : new();

        public void TryFire<TSignal>() where TSignal : new();
        public void TryFire<TSignal>(Func<TSignal> action) where TSignal : new();
    }
}