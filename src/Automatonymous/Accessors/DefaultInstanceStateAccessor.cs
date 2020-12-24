namespace Automatonymous.Accessors
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// The default state accessor will attempt to find and use a single State property on the
    /// instance type. If no State property is found, or more than one is found, an exception
    /// will be thrown
    /// </summary>
    public class DefaultInstanceStateAccessor<TInstance> :
        StateAccessor<TInstance>
        where TInstance : class
    {
        readonly Lazy<StateAccessor<TInstance>> _accessor;
        readonly State<TInstance> _initialState;
        readonly StateMachine<TInstance> _machine;
        readonly StateObserver<TInstance> _observer;

        public DefaultInstanceStateAccessor(StateMachine<TInstance> machine, State<TInstance> initialState,
            StateObserver<TInstance> observer)
        {
            _machine = machine;
            _initialState = initialState;
            _observer = observer;
            _accessor = new Lazy<StateAccessor<TInstance>>(CreateDefaultAccessor);
        }

        Task<State<TInstance>> StateAccessor<TInstance>.Get(InstanceContext<TInstance> context)
        {
            return _accessor.Value.Get(context);
        }

        Task StateAccessor<TInstance>.Set(InstanceContext<TInstance> context, State<TInstance> state)
        {
            return _accessor.Value.Set(context, state);
        }

        public Expression<Func<TInstance, bool>> GetStateExpression(params State[] states)
        {
            return _accessor.Value.GetStateExpression(states);
        }

        public void Probe(ProbeContext context)
        {
            _accessor.Value.Probe(context);
        }

        StateAccessor<TInstance> CreateDefaultAccessor()
        {
            var states = typeof(TInstance)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.PropertyType == typeof(State))
                .Where(x => x.GetGetMethod(true) != null)
                .Where(x => x.GetSetMethod(true) != null)
                .ToList();

            if (states.Count > 1)
                throw new AutomatonymousException(
                    "The InstanceState was not configured, and could not be automatically identified as multiple State properties exist.");

            if (states.Count == 0)
                throw new AutomatonymousException(
                    "The InstanceState was not configured, and no public State property exists.");

            var instance = Expression.Parameter(typeof(TInstance), "instance");
            var memberExpression = Expression.Property(instance, states[0]);

            var expression = Expression.Lambda<Func<TInstance, State>>(memberExpression,
                instance);

            return new InitialIfNullStateAccessor<TInstance>(_initialState, new RawStateAccessor<TInstance>(_machine, expression, _observer));
        }
    }
}
