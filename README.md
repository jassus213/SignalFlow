# EventHubFramework

![Last Version : 2.1.4](https://img.shields.io/badge/release-2.1.4-green)
![Last Commit : April 2024](https://img.shields.io/badge/last_commit-april_2024-blue)
![License](https://img.shields.io/badge/License-MIT-green)

## Introduction

The EventBus repository is an implementation of an event bus for managing and distributing events within an application.
It provides functionality for subscribing to and unsubscribing from events, as well as firing events and registering new
event signals. The EventBus class is a static class, meaning it can be accessed globally without the need for an
instance.

## Table of Contents

- [Installation](#installation)
    - [.NET CLI](#installcli)
    - [NuGet Package Referense](#installnugetpackage)
    - [PackageReference (MSBuild)](#installmsbuild)
- [Usage](#usage)
    - [Registering Event Signals](#registering)
    - [Subscribing to Events](#subscribing)
        - [Subscribing To Sync Events](#subscribingsync)
        - [Subscribing To Async Events](#subscribingasync)
    - [Unsubscribing from Events](#unsubscribing)
    - [Firing](#firing)
    - [Firing Events with Arguments](#firingwitharguments)
    - [Async Firing](#asyncfiring)
- [Examples](#examples)
- [Contributing](#contributing)

# <a id="installation"/> Installation

## <a id="installcli"/> Using .NET CLI

To install the EventHubFramework NuGet package using the .NET CLI, open the command-line interface and execute the
following command:

```c#
dotnet add package EventHubFramework --version 2.1.3
```

## <a id="installnugetpackage"/> Using NuGet Package Manager Console

If you prefer using the NuGet Package Manager Console, you can execute the following command:

```c#
Install-Package EventHubFramework -Version 2.1.3
```

## <a id="installmsbuild"/> Using PackageReference (MSBuild)

For projects that use MSBuild and PackageReference format, you can add the following line to your project file:

```c#
<PackageReference Include="EventHubFramework" Version="2.1.3" />
```

# <a id="usage"/> Usage

## <a id="registering"> Registering and Unregistering Event Signals

### Registering

You can register new event signals dynamically using the `RegisterSignal<TSignal>` method. This method takes a type
parameter `TSignal`, which represents the event signal to be registered.

```c#
var bus = new EventBus();
bus.Register<ExampleSignal>();
```

After registering an event signal, you can subscribe, unsubscribe, and fire events using that signal.

### Unregistering

To unregister an event signal, use the UnRegister<TSignal> method. This method removes the specified event signal type
TSignal from the list of registered event signals.

```c#
var bus = new EventBus();
bus.UnRegister<ExampleSignal>();
```

In this example, ExampleSignal is unregistered from the EventBus instance bus. The UnRegister<TSignal> method is useful
for dynamically managing the set of event signals that the EventBus instance is configured to handle.

## <a id="subscribing"> Subscribing to Events

### <a id="subscribingsync"/> Subscribing To Synchronous Events

To subscribe to an event, you can use the `Subscribe<TSignal>` method. This method takes a type parameter `TSignal`,
which represents the event signal you want to subscribe to. The event signal should be a class or interface that defines
the structure of the event.

```c#
bus.Subscribe<ExampleSignal>(HandleEvent);
```

The HandleMyEvent method is the event handler that will be called when the event is fired. It should have a compatible
signature with the event signal.

### <a id="subscribingasync"/> Subscribing to Asynchronous Signals

To subscribe to an asynchronous signal, you can use the Subscribe<TSignal> method in the following format:

```c#
bus.Subscribe<ExampleSignal>(HandleEventAsync);
// where Method Is
private async Task HandleEventAsync(ExampleSignal signal)
{
    await Task.Delay(5000);
    Console.WriteLine(signal.Id);
}
```

Here, TestSubscribe is an asynchronous method that returns a Task. It should have a compatible signature with the event
signal TestSignal.

## <a id="unsubscribing"> Unsubscribing from Events

**Please note that unsubscribing from signals is crucial**. To do this, implement the `IDisposable` interface and
unsubscribe from all the signals to which the object is subscribed in its `Dispose` method.

```c#
public class BarReceiver : IDisposable
{
    private readonly IBus _bus;

    public BarReceiver(IBus bus)
    {
        _bus = bus;

        _bus.Subscribe<Examples.ExampleSignal>(HandleEvent);
    }

    private void HandleEvent(Examples.ExampleSignal signal) =>
        Console.WriteLine(signal.Id);

    public void Dispose()
    {
        _bus.UnSubscribe<Examples.ExampleSignal>(HandleEvent);
    }
}

// OR if Async Signal

public class BarReceiver : IDisposable
{
    private readonly IBus _bus;

    public BarReceiver(IBus bus)
    {
        _bus = bus;

        _bus.Subscribe<Examples.ExampleSignal>(HandleEvent);
    }

    private async Task HandleEvent(Examples.ExampleSignal signal)
    {
        // Some Async Code
        await Task.Delay(2000);
        
        Console.WriteLine(signal.Id);
    }
        

    public void Dispose()
    {
        _bus.UnSubscribe<Examples.ExampleSignal>(HandleEvent);
    }
}

```

To unsubscribe from an event, you can use the `UnSubscribe<TSignal>` method. This method takes the same type
parameter `TSignal` as the Subscribe method.

```c#
bus.UnSubscribe<Examples.ExampleSignal>(HandleEvent);
```

## <a id="firing"> Firing Events

To fire an event, you can use the Fire<TSignal> method. This method takes a type parameter TSignal, representing the
event signal to be fired.

```c#
bus.Fire<ExampleSignal>();
// Or, use TryFire if you do not want to receive an exception in case it occurs.
// Please note that if an exception occurs and you are not in Debug Mode, you will not be able to know about it.
bus.TryFire<ExampleSignal>();
```

All the event handlers that have subscribed to the specified event signal will be called.

## <a id="firingwitharguments"/> Firing Events with Arguments

To fire an event with arguments, you can use the `Fire<TSignal>` or `TryFire<TSignal>` method variants that accept
a `Func<TSignal>` parameter. This function should return an instance of the event signal with the desired argument
values.

```c#
bus.Fire(() => new ExampleSignal() { Id = 10 });
// Or, use TryFire if you do not want to receive an exception in case it occurs.
bus.TryFire(() => new ExampleSignal() { Id = 10 });
```

## <a id="asyncfiring"/> Async Firing

To fire an asynchronous signal, you can use the FireAsync method. It can accept an instance of the event signal TSignal
or no arguments. The method returns a Task that represents the asynchronous operation.

```c#
await _bus.FireAsync<Examples.ExampleSignal>();
```

You can also pass an instance of the event signal as a parameter:

```c#
await _bus.FireAsync<Examples.ExampleSignal>(() => new Examples.ExampleSignal()
{
    Id = 10
});
// If you dont want awaiting signal
```

# <a id="examples"/> Examples

Here are some examples of how you can use the EventBus:

```c#
// Subscribe to an event
bus.Subscribe<ExampleSignal>(HandleEvent);

// Unsubscribe from an event
bus.UnSubscribe<ExampleSignal>(HandleEvent);

// Register a new event signal
bus.Register<ExampleSignal>();

// Fire an event
bus.Fire(() => new ExampleSignal() { Id = 10 });
// Or
bus.TryFire(() => new ExampleSignal() { Id = 10 });
```

In the above example, MyClass subscribes to the TestSignal event with the asynchronous method TestSubscribe. When the
event is fired, TestSubscribe is invoked asynchronously to handle the event.
Remember to implement the IDisposable interface if you need to unsubscribe from signals when the object is disposed to
avoid memory leaks.
That's it! You can now leverage the power of asynchronous signals in the EventBus repository to handle events
asynchronously in your application.

# <a id="contributing"> Contributing

Contributions to the EventBus repository are welcome! If you find any issues or have suggestions for improvements,
please open an issue or submit a pull request on the GitHub repository.
