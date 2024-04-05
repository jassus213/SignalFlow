using System;

namespace EventHubFramework.Exceptions
{
    public class EventBusException : Exception
    {
        public EventBusException(string message) : base(message)
        {
        
        }
    }
}