using System.Collections.Concurrent;
using EventHubFramework.Common;
using EventHubFramework.Signal;
using EventHubFramework.Signal.Factory;

namespace EventHubFramework;

public static class EventBus
{
    private static readonly ConcurrentDictionary<Type, object> _map = new ConcurrentDictionary<Type, object>();

    /// <summary>
    /// Subscribing To Signal, with sync execution
    /// </summary>
    /// <param name="subscriber"></param>
    /// <param name="method"></param>
    /// <typeparam name="TSignal"></typeparam>
    /// <exception cref="EventBusException"></exception>
    public static void Subscribe<TSignal>(this object subscriber, Action<TSignal> method)
        where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");

        var signal = _map[type] as Signal<TSignal>;
        signal.Subscribe(subscriber, method);
    }

    /// <summary>
    /// Subscribing To Signal, with async execution
    /// </summary>
    /// <param name="subscriber"></param>
    /// <param name="method"></param>
    /// <typeparam name="TSignal"></typeparam>
    /// <exception cref="EventBusException"></exception>
    public static void Subscribe<TSignal>(this object subscriber, Func<TSignal, Task> method)
        where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");

        var signal = _map[type] as Signal<TSignal>;
        signal.Subscribe(subscriber, method);
    }

    /// <summary>
    /// UnSubscribing To Signal, with sync execution
    /// </summary>
    /// <param name="subscriber"></param>
    /// <param name="method"></param>
    /// <typeparam name="TSignal"></typeparam>
    /// <exception cref="EventBusException"></exception>
    public static void UnSubscribe<TSignal>(this object subscriber, Action<TSignal> method)
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");

        var signal = _map[type] as Signal<TSignal>;
        signal.UnSubscribe(subscriber, method);
    }

    /// <summary>
    /// UnSubscribing To Signal, with async execution
    /// </summary>
    /// <param name="subscriber"></param>
    /// <param name="method"></param>
    /// <typeparam name="TSignal"></typeparam>
    /// <exception cref="EventBusException"></exception>
    public static void UnSubscribe<TSignal>(this object subscriber, Func<TSignal, Task> method)
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");

        var signal = _map[type] as Signal<TSignal>;
        signal.UnSubscribe(subscriber, method);
    }

    /// <summary>
    /// Registering Signal
    /// </summary>
    /// <typeparam name="TSignal"></typeparam>
    /// <returns>Current Signal Class</returns>
    public static TSignal RegisterSignal<TSignal>() where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        var signal = SignalFactory.Create<TSignal>().Invoke();
        _map.TryAdd(type, signal);
        return signal;
    }

    /// <summary>
    /// Firing Signal Without Params
    /// </summary>
    /// <typeparam name="TSignal"></typeparam>
    /// <exception cref="EventBusException"></exception>
    public static void Fire<TSignal>() where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");

        var signal = _map[type] as Signal<TSignal>;
        signal.Fire();
    }
    
    /// <summary>
    /// Firing Signal With Params
    /// </summary>
    /// <typeparam name="TSignal"></typeparam>
    /// <exception cref="EventBusException"></exception>
    public static void Fire<TSignal>(Func<TSignal> action) where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");

        var signal = _map[type] as Signal<TSignal>;
        signal.Fire(action.Invoke());
    }

    
    /// <summary>
    /// Try Fire Signal without params, when attempting to fire a signal, if an exception occurs during the firing process, the exception will not occur.
    /// </summary>
    /// <typeparam name="TSignal"></typeparam>
    public static void TryFire<TSignal>() where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            return;

        var signal = _map[type] as Signal<TSignal>;
        signal.Fire();
    }

    /// <summary>
    /// Async Firing Signal without params
    /// </summary>
    /// <param name="currentSignal"></param>
    /// <typeparam name="TSignal"></typeparam>
    /// <exception cref="EventBusException"></exception>
    public static async Task FireAsync<TSignal>() where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");


        var signal = _map[type] as Signal<TSignal>;
        await signal.FireAsync();
    }
    
    /// <summary>
    /// Async Try Fire Signal without Params, when attempting to fire a signal, if an exception occurs during the firing process, the exception will not occur.
    /// </summary>
    /// <typeparam name="TSignal"></typeparam>
    public static async void TryFireAsync<TSignal>()
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            return;

        var signal = _map[type] as Signal<TSignal>;
        await signal.FireAsync();
    }

    /// <summary>
    /// Async Firing Signal with params
    /// </summary>
    /// <param name="currentSignal"></param>
    /// <typeparam name="TSignal"></typeparam>
    /// <exception cref="EventBusException"></exception>
    public static async Task FireAsync<TSignal>(Func<TSignal> currentSignal) where TSignal : Signal<TSignal>
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");


        var signal = _map[type] as Signal<TSignal>;
        await signal.FireAsync(currentSignal.Invoke());
    }
    
    /// <summary>
    /// Async Try Fire Signal with Params, when attempting to fire a signal, if an exception occurs during the firing process, the exception will not occur.
    /// </summary>
    /// <typeparam name="TSignal"></typeparam>
    public static async void TryFireAsync<TSignal>(Func<TSignal> currentSignal)
    {
        var type = typeof(TSignal);
        if (!_map.ContainsKey(type))
            return;

        var signal = _map[type] as Signal<TSignal>;
        await signal.FireAsync(currentSignal.Invoke());
    }
}