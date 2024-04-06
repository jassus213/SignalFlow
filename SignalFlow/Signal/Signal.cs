using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalFlow.Signal
{
    public abstract class Signal<TSignal> where TSignal : new()
    {
        private readonly Dictionary<string, Action<TSignal>> _syncSubscribers =
            new Dictionary<string, Action<TSignal>>();

        private readonly Dictionary<string, Func<TSignal, Task>> _queueAsyncSubscribers =
            new Dictionary<string, Func<TSignal, Task>>();

        internal void Subscribe(Action<TSignal> action)
        {
            var hash = action.GetHashCode().ToString();
            if (_syncSubscribers.ContainsKey(hash))
                throw new Exception($"Already Subscribed {hash}");

            Subscribe(hash, action);
        }

        internal void Subscribe(Func<TSignal, Task> action)
        {
            var hash = action.GetHashCode().ToString();
            if (_syncSubscribers.ContainsKey(hash))
                throw new Exception($"Already Subscribed {action.GetType()}");
            Subscribe(hash, action);
        }

        private void Subscribe(string hash, Action<TSignal> action) =>
            _syncSubscribers.Add(hash, action);

        private void Subscribe(string hash, Func<TSignal, Task> action) =>
            _queueAsyncSubscribers.Add(hash, action);


        private void UnSubscribe(string hash) =>
            _syncSubscribers.Remove(hash);

        private void UnSubscribeAsync(string hash) =>
            _queueAsyncSubscribers.Remove(hash);

        internal void UnSubscribe(string hash, Action<TSignal> method)
        {
            if (!_syncSubscribers.ContainsKey(hash))
                throw new Exception($"Not Subscribed");

            if (_syncSubscribers[hash] == method)
                UnSubscribe(hash);
        }

        internal void UnSubscribe(string hash, Func<TSignal, Task> method)
        {
            if (!_queueAsyncSubscribers.ContainsKey(hash))
                throw new Exception($"{this} Is Have Not Subscribe To Method {method}");

            if (_queueAsyncSubscribers[hash] == method)
                UnSubscribeAsync(hash);
        }

        internal void Fire()
        {
            foreach (var subscriber in _syncSubscribers.Values)
            {
                subscriber.Invoke(new TSignal());
            }
        }

        internal void Fire(TSignal currentSignal)
        {
            foreach (var subscriber in _syncSubscribers.Values)
            {
                subscriber.Invoke(currentSignal);
            }
        }

        internal async Task FireAsync()
        {
            foreach (var subscriber in _queueAsyncSubscribers.Values)
            {
                await subscriber.Invoke(new TSignal());
            }
        }

        internal async Task FireAsync(TSignal currentSignal)
        {
            foreach (var subscriber in _queueAsyncSubscribers.Values)
            {
                await subscriber.Invoke(currentSignal);
            }
        }
    }
}