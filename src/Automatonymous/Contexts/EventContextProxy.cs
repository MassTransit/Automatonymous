namespace Automatonymous.Contexts
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class EventContextProxy<TInstance> :
        ProxyPipeContext,
        EventContext<TInstance>
    {
        readonly EventContext<TInstance> _context;

        public EventContextProxy(EventContext<TInstance> context, Event @event)
            : base(context)
        {
            _context = context;
            Event = @event;
        }

        public Task Raise(Event @event)
        {
            return _context.Raise(@event);
        }

        public Task Raise<T>(Event<T> @event, T data)
        {
            return _context.Raise(@event, data);
        }

        public Event Event { get; }
        public TInstance Instance => _context.Instance;
    }


    public class EventContextProxy<TInstance, TData> :
        ProxyPipeContext,
        EventContext<TInstance, TData>
    {
        readonly EventContext<TInstance> _context;
        readonly Event<TData> _event;

        public EventContextProxy(EventContext<TInstance> context, Event<TData> @event, TData data)
            : base(context)
        {
            _context = context;
            _event = @event;
            Data = data;
        }

        public Task Raise(Event @event)
        {
            return _context.Raise(@event);
        }

        public Task Raise<T>(Event<T> @event, T data)
        {
            return _context.Raise(@event, data);
        }

        public TData Data { get; }
        Event EventContext<TInstance>.Event => _event;
        Event<TData> EventContext<TInstance, TData>.Event => _event;
        public TInstance Instance => _context.Instance;
    }
}
