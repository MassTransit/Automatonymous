namespace Automatonymous.Behaviors
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class ActivityBehavior<TInstance> :
        Behavior<TInstance>
    {
        readonly Activity<TInstance> _activity;
        readonly Behavior<TInstance> _next;

        public ActivityBehavior(Activity<TInstance> activity, Behavior<TInstance> next)
        {
            _activity = activity;
            _next = next;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x =>
            {
                _activity.Accept(visitor);
                _next.Accept(visitor);
            });
        }

        public void Probe(ProbeContext context)
        {
            _activity.Probe(context);
            _next.Probe(context);
        }

        async Task Behavior<TInstance>.Execute(BehaviorContext<TInstance> context)
        {
            try
            {
                await _activity.Execute(context, _next).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await ExceptionTypeCache.Faulted(_next, context, exception).ConfigureAwait(false);
            }
        }

        async Task Behavior<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context)
        {
            var behavior = new DataBehavior<TInstance, T>(_next);
            try
            {
                await _activity.Execute(context, behavior).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await ExceptionTypeCache.Faulted(behavior, context, exception).ConfigureAwait(false);
            }
        }

        Task Behavior<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context)
        {
            var behavior = new DataBehavior<TInstance, T>(_next);

            return _activity.Faulted(context, behavior);
        }

        Task Behavior<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context)
        {
            return _activity.Faulted(context, _next);
        }
    }
}
