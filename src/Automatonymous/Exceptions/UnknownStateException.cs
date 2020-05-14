namespace Automatonymous
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class UnknownStateException :
        AutomatonymousException
    {
        public UnknownStateException()
        {
        }

        public UnknownStateException(string machineType, string stateName)
            : base($"The {stateName} state is not defined for the {machineType} state machine")
        {
        }

        protected UnknownStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
