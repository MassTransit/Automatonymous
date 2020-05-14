namespace Automatonymous.Lifts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class StateMachineInstanceLift<TStateMachine, TInstance> :
        InstanceLift<TStateMachine>
        where TStateMachine : class, StateMachine, StateMachine<TInstance>
        where TInstance : class
    {
        readonly TInstance _instance;
        readonly TStateMachine _stateMachine;

        public StateMachineInstanceLift(TStateMachine stateMachine, TInstance instance)
        {
            _stateMachine = stateMachine;
            _instance = instance;
        }

        Task InstanceLift<TStateMachine>.Raise(Event @event, CancellationToken cancellationToken)
        {
            return _stateMachine.RaiseEvent(_instance, @event, cancellationToken);
        }

        Task InstanceLift<TStateMachine>.Raise<T>(Event<T> @event, T data, CancellationToken cancellationToken)
        {
            return _stateMachine.RaiseEvent(_instance, @event, data, cancellationToken);
        }

        Task InstanceLift<TStateMachine>.Raise(Func<TStateMachine, Event> eventSelector, CancellationToken cancellationToken)
        {
            return _stateMachine.RaiseEvent(_instance, eventSelector, cancellationToken);
        }

        Task InstanceLift<TStateMachine>.Raise<T>(Func<TStateMachine, Event<T>> eventSelector, T data, CancellationToken cancellationToken)
        {
            return _stateMachine.RaiseEvent(_instance, eventSelector, data, cancellationToken);
        }
    }
}
