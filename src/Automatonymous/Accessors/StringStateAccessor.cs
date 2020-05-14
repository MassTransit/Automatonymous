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
            string stateName = _property.Get(context.Instance);
            if (string.IsNullOrWhiteSpace(stateName))
                return Task.FromResult<State<TInstance>>(null);

            return Task.FromResult(_machine.GetState(stateName));
        }

        Task StateAccessor<TInstance>.Set(InstanceContext<TInstance> context, State<TInstance> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            string previous = _property.Get(context.Instance);
            if (state.Name.Equals(previous))
                return TaskUtil.Completed;

            _property.Set(context.Instance, state.Name);

            State<TInstance> previousState = null;
            if (previous != null)
                previousState = _machine.GetState(previous);

            return _observer.StateChanged(context, state, previousState);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("currentStateProperty", _property.Property.Name);
        }

        static ReadWriteProperty<TInstance, string> GetCurrentStateProperty(Expression<Func<TInstance, string>> currentStateExpression)
        {
            PropertyInfo propertyInfo = currentStateExpression.GetPropertyInfo();

            return new ReadWriteProperty<TInstance, string>(propertyInfo);
        }
    }
}
