namespace Automatonymous.Events
{
    using System;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;


    public class DataEvent<TData> :
        TriggerEvent,
        Event<TData>,
        IEquatable<DataEvent<TData>>
    {
        public DataEvent(string name)
            : base(name)
        {
        }

        public override void Accept(StateMachineVisitor visitor) =>
            visitor.Visit(this, x =>
            {
            });

        public override void Probe(ProbeContext context)
        {
            base.Probe(context);
            context.Add("dataType", TypeCache<TData>.ShortName);
        }

        public bool Equals(DataEvent<TData> other) =>
            !ReferenceEquals(null, other) && (ReferenceEquals(this, other) || Equals(other.Name, Name));

        public override string ToString() =>
            $"{Name}<{typeof(TData).Name}> (Event)";

        public override bool Equals(object obj) =>
            !ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) || Equals(obj as DataEvent<TData>));

        public override int GetHashCode() =>
            base.GetHashCode() * 27 + typeof(TData).GetHashCode();
    }
}
