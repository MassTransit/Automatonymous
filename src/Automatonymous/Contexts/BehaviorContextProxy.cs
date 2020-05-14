namespace Automatonymous.Contexts
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class BehaviorContextProxy<TInstance> :
        ProxyPipeContext,
        BehaviorContext<TInstance>
    {
        readonly BehaviorContext<TInstance> _context;

        public BehaviorContextProxy(BehaviorContext<TInstance> context, Event @event)
            : base(context)
        {
            _context = context;
            Event = @event;
        }

        public Task Raise(Event @event)
        {
            return _context.Raise(@event);
        }

        public Task Raise<TData>(Event<TData> @event, TData data)
        {
            return _context.Raise(@event, data);
        }

        public Event Event { get; }
        public TInstance Instance => _context.Instance;

        public BehaviorContext<TInstance> GetProxy(Event @event)
        {
            return _context.GetProxy(@event);
        }

        public BehaviorContext<TInstance, T> GetProxy<T>(Event<T> @event, T data)
        {
            return _context.GetProxy(@event, data);
        }
    }


    public class BehaviorContextProxy<TInstance, TData> :
        ProxyPipeContext,
        BehaviorContext<TInstance, TData>
    {
        readonly BehaviorContext<TInstance> _context;
        readonly Event<TData> _event;

        public BehaviorContextProxy(BehaviorContext<TInstance> context, Event<TData> @event, TData data)
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

        public Task Raise<TData1>(Event<TData1> @event, TData1 data)
        {
            return _context.Raise(@event, data);
        }

        public TData Data { get; }
        Event EventContext<TInstance>.Event => _event;
        Event<TData> EventContext<TInstance, TData>.Event => _event;
        public TInstance Instance => _context.Instance;

        public BehaviorContext<TInstance> GetProxy(Event @event)
        {
            return _context.GetProxy(@event);
        }

        public BehaviorContext<TInstance, T> GetProxy<T>(Event<T> @event, T data)
        {
            return _context.GetProxy(@event, data);
        }
    }
}
