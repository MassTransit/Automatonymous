namespace Automatonymous.Contexts
{
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;


    public class StateUnhandledEventContext<TInstance> :
        ProxyPipeContext,
        UnhandledEventContext<TInstance>
        where TInstance : class
    {
        readonly EventContext<TInstance> _context;
        readonly StateMachine<TInstance> _machine;
        readonly State _state;

        public StateUnhandledEventContext(EventContext<TInstance> context, State state, StateMachine<TInstance> machine)
            : base(context)
        {
            _context = context;
            _state = state;
            _machine = machine;
        }

        public State CurrentState => _state;
        public TInstance Instance => _context.Instance;
        public Event Event => _context.Event;

        public Task Raise(Event @event)
        {
            return _context.Raise(@event);
        }

        public Task Raise<TData>(Event<TData> @event, TData data)
        {
            return _context.Raise(@event, data);
        }

        public Task Ignore()
        {
            return TaskUtil.Completed;
        }

        public Task Throw()
        {
            throw new UnhandledEventException(_machine.Name, _context.Event.Name, _state.Name);
        }
    }
}
