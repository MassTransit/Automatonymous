namespace Automatonymous
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class AutomatonymousException :
        Exception
    {
        public AutomatonymousException()
        {
        }

        public AutomatonymousException(string message)
            : base(message)
        {
        }

        public AutomatonymousException(Type machineType, string message)
            : base($"{machineType.Name}: {message}")
        {
        }

        public AutomatonymousException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AutomatonymousException(Type machineType, string message, Exception innerException)
            : base($"{machineType.Name}: {message}", innerException)
        {
        }

        protected AutomatonymousException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
