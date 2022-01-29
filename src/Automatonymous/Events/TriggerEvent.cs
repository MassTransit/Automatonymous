namespace Automatonymous.Events
{
    using System;
    using GreenPipes;


    public class TriggerEvent : Event
    {

        public string Name { get; }
        public bool IsComposite { get; set; }

        public TriggerEvent(string name, bool isComposite = false)
        {
            Name = name;
            IsComposite = isComposite;
        }

        public virtual void Accept(StateMachineVisitor visitor) =>
            visitor.Visit(this, x =>
            {
            });

        public virtual void Probe(ProbeContext context) =>
            context.Add("name", Name);

        public int CompareTo(Event other) =>
            string.Compare(Name, other.Name, StringComparison.Ordinal);

        public bool Equals(TriggerEvent other) =>
            !ReferenceEquals(null, other) && (ReferenceEquals(this, other) || Equals(other.Name, Name));

        public override bool Equals(object obj) =>
            !ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) || obj.GetType() == typeof(TriggerEvent) && Equals((TriggerEvent)obj));

        public override int GetHashCode() =>
            Name?.GetHashCode() ?? 0;

        public override string ToString() =>
            $"{Name} (Event)";
    }
}
