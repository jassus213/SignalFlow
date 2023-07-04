# EventBus
## Introduction
The EventBus repository is an implementation of an event bus for managing and distributing events within an application. It provides functionality for subscribing to and unsubscribing from events, as well as firing events and registering new event signals. The EventBus class is a static class, meaning it can be accessed globally without the need for an instance.
## Table of Contents
- [Installation](#installation)
- [Usage](#usage)
    - [Registering Event Signals](#registering)
    - [Subscribing to Events](#subscribing)
    - [Unsubscribing from Events](#unsubscribing)
    - [Firing](#firing)
    - [Firing Events with Arguments](#firingwitharguments)
- [Examples](#examples)
- [Contributing](#contributing)

# <a id="installation"/> Installation

# <a id="usage"/> Usage
## <a id="registering"> Registering Event Signals
You can register new event signals dynamically using the `RegisterSignal<TSignal>` method. This method takes a type parameter `TSignal`, which represents the event signal to be registered.
```c#
EventBus.RegisterSignal<MyEventSignal>();
```
After registering an event signal, you can subscribe, unsubscribe, and fire events using that signal.
## <a id="subscribing"> Subscribing to Events
To subscribe to an event, you can use the `Subscribe<TSignal>` method. This method takes a type parameter `TSignal`, which represents the event signal you want to subscribe to. The event signal should be a class or interface that defines the structure of the event.
```c#
EventBus.Subscribe<MyEventSignal>(this, HandleMyEvent);
// Or
this.Subscribe<MyEventSignal>(HandleMyEvent);
```
The HandleMyEvent method is the event handler that will be called when the event is fired. It should have a compatible signature with the event signal.
## <a id="unsubscribing"> Unsubscribing from Events
**Please note that unsubscribing from signals is crucial**. To do this, implement the `IDisposable` interface and unsubscribe from all the signals to which the object is subscribed in its `Dispose` method.
```c#
public class BarReceiver : IDisposable
{
    public BarReceiver()
    {
        this.Subscribe<TestSignal>(TestSubscribe);
    }

    public void TestSubscribe(TestSignal args)
    {
        Console.WriteLine(args.Float);
    }

    public void Dispose()
    {
        this.UnSubscribe<TestSignal>(TestSubscribe);
    }
}
```

To unsubscribe from an event, you can use the `UnSubscribe<TSignal>` method. This method takes the same type parameter `TSignal` as the Subscribe method.
```c#
EventBus.Unsubscribe<MyEventSignal>(this, HandleMyEvent);
// Or
this.UnSubscribe<MyEventSignal>(HandleMyEvent);
```
## <a id="firing"> Firing Events
To fire an event, you can use the Fire<TSignal> method. This method takes a type parameter TSignal, representing the event signal to be fired.
```c#
EventBus.Fire<MyEventSignal>();
// Or, use TryFire if you do not want to receive an exception in case it occurs.
// Please note that if an exception occurs and you are not in Debug Mode, you will not be able to know about it.
EventBus.TryFire<MyEventSignal>()
```
All the event handlers that have subscribed to the specified event signal will be called.
## <a id="firingwitharguments"/> Firing Events with Arguments
To fire an event with arguments, you can use the `Fire<TSignal>` or `TryFire<TSignal>` method variants that accept a `Func<TSignal>` parameter. This function should return an instance of the event signal with the desired argument values.
```c#
EventBus.Fire<MyEventSignal>(() => new MyEventSignal {Arg1 = 10, Arg2 = string.Empty});
```
# <a id="examples"/> Examples
Here are some examples of how you can use the EventBus:
```c#
// Subscribe to an event
EventBus.Subscribe<MyEventSignal>(this, HandleMyEvent);
// Or
this.Subscribe<MyEventSignal>(HandleMyEvent);

// Unsubscribe from an event
EventBus.UnSubscribe<MyEventSignal>(this, HandleMyEvent);
// Or
this.UnSubscribe<MyEventSignal>(HandleMyEvent);

// Register a new event signal
EventBus.RegisterSignal<MyEventSignal>();

// Fire an event
EventBus.Fire<MyEventSignal>();
// Or
EventBus.TryFire<MyEventSignal>();

// Fire an event with arguments
EventBus.Fire<MyEventSignal>(() => new MyEventSignal {Arg1 = 10};
// Or
EventBus.TryFire<MyEventSignal>(() => new MyEventSignal {Arg1 = 10};
```
# <a id="contributing"> Contributing
Contributions to the EventBus repository are welcome! If you find any issues or have suggestions for improvements, please open an issue or submit a pull request on the GitHub repository.
