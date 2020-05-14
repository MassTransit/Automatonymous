namespace Automatonymous.Lifts
{
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;


    public class StateMachineEventLift<TInstance> :
        EventLift<TInstance>
        where TInstance : class
    {
        readonly Event _event;
        readonly StateMachine<TInstance> _machine;

        public StateMachineEventLift(StateMachine<TInstance> machine, Event @event)
        {
            _machine = machine;
            _event = @event;
        }

        Task EventLift<TInstance>.Raise(TInstance instance, CancellationToken cancellationToken)
        {
            var context = new StateMachineEventContext<TInstance>(_machine, instance, _event, cancellationToken);

            return _machine.RaiseEvent(context);
        }
    }


    public class StateMachineEventLift<TInstance, TData> :
        EventLift<TInstance, TData>
        where TInstance : class
    {
        readonly Event<TData> _event;
        readonly StateMachine<TInstance> _machine;

        public StateMachineEventLift(StateMachine<TInstance> machine, Event<TData> @event)
        {
            _machine = machine;
            _event = @event;
        }

        Task EventLift<TInstance, TData>.Raise(TInstance instance, TData data, CancellationToken cancellationToken)
        {
            var context = new StateMachineEventContext<TInstance, TData>(_machine, instance, _event, data, cancellationToken);

            return _machine.RaiseEvent(context);
        }
    }
}
