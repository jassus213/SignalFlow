namespace EventHubFramework.Signal;

public abstract class Signal<TSignal>
{
    private readonly TSignal _currentSignal;

    private readonly Dictionary<string, object> _subscribers = new Dictionary<string, object>();

    private readonly Dictionary<string, Action<TSignal>> _syncSubscribers =
        new Dictionary<string, Action<TSignal>>();

    private readonly Dictionary<string, Func<TSignal, Task>> _queueAsyncSubscribers =
        new Dictionary<string, Func<TSignal, Task>>();

    internal void Subscribe(object subscriber, Action<TSignal> action)
    {
        var hash = subscriber.GetHashCode().ToString();
        if (_syncSubscribers.ContainsKey(hash))
            throw new Exception($"Already Subscribed {hash}");
        
        Subscribe(hash, subscriber);
        _syncSubscribers.Add(hash, action);
    }
    
    
    internal void Subscribe(object subscriber, Func<TSignal, Task> action)
    {
        var hash = subscriber.GetHashCode().ToString();
        if (_syncSubscribers.ContainsKey(hash))
            throw new Exception($"Already Subscribed {subscriber.GetType()}");
        Subscribe(hash, subscriber);
        _queueAsyncSubscribers.Add(hash, action);
    }

    private void Subscribe(string hash, object subscriber)
    {
        _subscribers.Add(hash, subscriber);
    }

    private void UnSubscribe(string hash)
    {
        _subscribers.Remove(hash);
    }

    internal void UnSubscribe(object subscriber)
    {
        var hash = subscriber.GetHashCode().ToString();
        if (!_syncSubscribers.ContainsKey(hash))
            throw new Exception($"Not Subscribed {subscriber.GetType()}");
        
        UnSubscribe(hash);
        _syncSubscribers.Remove(hash);
    }

    internal void UnSubscribe(object subscriber, Action<TSignal> method)
    {
        var hash = subscriber.GetHashCode().ToString();
        if (!_syncSubscribers.ContainsKey(hash))
            throw new Exception($"Not Subscribed {subscriber.GetType()}");

        if (_syncSubscribers[hash] == method)
            _syncSubscribers.Remove(hash);
        
        UnSubscribe(hash);
    }
    
    internal void UnSubscribe(object subscriber, Func<TSignal, Task> method)
    {
        var hash = subscriber.GetHashCode().ToString();
        if (!_queueAsyncSubscribers.ContainsKey(hash))
            throw new Exception($"{subscriber} Not Subscribed To {subscriber.GetType()}");

        if (_queueAsyncSubscribers[hash] == method)
            _queueAsyncSubscribers.Remove(hash);
        
        UnSubscribe(hash);
    }

    internal void Fire()
    {
        foreach (var subscriber in _syncSubscribers.Values)
        {
            subscriber.Invoke(_currentSignal);
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
            await subscriber.Invoke(_currentSignal);
        }
    }

    internal async Task FireAsync(TSignal currentSignal)
    {
        foreach (var subscriber in _queueAsyncSubscribers.Values)
        {
            subscriber.Invoke(currentSignal);
        }
    }
}