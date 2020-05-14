namespace Automatonymous.Contexts
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class StateMachineEventContextProxy<TInstance, TData> :
        ProxyPipeContext,
        EventContext<TInstance, TData>
        where TInstance : class
    {
        readonly TData _data;
        readonly Event<TData> _event;
        readonly TInstance _instance;
        readonly StateMachine<TInstance> _machine;

        public StateMachineEventContextProxy(PipeContext context, StateMachine<TInstance> machine, TInstance instance, Event<TData> @event,
            TData data)
            : base(context)
        {
            _machine = machine;
            _instance = instance;
            _event = @event;
            _data = data;
        }

        public Task Raise(Event @event)
        {
            var eventContext = new EventContextProxy<TInstance>(this, @event);

            return _machine.RaiseEvent(eventContext);
        }

        public Task Raise<T>(Event<T> @event, T data)
        {
            var eventContext = new EventContextProxy<TInstance, T>(this, @event, data);

            return _machine.RaiseEvent(eventContext);
        }

        Event EventContext<TInstance>.Event => _event;
        TInstance InstanceContext<TInstance>.Instance => _instance;

        TData EventContext<TInstance, TData>.Data => _data;
        Event<TData> EventContext<TInstance, TData>.Event => _event;
    }
}
