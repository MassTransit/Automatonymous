namespace Stayt.Impl
{
    public class EventImpl<TInstance> :
        Event
        where TInstance : StateMachineInstance
    {
        public EventImpl(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    public class EventImpl<TInstance, T> :
        EventImpl<TInstance>,
        Event<T>
        where TInstance : StateMachineInstance
    {
        public EventImpl(string name)
            : base(name)
        {
        }
    }
}