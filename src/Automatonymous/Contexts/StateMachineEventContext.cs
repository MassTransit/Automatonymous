namespace Automatonymous.Contexts
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public class StateMachineEventContext<TInstance> :
        BasePipeContext,
        EventContext<TInstance>
        where TInstance : class
    {
        readonly Event _event;
        readonly TInstance _instance;
        readonly StateMachine<TInstance> _machine;

        public StateMachineEventContext(StateMachine<TInstance> machine, TInstance instance, Event @event,
            CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _machine = machine;
            _instance = instance;
            _event = @event;
        }

        public StateMachineEventContext(StateMachine<TInstance> machine, TInstance instance, Event @event,
            CancellationToken cancellationToken, params object[] payloads)
            : base(cancellationToken, payloads)
        {
            _machine = machine;
            _instance = instance;
            _event = @event;
        }

        public Task Raise(Event @event)
        {
            var eventContext = new EventContextProxy<TInstance>(this, @event);

            return _machine.RaiseEvent(eventContext);
        }

        public Task Raise<TData>(Event<TData> @event, TData data)
        {
            var eventContext = new EventContextProxy<TInstance, TData>(this, @event, data);

            return _machine.RaiseEvent(eventContext);
        }

        Event EventContext<TInstance>.Event => _event;
        TInstance InstanceContext<TInstance>.Instance => _instance;
    }


    public class StateMachineEventContext<TInstance, TData> :
        StateMachineEventContext<TInstance>,
        EventContext<TInstance, TData>
        where TInstance : class
    {
        readonly TData _data;
        readonly Event<TData> _event;

        public StateMachineEventContext(StateMachine<TInstance> machine, TInstance instance, Event<TData> @event, TData data,
            CancellationToken cancellationToken)
            : base(machine, instance, @event, cancellationToken)
        {
            _data = data;
            _event = @event;
        }

        public StateMachineEventContext(StateMachine<TInstance> machine, TInstance instance, Event<TData> @event, TData data,
            CancellationToken cancellationToken, params object[] payloads)
            : base(machine, instance, @event, cancellationToken, payloads)
        {
            _data = data;
            _event = @event;
        }

        TData EventContext<TInstance, TData>.Data => _data;
        Event<TData> EventContext<TInstance, TData>.Event => _event;
    }
}
