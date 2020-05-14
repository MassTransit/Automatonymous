namespace Automatonymous.Contexts
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class EventBehaviorContext<TInstance> :
        ProxyPipeContext,
        BehaviorContext<TInstance>
    {
        readonly InstanceContext<TInstance> _context;
        readonly EventContext<TInstance> _eventContext;

        public EventBehaviorContext(EventContext<TInstance> context)
            : base(context)
        {
            _context = context;
            _eventContext = context;

            Event = context.Event;
        }

        public EventBehaviorContext(InstanceContext<TInstance> context)
            : base(context)
        {
            _context = context;
        }

        public Task Raise(Event @event)
        {
            if (_eventContext == null)
                throw new AutomatonymousException($"Events cannot be raised from an instance only: {@event.Name}");

            return _eventContext.Raise(@event);
        }

        public Task Raise<TData>(Event<TData> @event, TData data)
        {
            if (_eventContext == null)
                throw new AutomatonymousException($"Events cannot be raised from an instance only: {@event.Name}");

            return _eventContext.Raise(@event, data);
        }

        public Event Event { get; }
        public TInstance Instance => _context.Instance;

        public BehaviorContext<TInstance> GetProxy(Event @event)
        {
            return new BehaviorContextProxy<TInstance>(this, @event);
        }

        public BehaviorContext<TInstance, T> GetProxy<T>(Event<T> @event, T data)
        {
            return new BehaviorContextProxy<TInstance, T>(this, @event, data);
        }
    }


    public class EventBehaviorContext<TInstance, TData> :
        EventBehaviorContext<TInstance>,
        BehaviorContext<TInstance, TData>
    {
        readonly EventContext<TInstance, TData> _context;

        public EventBehaviorContext(EventContext<TInstance, TData> context)
            : base(context)
        {
            _context = context;
        }

        Event<TData> EventContext<TInstance, TData>.Event => _context.Event;
        TData EventContext<TInstance, TData>.Data => _context.Data;
    }
}
