# SignalFlow.Microsoft.Extensions.DependencyInjection

![Last Version : 1.0.0](https://img.shields.io/badge/release-1.0.0-green)
![Last Commit : April 2024](https://img.shields.io/badge/last_commit-april_2024-blue)
![License](https://img.shields.io/badge/License-MIT-green)

## Introduction

This NuGet package facilitates integration of the SignalFlow library with
Microsoft.Extensions.DependencyInjection for dependency injection in .NET applications.

## Table of Contents

- [Installation](#installation)
    - [.NET CLI](#installcli)
    - [NuGet Package Referense](#installnugetpackage)
- [Usage](#usage)
    - [Registering Signals](#registering)
    - [Resolving Event Bus](#resolving)
    - [Additional Notes](#additional)
- [Contributing](#contributing)

# <a id="installation"/> Installation

## <a id="installcli"/> Using .NET CLI

Install this package via .NET CLI:

```c#
dotnet add package SignalFlow.Microsoft.Extensions.DependencyInjection --version 1.0.0
```

## <a id="installnugetpackage"/> Using NuGet Package Manager Console

If you prefer using the NuGet Package Manager Console, you can execute the following command:

```c#
Install-Package SignalFlow.Microsoft.Extensions.DependencyInjection -Version 1.0.0
```

# <a id="usage"/> Usage

## <a id="registering"/> Signal Registration

Register the SignalFlow services with specific interfaces for dependency injection:

### Registering

You can register new event signals dynamically using the `Register<TSignal>` method. This method takes a type
parameter `TSignal`, which represents the event signal to be registered.

```c#
services.AddSignalFlow(configuration =>
{
    configuration.Register<ExampleSignal>(); // Replace ExampleSignal with your signal type
});
```

After registering an event signal, you can subscribe, unsubscribe, and fire events using that signal.

In this example, ExampleSignal is unregistered from the EventBus instance bus. The UnRegister<TSignal> method is useful
for dynamically managing the set of event signals that the EventBus instance is configured to handle.

## <a id="resolving"/> Resolving Event Bus

Once registered, you can resolve the SignalFlow services through their respective interfaces:

```c#
using SignalFlow;

public class MyService
{
    private readonly ISignalManager _signalManager;
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly IFireManager _fireManager;

    public MyService(ISignalManager signalManager, ISubscriptionManager subscriptionManager, IFireManager fireManager)
    {
        _signalManager = signalManager;
        _subscriptionManager = subscriptionManager;
        _fireManager = fireManager;
    }

    public void UseSignalFlow()
    {
        // Use the resolved interfaces to interact with SignalFlow functionality
        _signalManager.Register<ExampleSignal>();
        _subscriptionManager.Subscribe<ExampleSignal>(HandleSignal);
        _fireManager.Fire<ExampleSignal>();
    }

    private void HandleSignal(ExampleSignal signal)
    {
        // Handle the received signal
    }
}
```

### <a id="resolving"/> Available Interfaces

The EventBus instance can be accessed via the following interfaces:

* `IBus`: Represents the main SignalFlow bus.
* `ISignalManager`: Provides methods to register and unregister signals.
* `ISubscriptionManager`: Offers methods to subscribe and unsubscribe from signals.
* `IFireManager`: Enables firing signals synchronously and asynchronously.

Each interface provides specific functionalities to interact with the SignalFlow framework within your application.

### <a id="additional"/> Additional Notes

Make sure to replace ExampleSignal with your custom signal type that implements the IEvent interface from
SignalFlow.
For more detailed usage examples and documentation, refer to the [SignalFlow GitHub repository](https://github.com/jassus213/SignalFlow).

# <a id="contributing"/> Contributing

Contributions to the EventBus repository are welcome! If you find any issues or have suggestions for improvements,
please open an issue or submit a pull request on the GitHub repository.
