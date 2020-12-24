namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using Behaviors;
    using GreenPipes;


    public class AsyncFactoryActivity<TInstance, TData> :
        Activity<TInstance, TData>
    {
        readonly Func<BehaviorContext<TInstance, TData>, Task<Activity<TInstance, TData>>> _activityFactory;

        public AsyncFactoryActivity(Func<BehaviorContext<TInstance, TData>, Task<Activity<TInstance, TData>>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("activityFactory");
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            var activity = await _activityFactory(context).ConfigureAwait(false);

            await activity.Execute(context, next).ConfigureAwait(false);
        }

        async Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context,
            Behavior<TInstance, TData> next)
        {
            var activity = await _activityFactory(context).ConfigureAwait(false);

            await activity.Faulted(context, next).ConfigureAwait(false);
        }
    }


    public class AsyncFactoryActivity<TInstance> :
        Activity<TInstance>
    {
        readonly Func<BehaviorContext<TInstance>, Task<Activity<TInstance>>> _activityFactory;

        public AsyncFactoryActivity(Func<BehaviorContext<TInstance>, Task<Activity<TInstance>>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("activityFactory");
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            var activity = await _activityFactory(context).ConfigureAwait(false);

            await activity.Execute(context, next).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            var activity = await _activityFactory(context).ConfigureAwait(false);

            await activity.Execute(context, new WidenBehavior<TInstance, T>(next, context)).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            var activity = await _activityFactory(context).ConfigureAwait(false);

            await activity.Faulted(context, next).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context,
            Behavior<TInstance, T> next)
        {
            var activity = await _activityFactory(context).ConfigureAwait(false);

            await activity.Faulted(context, next).ConfigureAwait(false);
        }
    }
}
