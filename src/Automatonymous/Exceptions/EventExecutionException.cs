namespace Automatonymous
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class EventExecutionException :
        AutomatonymousException
    {
        public EventExecutionException(string message)
            : base(message)
        {
        }

        public EventExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected EventExecutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public EventExecutionException()
        {
        }
    }
}
