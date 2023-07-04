namespace EventBus.Signal;

public abstract class Signal<TSignal>
{
    private readonly TSignal _currentSignal;

    private readonly Dictionary<Type, KeyValuePair<object, Action<TSignal>>> Subscribers =
        new Dictionary<Type, KeyValuePair<object, Action<TSignal>>>();

    internal void Subscribe(object subscriber, Action<TSignal> action)
    {
        var type = subscriber.GetType();
        if (Subscribers.ContainsKey(type))
            throw new Exception($"Already Subscribed {type}");

        Subscribers.Add(type, new KeyValuePair<object, Action<TSignal>>(subscriber, action));
    }

    internal void UnSubscribe(object subscriber)
    {
        var type = subscriber.GetType();
        if (!Subscribers.ContainsKey(type))
            throw new Exception($"Not Subscribed {type}");

        Subscribers.Remove(type);
    }

    internal void Fire()
    {
        foreach (var subscriber in Subscribers.Values)
        {
            subscriber.Value.Invoke(_currentSignal);
        }
    }

    internal void Fire(TSignal currentSignal)
    {
        foreach (var subscriber in Subscribers.Values)
        {
            subscriber.Value.Invoke(currentSignal);
        }
    }
}