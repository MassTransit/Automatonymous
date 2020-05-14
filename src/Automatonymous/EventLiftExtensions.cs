namespace Automatonymous
{
    using System.Threading;
    using System.Threading.Tasks;
    using Lifts;


    public static class EventLiftExtensions
    {
        public static EventLift<TInstance> CreateEventLift<TInstance>(this StateMachine<TInstance> stateMachine, Event @event)
            where TInstance : class
        {
            return new StateMachineEventLift<TInstance>(stateMachine, @event);
        }

        public static EventLift<TInstance, TData> CreateEventLift<TInstance, TData>(this StateMachine<TInstance> stateMachine,
            Event<TData> @event)
            where TInstance : class
        {
            return new StateMachineEventLift<TInstance, TData>(stateMachine, @event);
        }

        public static Task Raise<TInstance>(this EventLift<TInstance> lift, TInstance instance,
            CancellationToken cancellationToken = default)
            where TInstance : class
        {
            return lift.Raise(instance, cancellationToken);
        }

        public static Task Raise<TInstance, TData>(this EventLift<TInstance, TData> lift, TInstance instance, TData value,
            CancellationToken cancellationToken = default)
            where TInstance : class
        {
            return lift.Raise(instance, value, cancellationToken);
        }
    }
}
