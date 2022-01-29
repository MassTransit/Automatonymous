namespace Automatonymous
{
    using System;
    using Binders;


    public interface Event :
        Visitable,
        IComparable<Event>
    {
        string Name { get; }
        bool IsComposite { get; set;}
    }

    public interface Event<out TData> :
        Event
    {
    }
}
