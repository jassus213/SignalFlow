using Microsoft.Extensions.DependencyInjection;
using SignalFlow.Abstractions;
using SignalFlow.Abstractions.Firing;

namespace SignalFlow.Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static void AddEventHub(this IServiceCollection serviceCollection,
        EventHubConfigurationDelegate configurationDelegate)
    {
        var eventBus = new EventBus();
        configurationDelegate(eventBus);

        serviceCollection.AddSingleton(eventBus);

        serviceCollection.AddSingleton<IBus>(x => x.GetRequiredService<EventBus>());
        serviceCollection.AddSingleton<ISignalManager>(x => x.GetRequiredService<EventBus>());
        serviceCollection.AddSingleton<ISubscriptionManager>(x => x.GetRequiredService<EventBus>());
        serviceCollection.AddSingleton<IFireManager>(x => x.GetRequiredService<EventBus>());
    }
}