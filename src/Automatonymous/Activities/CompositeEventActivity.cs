namespace Automatonymous.Activities
{
    using System.Threading.Tasks;
    using Accessors;
    using GreenPipes;
    using GreenPipes.Util;


    public class CompositeEventActivity<TInstance> :
        Activity<TInstance>
        where TInstance : class
    {
        readonly CompositeEventStatusAccessor<TInstance> _accessor;
        readonly CompositeEventStatus _complete;
        readonly Event _event;
        readonly int _flag;

        public CompositeEventActivity(CompositeEventStatusAccessor<TInstance> accessor, int flag, CompositeEventStatus complete,
            Event @event)
        {
            _accessor = accessor;
            _flag = flag;
            _complete = complete;
            _event = @event;
        }

        public Event Event => _event;

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("compositeEvent");
            _accessor.Probe(scope);
            scope.Add("event", _event.Name);
            scope.Add("flag", _flag.ToString("X8"));
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Execute<TData>(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            return next.Faulted(context);
        }

        Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context,
            Behavior<TInstance, T> next)
        {
            return next.Faulted(context);
        }

        Task Execute(BehaviorContext<TInstance> context)
        {
            var value = _accessor.Get(context.Instance);
            value.Set(_flag);

            _accessor.Set(context.Instance, value);

            if (!value.Equals(_complete))
                return TaskUtil.Completed;

            return RaiseCompositeEvent(context);
        }

        Task RaiseCompositeEvent(BehaviorContext<TInstance> context)
        {
            return context.Raise(_event);
        }
    }
}
