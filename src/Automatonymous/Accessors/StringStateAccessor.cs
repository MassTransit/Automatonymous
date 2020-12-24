namespace Automatonymous.Accessors
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Internals.Reflection;
    using GreenPipes.Util;


    /// <summary>
    /// Accesses the current state as a string property
    /// </summary>
    /// <typeparam name="TInstance">The instance type</typeparam>
    public class StringStateAccessor<TInstance> :
        StateAccessor<TInstance>
        where TInstance : class
    {
        readonly StateMachine<TInstance> _machine;
        readonly StateObserver<TInstance> _observer;
        readonly ReadWriteProperty<TInstance, string> _property;

        public StringStateAccessor(StateMachine<TInstance> machine, Expression<Func<TInstance, string>> currentStateExpression,
            StateObserver<TInstance> observer)
        {
            _machine = machine;
            _observer = observer;

            _property = GetCurrentStateProperty(currentStateExpression);
        }

        Task<State<TInstance>> StateAccessor<TInstance>.Get(InstanceContext<TInstance> context)
        {
            var stateName = _property.Get(context.Instance);
            if (string.IsNullOrWhiteSpace(stateName))
                return Task.FromResult<State<TInstance>>(null);

            return Task.FromResult(_machine.GetState(stateName));
        }

        Task StateAccessor<TInstance>.Set(InstanceContext<TInstance> context, State<TInstance> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            var previous = _property.Get(context.Instance);
            if (state.Name.Equals(previous))
                return TaskUtil.Completed;

            _property.Set(context.Instance, state.Name);

            State<TInstance> previousState = null;
            if (previous != null)
                previousState = _machine.GetState(previous);

            return _observer.StateChanged(context, state, previousState);
        }

        public Expression<Func<TInstance, bool>> GetStateExpression(params State[] states)
        {
            if (states == null || states.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(states), "One or more states must be specified");

            var parameterExpression = Expression.Parameter(typeof(TInstance), "instance");

            var statePropertyExpression = Expression.Property(parameterExpression, _property.Property.GetMethod);

            var stateExpression = states.Select(state => Expression.Equal(statePropertyExpression, Expression.Constant(state.Name)))
                .Aggregate((left, right) => Expression.Or(left, right));

            return Expression.Lambda<Func<TInstance, bool>>(stateExpression, parameterExpression);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("currentStateProperty", _property.Property.Name);
        }

        static ReadWriteProperty<TInstance, string> GetCurrentStateProperty(Expression<Func<TInstance, string>> currentStateExpression)
        {
            var propertyInfo = currentStateExpression.GetPropertyInfo();

            return new ReadWriteProperty<TInstance, string>(propertyInfo);
        }
    }
}
