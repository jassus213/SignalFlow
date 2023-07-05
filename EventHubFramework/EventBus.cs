using System.Collections.Concurrent;
using EventHubFramework.Common;
using EventHubFramework.Signal;
using EventHubFramework.Signal.Factory;

namespace EventHubFramework;

public static class EventBus
{
    private static readonly ConcurrentDictionary<Type, object> _map = new ConcurrentDictionary<Type, object>();

    public static void Subscribe<TSignal>(this object subscriber, Action<TSignal> method)
        where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");

        var signal = _map[type] as Signal<TSignal>;
        signal.Subscribe(subscriber, method);
    }

    public static void Subscribe<TSignal>(this object subscriber, Func<TSignal, Task> method)
        where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");

        var signal = _map[type] as Signal<TSignal>;
        signal.Subscribe(subscriber, method);
    }

    public static void UnSubscribe<TSignal>(this object subscriber, Action<TSignal> method)
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");

        var signal = _map[type] as Signal<TSignal>;
        signal.UnSubscribe(subscriber, method);
    }

    public static void UnSubscribe<TSignal>(this object subscriber, Func<TSignal, Task> method)
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");

        var signal = _map[type] as Signal<TSignal>;
        signal.UnSubscribe(subscriber, method);
    }

    public static TSignal RegisterSignal<TSignal>() where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        var signal = SignalFactory.Create<TSignal>().Invoke();
        _map.TryAdd(type, signal);
        return signal;
    }

    public static void Fire<TSignal>() where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");

        var signal = _map[type] as Signal<TSignal>;
        signal.Fire();
    }

    public static void Fire<TSignal>(Func<TSignal> currentSignal)
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");

        var signal = _map[type] as Signal<TSignal>;
        signal.Fire(currentSignal.Invoke());
    }

    public static void TryFire<TSignal>() where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            return;

        var signal = _map[type] as Signal<TSignal>;
        signal.Fire();
    }

    public static void TryFire<TSignal>(Func<TSignal> currentSignal)
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            return;

        var signal = _map[type] as Signal<TSignal>;
        signal.Fire(currentSignal.Invoke());
    }

    public static async Task FireAsync<TSignal>() where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");


        var signal = _map[type] as Signal<TSignal>;
        await signal.FireAsync();
    }

    public static async Task FireAsync<TSignal>(Func<TSignal> currentSignal) where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");


        var signal = _map[type] as Signal<TSignal>;
        await signal.FireAsync(currentSignal.Invoke());
    }
}