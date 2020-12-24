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
    public class IntStateAccessor<TInstance> :
        StateAccessor<TInstance>
        where TInstance : class
    {
        readonly StateAccessorIndex<TInstance> _index;
        readonly StateObserver<TInstance> _observer;
        readonly ReadWriteProperty<TInstance, int> _property;

        public IntStateAccessor(Expression<Func<TInstance, int>> currentStateExpression, StateAccessorIndex<TInstance> index,
            StateObserver<TInstance> observer)
        {
            _observer = observer;
            _index = index;

            _property = GetCurrentStateProperty(currentStateExpression);
        }

        Task<State<TInstance>> StateAccessor<TInstance>.Get(InstanceContext<TInstance> context)
        {
            var stateIndex = _property.Get(context.Instance);

            return Task.FromResult(_index[stateIndex]);
        }

        Task StateAccessor<TInstance>.Set(InstanceContext<TInstance> context, State<TInstance> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            var stateIndex = _index[state.Name];

            var previousIndex = _property.Get(context.Instance);

            if (stateIndex == previousIndex)
                return TaskUtil.Completed;

            _property.Set(context.Instance, stateIndex);

            var previousState = _index[previousIndex];

            return _observer.StateChanged(context, state, previousState);
        }

        public Expression<Func<TInstance, bool>> GetStateExpression(params State[] states)
        {
            if (states == null || states.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(states), "One or more states must be specified");

            var parameterExpression = Expression.Parameter(typeof(TInstance), "instance");

            var statePropertyExpression = Expression.Property(parameterExpression, _property.Property.GetMethod);

            var stateExpression = states.Select(state => Expression.Equal(statePropertyExpression, Expression.Constant(_index[state.Name])))
                .Aggregate((left, right) => Expression.Or(left, right));

            return Expression.Lambda<Func<TInstance, bool>>(stateExpression, parameterExpression);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("currentStateProperty", _property.Property.Name);
        }

        static ReadWriteProperty<TInstance, int> GetCurrentStateProperty(Expression<Func<TInstance, int>> currentStateExpression)
        {
            var propertyInfo = currentStateExpression.GetPropertyInfo();

            return new ReadWriteProperty<TInstance, int>(propertyInfo);
        }
    }
}
