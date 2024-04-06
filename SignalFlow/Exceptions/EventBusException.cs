using System;

namespace SignalFlow.Exceptions
{
    public class EventBusException : Exception
    {
        public EventBusException(string message) : base(message)
        {
        
        }
    }
}