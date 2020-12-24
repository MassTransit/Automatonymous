namespace Automatonymous
{
    using System.Threading;
    using System.Threading.Tasks;
    using Activities;
    using Behaviors;
    using Contexts;


    public static class StateMachineExtensions
    {
        /// <summary>
        ///     Transition a state machine instance to a specific state, producing any events related
        ///     to the transaction such as leaving the previous state and entering the target state
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state instance</param>
        /// <param name="state">The target state</param>
        /// <param name="cancellationToken"></param>
        public static Task TransitionToState<TInstance>(this StateMachine<TInstance> machine, TInstance instance, State state,
            CancellationToken cancellationToken = default)
            where TInstance : class
        {
            var accessor = machine.Accessor;
            var toState = machine.GetState(state.Name);

            Activity<TInstance> activity = new TransitionActivity<TInstance>(toState, accessor);
            Behavior<TInstance> behavior = new LastBehavior<TInstance>(activity);

            var eventContext = new StateMachineEventContext<TInstance>(machine, instance, toState.Enter, cancellationToken);

            BehaviorContext<TInstance> behaviorContext = new EventBehaviorContext<TInstance>(eventContext);

            return behavior.Execute(behaviorContext);
        }
    }
}
