namespace EventBus.Common;

public class EventBusException : Exception
{
    public EventBusException(string message) : base(message)
    {
        
    }
}