using SignalFlow;
using SignalFlow.Abstractions;
using SignalFlow.Exceptions;
using SignalFlow.Signal;

namespace EventHubFramework.Unit;

public class Examples
{
    [Test]
    public void Register_Sync_Signal_Then_Subscribe_Fire_Unsubscribe_Success()
    {
        IBus bus = new EventBus();
        bus.Register<ExampleSignal>();

        bus.Subscribe<ExampleSignal>(HandleEventWithArguments);

        bus.TryFire(() => new ExampleSignal() { Id = 1 });

        bus.UnSubscribe<ExampleSignal>(HandleEventWithArguments);

        bus.Fire(() => new ExampleSignal() { Id = 1 });

        bus.UnRegister<ExampleSignal>();
    }

    [Test]
    public async Task Register_Async_Signal_Then_Subscribe_Fire_Unsubscribe_Success()
    {
        IBus bus = new EventBus();
        bus.Register<ExampleSignal>();

        bus.Subscribe<ExampleSignal>(HandleEventWithArgumentsAsync);

        await bus.FireAsync(() => new ExampleSignal() { Id = 1 });

        bus.UnSubscribe<ExampleSignal>(HandleEventWithArgumentsAsync);

        await bus.FireAsync(() => new ExampleSignal() { Id = 1 });
    }

    [Test]
    public void Subscribe_To_Unknown_Async_Signal()
    {
        var bus = new EventBus();
        var exceptionMessage = $"Cant Find Current Signal {typeof(ExampleSignal)}";

        var exception =
            Assert.Catch<EventBusException>(() => bus.Subscribe<ExampleSignal>(HandleEventWithArgumentsAsync));
        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        });
    }

    [Test]
    public void Subscribe_To_Unknown_Sync_Signal()
    {
        var bus = new EventBus();
        var exceptionMessage = $"Cant Find Current Signal {typeof(ExampleSignal)}";

        var exception = Assert.Catch<EventBusException>(() => bus.Subscribe<ExampleSignal>(HandleEventWithArguments));
        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        });
    }

    [Test]
    public async Task Fire_Unknown_Async_Signal()
    {
        var bus = new EventBus();
        var exceptionMessage = $"Cant Find Current Signal {typeof(ExampleSignal)}";

        var exception = Assert.CatchAsync<EventBusException>(async () => await bus.FireAsync<ExampleSignal>());
        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        });
    }

    [Test]
    public void Fire_Unknown_Sync_Signal()
    {
        var bus = new EventBus();
        var exceptionMessage = $"Cant Find Current Signal {typeof(ExampleSignal)}";

        var exception = Assert.Catch<EventBusException>(() => bus.Fire<ExampleSignal>());
        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        });
    }

    [Test]
    public void UnSubscribe_From_Unknown_Signal_Sync()
    {
        var bus = new EventBus();
        var exceptionMessage = $"Cant Find Current Signal {typeof(ExampleSignal)}";

        var exception = Assert.Catch<EventBusException>(() => bus.UnSubscribe<ExampleSignal>(HandleEventWithArguments));
        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        });
    }

    [Test]
    public void UnSubscribe_From_Unknown_Signal_Async()
    {
        var bus = new EventBus();
        var exceptionMessage = $"Cant Find Current Signal {typeof(ExampleSignal)}";

        var exception =
            Assert.Catch<EventBusException>(() => bus.UnSubscribe<ExampleSignal>(HandleEventWithArgumentsAsync));
        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        });
    }

    [Test]
    public void Fire_With_Arguments_Success()
    {
        var bus = new EventBus();
        bus.Register<ExampleSignal>();

        bus.Subscribe<ExampleSignal>(HandleEventWithArguments);

        bus.Fire(() => new ExampleSignal() { Id = 1 });

        bus.UnSubscribe<ExampleSignal>(HandleEventWithArguments);
    }

    [Test]
    public void Fire_Success()
    {
        var bus = new EventBus();
        bus.Register<ExampleSignal>();

        bus.Subscribe<ExampleSignal>(HandleEvent);

        bus.Fire<ExampleSignal>();

        bus.UnSubscribe<ExampleSignal>(HandleEvent);
    }

    [Test]
    public void Fire_Unknown_Signal()
    {
        var bus = new EventBus();
        var exceptionMessage = $"Cant Find Current Signal {typeof(ExampleSignal)}";

        var exception = Assert.Catch<EventBusException>(() => bus.Fire(() => new ExampleSignal() { Id = 1 }));
        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        });
    }

    [Test]
    public void TryFire_Signal_Success()
    {
        var bus = new EventBus();

        bus.Register<ExampleSignal>();
        bus.Subscribe<ExampleSignal>(HandleEvent);
        bus.TryFire<ExampleSignal>();
    }

    [Test]
    public async Task Fire_Async_Success()
    {
        var bus = new EventBus();

        bus.Register<ExampleSignal>();
        bus.Subscribe<ExampleSignal>(HandleEventAsync);
        await bus.FireAsync<ExampleSignal>();
    }

    [Test]
    public async Task Fire_Async_With_Arguments_For_Unknown_Signal()
    {
        var bus = new EventBus();
        var exceptionMessage = $"Cant Find Current Signal {typeof(ExampleSignal)}";

        var exception = Assert.CatchAsync<EventBusException>(async () =>
            await bus.FireAsync(() => new ExampleSignal() { Id = 1 }));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        });
    }

    [Test]
    public async Task TryFire_Async_With_Arguments_For_Unknown_Signal()
    {
        var bus = new EventBus();
        bus.TryFireAsync(() => new ExampleSignal() { Id = 1 });
    }
    
    [Test]
    public void TryFire_Async_With_Arguments_Success()
    {
        var bus = new EventBus();
        bus.Register<ExampleSignal>();
        bus.TryFireAsync(() => new ExampleSignal() { Id = 1 });
    }
    
    [Test]
    public void TryFire_Async_For_Unknown_Signal()
    {
        var bus = new EventBus();
        bus.TryFireAsync<ExampleSignal>();
    }
    
    [Test]
    public void TryFire_Async_Success()
    {
        var bus = new EventBus();
        bus.Register<ExampleSignal>();
        bus.TryFireAsync<ExampleSignal>();
    }

    private void HandleEventWithArguments(ExampleSignal signal)
    {
        Console.WriteLine(signal.Id);
        Assert.That(signal.Id, Is.EqualTo(1));
    }

    private void HandleEvent(ExampleSignal signal)
    {
        Assert.That(true, Is.True);
    }

    private async Task HandleEventWithArgumentsAsync(ExampleSignal signal)
    {
        await Task.Delay(500);
        Assert.That(signal.Id, Is.EqualTo(1));
    }

    private async Task HandleEventAsync(ExampleSignal signal)
    {
        await Task.Delay(500);
    }


    public class ExampleSignal : Signal<ExampleSignal>
    {
        public int Id { get; set; }
    }
}