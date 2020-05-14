namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class ExecuteOnFaultedActivity<TInstance> :
        Activity<TInstance>
    {
        readonly Activity<TInstance> _activity;

        public ExecuteOnFaultedActivity(Activity<TInstance> activity)
        {
            _activity = activity;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            _activity.Accept(visitor);
        }

        public void Probe(ProbeContext context)
        {
            _activity.Probe(context);
        }

        public Task Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return next.Execute(context);
        }

        public Task Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            return next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
            where TException : Exception
        {
            var nextBehavior = new ExceptionBehavior<TInstance, TException>(next, context);

            return _activity.Execute(context, nextBehavior);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next)
            where TException : Exception
        {
            var nextBehavior = new ExceptionBehavior<TInstance, T, TException>(next, context);

            return _activity.Execute(context, nextBehavior);
        }
    }
}
