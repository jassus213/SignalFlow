namespace EventHubFramework.Signal;

public abstract class Signal<TSignal>
{
    private readonly TSignal _currentSignal;

    private readonly Dictionary<Type, KeyValuePair<object, Action<TSignal>>> _subscribers =
        new Dictionary<Type, KeyValuePair<object, Action<TSignal>>>();

    private readonly Dictionary<Type, KeyValuePair<object, Func<TSignal, Task>>> _queueAsyncSubscribers =
        new Dictionary<Type, KeyValuePair<object, Func<TSignal, Task>>>();

    internal void Subscribe(object subscriber, Action<TSignal> action)
    {
        var type = subscriber.GetType();
        if (_subscribers.ContainsKey(type))
            throw new Exception($"Already Subscribed {type}");

        _subscribers.Add(type, new KeyValuePair<object, Action<TSignal>>(subscriber, action));
    }
    
    
    internal void Subscribe(object subscriber, Func<TSignal, Task> action)
    {
        var type = subscriber.GetType();
        if (_subscribers.ContainsKey(type))
            throw new Exception($"Already Subscribed {type}");

        _queueAsyncSubscribers.Add(type, new KeyValuePair<object, Func<TSignal, Task>>(subscriber, action));
    }

    internal void UnSubscribe(object subscriber)
    {
        var type = subscriber.GetType();
        if (!_subscribers.ContainsKey(type))
            throw new Exception($"Not Subscribed {type}");

        _subscribers.Remove(type);
    }

    internal void UnSubscribe(object subscriber, Action<TSignal> method)
    {
        var type = subscriber.GetType();
        if (!_subscribers.ContainsKey(type))
            throw new Exception($"Not Subscribed {type}");

        if (_subscribers[type].Value == method)
            _subscribers.Remove(type);
    }
    
    internal void UnSubscribe(object subscriber, Func<TSignal, Task> method)
    {
        var type = subscriber.GetType();
        if (!_subscribers.ContainsKey(type))
            throw new Exception($"Not Subscribed {type}");

        if (_queueAsyncSubscribers[type].Value == method)
            _queueAsyncSubscribers.Remove(type);
    }

    internal void Fire()
    {
        foreach (var subscriber in _subscribers.Values)
        {
            subscriber.Value.Invoke(_currentSignal);
        }
    }

    internal void Fire(TSignal currentSignal)
    {
        foreach (var subscriber in _subscribers.Values)
        {
            subscriber.Value.Invoke(currentSignal);
        }
    }
    
    internal async Task FireAsync()
    {
        foreach (var subscriber in _queueAsyncSubscribers.Values)
        {
            await subscriber.Value.Invoke(_currentSignal);
        }
    }

    internal async Task FireAsync(TSignal currentSignal)
    {
        foreach (var subscriber in _queueAsyncSubscribers.Values)
        {
            await subscriber.Value.Invoke(currentSignal);
        }
    }
}