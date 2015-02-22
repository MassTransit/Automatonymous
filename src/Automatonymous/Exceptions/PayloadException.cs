namespace Automatonymous
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class PayloadException :
        AutomatonymousException
    {
        public PayloadException()
        {
        }

        public PayloadException(string message)
            : base(message)
        {
        }

        public PayloadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PayloadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}