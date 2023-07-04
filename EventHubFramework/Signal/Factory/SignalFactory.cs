using System.Linq.Expressions;

namespace EventHubFramework.Signal.Factory;

public static class SignalFactory
{
    internal static Func<TSignal> Create<TSignal>() where TSignal : Signal<TSignal>
    {
        var constructorInfo = typeof(TSignal).GetConstructors().First();
        return Expression.Lambda<Func<TSignal>>(Expression.New(constructorInfo)).Compile();
    }
}