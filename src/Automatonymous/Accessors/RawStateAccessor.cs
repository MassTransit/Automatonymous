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


    public class RawStateAccessor<TInstance> :
        StateAccessor<TInstance>
        where TInstance : class
    {
        readonly StateMachine<TInstance> _machine;
        readonly StateObserver<TInstance> _observer;
        readonly ReadWriteProperty<TInstance, State> _property;

        public RawStateAccessor(StateMachine<TInstance> machine, Expression<Func<TInstance, State>> currentStateExpression,
            StateObserver<TInstance> observer)
        {
            _machine = machine;
            _observer = observer;

            _property = GetCurrentStateProperty(currentStateExpression);
        }

        Task<State<TInstance>> StateAccessor<TInstance>.Get(InstanceContext<TInstance> context)
        {
            State state = _property.Get(context.Instance);
            if (state == null)
                return Task.FromResult<State<TInstance>>(null);

            return Task.FromResult(_machine.GetState(state.Name));
        }

        Task StateAccessor<TInstance>.Set(InstanceContext<TInstance> context, State<TInstance> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            State previous = _property.Get(context.Instance);
            if (state.Equals(previous))
                return TaskUtil.Completed;

            _property.Set(context.Instance, state);

            State<TInstance> previousState = null;
            if (previous != null)
                previousState = _machine.GetState(previous.Name);

            return _observer.StateChanged(context, state, previousState);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("currentStateProperty", _property.Property.Name);
        }

        static ReadWriteProperty<TInstance, State> GetCurrentStateProperty(Expression<Func<TInstance, State>> currentStateExpression)
        {
            PropertyInfo propertyInfo = currentStateExpression.GetPropertyInfo();

            return new ReadWriteProperty<TInstance, State>(propertyInfo);
        }
    }
}
