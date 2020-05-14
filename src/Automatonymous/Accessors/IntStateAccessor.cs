namespace Automatonymous.Accessors
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
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
            int stateIndex = _property.Get(context.Instance);

            return Task.FromResult(_index[stateIndex]);
        }

        Task StateAccessor<TInstance>.Set(InstanceContext<TInstance> context, State<TInstance> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            int stateIndex = _index[state.Name];

            int previousIndex = _property.Get(context.Instance);

            if (stateIndex == previousIndex)
                return TaskUtil.Completed;

            _property.Set(context.Instance, stateIndex);

            State<TInstance> previousState = _index[previousIndex];

            return _observer.StateChanged(context, state, previousState);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("currentStateProperty", _property.Property.Name);
        }

        static ReadWriteProperty<TInstance, int> GetCurrentStateProperty(Expression<Func<TInstance, int>> currentStateExpression)
        {
            PropertyInfo propertyInfo = currentStateExpression.GetPropertyInfo();

            return new ReadWriteProperty<TInstance, int>(propertyInfo);
        }
    }
}
