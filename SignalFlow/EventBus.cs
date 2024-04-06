using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SignalFlow.Abstractions;
using SignalFlow.Exceptions;
using SignalFlow.Signal;

namespace SignalFlow
{
    public class EventBus : IBus
    {
        private readonly ConcurrentDictionary<Type, object> _map = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Subscribing To Signal, with sync execution
        /// </summary>
        /// <param name="method"></param>
        /// <typeparam name="TSignal"></typeparam>
        /// <exception cref="EventBusException"></exception>
        public void Subscribe<TSignal>(Action<TSignal> method) where TSignal : new()
        {
            var type = typeof(TSignal);
            if (!_map.ContainsKey(type))
                throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");

            if (_map[type] is Signal<TSignal> signal) signal.Subscribe(method);
        }

        /// <summary>
        /// Subscribing To Signal, with async execution
        /// </summary>
        /// <param name="method"></param>
        /// <typeparam name="TSignal"></typeparam>
        /// <exception cref="EventBusException"></exception>
        public void Subscribe<TSignal>(Func<TSignal, Task> method) where TSignal : new()
        {
            var type = typeof(TSignal);

            if (_map.TryGetValue(type, out var value))
            {
                var signal = value as Signal<TSignal>;
                signal?.Subscribe(method);
                return;
            }

            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");
        }

        /// <summary>
        /// UnSubscribing To Signal, with sync execution
        /// </summary>
        /// <param name="method"></param>
        /// <typeparam name="TSignal"></typeparam>
        /// <exception cref="EventBusException"></exception>
        public void UnSubscribe<TSignal>(Action<TSignal> method) where TSignal : new()
        {
            var type = typeof(TSignal);

            if (_map.TryGetValue(type, out var value))
            {
                var signal = value as Signal<TSignal>;
                signal?.UnSubscribe(method.GetHashCode().ToString(), method);
                return;
            }

            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");
        }

        /// <summary>
        /// UnSubscribing To Signal, with async execution
        /// </summary>
        /// <param name="method"></param>
        /// <typeparam name="TSignal"></typeparam>
        /// <exception cref="EventBusException"></exception>
        public void UnSubscribe<TSignal>(Func<TSignal, Task> method) where TSignal : new()
        {
            var type = typeof(TSignal);

            if (_map.TryGetValue(type, out var value))
            {
                var signal = value as Signal<TSignal>;
                signal?.UnSubscribe(method.GetHashCode().ToString(), method);
                return;
            }

            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");
        }

        /// <summary>
        /// Registering Signal
        /// </summary>
        /// <typeparam name="TSignal"></typeparam>
        /// <returns>Current Signal Class</returns>
        public TSignal Register<TSignal>() where TSignal : new()
        {
            var type = typeof(TSignal);
            var signal = new TSignal();
            _map.TryAdd(type, signal);
            return signal;
        }

        /// <summary>
        /// UnRegistering Signal
        /// </summary>
        /// <typeparam name="TSignal"></typeparam>
        /// <returns>Current Signal Class</returns>
        public TSignal UnRegister<TSignal>() where TSignal : new()
        {
            var type = typeof(TSignal);
            _map.TryRemove(type, out var signal);
            return signal is TSignal value ? value : default;
        }

        /// <summary>
        /// Firing Signal Without Params
        /// </summary>
        /// <typeparam name="TSignal"></typeparam>
        /// <exception cref="EventBusException"></exception>
        public void Fire<TSignal>() where TSignal : new()
        {
            var type = typeof(TSignal);

            if (_map.TryGetValue(type, out var value))
            {
                var signal = value as Signal<TSignal>;
                signal?.Fire();
                return;
            }

            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");
        }

        /// <summary>
        /// Firing Signal With Params
        /// </summary>
        /// <typeparam name="TSignal"></typeparam>
        /// <exception cref="EventBusException"></exception>
        public void Fire<TSignal>(Func<TSignal> action) where TSignal : new()
        {
            var type = typeof(TSignal);

            if (_map.TryGetValue(type, out var value))
            {
                var signal = _map[type] as Signal<TSignal>;
                signal?.Fire(action.Invoke());
                return;
            }

            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");
        }


        /// <summary>
        /// Try Fire Signal without params, when attempting to fire a signal, if an exception occurs during the firing process, the exception will not occur.
        /// </summary>
        /// <typeparam name="TSignal"></typeparam>
        public void TryFire<TSignal>() where TSignal : new()
        {
            var type = typeof(TSignal);

            if (_map.TryGetValue(type, out var value))
            {
                var signal = value as Signal<TSignal>;
                signal?.Fire();
            }
        }

        /// <summary>
        /// Try Fire Signal with params, when attempting to fire a signal, if an exception occurs during the firing process, the exception will not occur.
        /// </summary>
        /// <typeparam name="TSignal"></typeparam>
        public void TryFire<TSignal>(Func<TSignal> action) where TSignal : new()
        {
            var type = typeof(TSignal);

            if (_map.TryGetValue(type, out var value))
            {
                var signal = value as Signal<TSignal>;
                signal?.Fire();
            }
        }

        /// <summary>
        /// Async Firing Signal without params
        /// </summary>
        /// <typeparam name="TSignal"></typeparam>
        /// <exception cref="EventBusException"></exception>
        public async Task FireAsync<TSignal>() where TSignal : new()
        {
            var type = typeof(TSignal);

            if (_map.TryGetValue(type, out var value))
            {
                var signal = value as Signal<TSignal>;
                await signal?.FireAsync()!;
            }

            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");
        }

        /// <summary>
        /// Async Try Fire Signal without Params, when attempting to fire a signal, if an exception occurs during the firing process, the exception will not occur.
        /// </summary>
        /// <typeparam name="TSignal"></typeparam>
        public async void TryFireAsync<TSignal>() where TSignal : new()
        {
            var type = typeof(TSignal);

            if (!_map.TryGetValue(type, out var value)) return;

            var signal = value as Signal<TSignal>;
            await signal?.FireAsync()!;
        }

        /// <summary>
        /// Async Firing Signal with params
        /// </summary>
        /// <param name="currentSignal"></param>
        /// <typeparam name="TSignal"></typeparam>
        /// <exception cref="EventBusException"></exception>
        public async Task FireAsync<TSignal>(Func<TSignal> currentSignal) where TSignal : new()
        {
            var type = typeof(TSignal);

            if (_map.TryGetValue(type, out var value))
            {
                var signal = value as Signal<TSignal>;
                await signal?.FireAsync(currentSignal.Invoke())!;
                return;
            }

            throw new EventBusException($"Cant Find Current Signal {typeof(TSignal)}");
        }

        /// <summary>
        /// Async Try Fire Signal with Params, when attempting to fire a signal, if an exception occurs during the firing process, the exception will not occur.
        /// </summary>
        /// <typeparam name="TSignal"></typeparam>
        public async void TryFireAsync<TSignal>(Func<TSignal> currentSignal) where TSignal : new()
        {
            var type = typeof(TSignal);

            if (_map.TryGetValue(type, out var value))
            {
                var signal = value as Signal<TSignal>;
                await signal?.FireAsync(currentSignal.Invoke())!;
            }
        }
    }
}