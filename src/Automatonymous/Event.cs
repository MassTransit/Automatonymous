namespace Automatonymous
{
    public interface Event
    {
        string Name { get; }
    }

    public interface Event<T> :
        Event
    {
    }
}