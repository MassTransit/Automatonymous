namespace Automatonymous.Accessors
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Activities;
    using Behaviors;
    using Contexts;
    using GreenPipes;


    public class InitialIfNullStateAccessor<TInstance> :
        StateAccessor<TInstance>
        where TInstance : class
    {
        readonly Behavior<TInstance> _initialBehavior;
        readonly StateAccessor<TInstance> _stateAccessor;

        public InitialIfNullStateAccessor(State<TInstance> initialState, StateAccessor<TInstance> stateAccessor)
        {
            _stateAccessor = stateAccessor;
            _initialBehavior = new LastBehavior<TInstance>(new TransitionActivity<TInstance>(initialState, _stateAccessor));
        }

        async Task<State<TInstance>> StateAccessor<TInstance>.Get(InstanceContext<TInstance> context)
        {
            var state = await _stateAccessor.Get(context).ConfigureAwait(false);
            if (state == null)
            {
                await _initialBehavior.Execute(new EventBehaviorContext<TInstance>(context)).ConfigureAwait(false);
                state = await _stateAccessor.Get(context).ConfigureAwait(false);
            }
            return state;
        }

        Task StateAccessor<TInstance>.Set(InstanceContext<TInstance> context, State<TInstance> state) =>
            _stateAccessor.Set(context, state);

        public Expression<Func<TInstance, bool>> GetStateExpression(params State[] states) =>
            _stateAccessor.GetStateExpression(states);

        public void Probe(ProbeContext context) =>
            _stateAccessor.Probe(context);
    }
}
