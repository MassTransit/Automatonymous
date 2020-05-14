namespace Automatonymous
{
    using System;


    public interface Event :
        Visitable,
        IComparable<Event>
    {
        string Name { get; }
    }


    public interface Event<out TData> :
        Event
    {
    }
}
