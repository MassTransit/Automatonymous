namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using Behaviors;
    using GreenPipes;


    public class FactoryActivity<TInstance> :
        Activity<TInstance>
    {
        readonly Func<BehaviorContext<TInstance>, Activity<TInstance>> _activityFactory;

        public FactoryActivity(Func<BehaviorContext<TInstance>, Activity<TInstance>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("factory");
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            var activity = _activityFactory(context);

            return activity.Execute(context, next);
        }

        Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            var activity = _activityFactory(context);

            return activity.Execute(context, new WidenBehavior<TInstance, T>(next, context));
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            var activity = _activityFactory(context);

            return activity.Faulted(context, next);
        }

        Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context,
            Behavior<TInstance, T> next)
        {
            var activity = _activityFactory(context);

            return activity.Faulted(context, new WidenBehavior<TInstance, T>(next, context));
        }
    }


    public class FactoryActivity<TInstance, TData> :
        Activity<TInstance, TData>
    {
        readonly Func<BehaviorContext<TInstance, TData>, Activity<TInstance, TData>> _activityFactory;

        public FactoryActivity(Func<BehaviorContext<TInstance, TData>, Activity<TInstance, TData>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("factory");
        }

        Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            var activity = _activityFactory(context);

            return activity.Execute(context, next);
        }

        Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context,
            Behavior<TInstance, TData> next)
        {
            var activity = _activityFactory(context);

            return activity.Faulted(context, next);
        }
    }
}
