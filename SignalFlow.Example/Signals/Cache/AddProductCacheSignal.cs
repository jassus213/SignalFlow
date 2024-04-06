using SignalFlow.Signal;

namespace SignalFlow.Example.Signals.Cache;

public class AddProductCacheSignal : Signal<AddProductCacheSignal>
{
    public int Id { get; set; }
    public string Tittle { get; set; }
    public int Cost { get; set; }
}