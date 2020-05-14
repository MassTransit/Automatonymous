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
            Activity<TInstance> activity = _activityFactory(context);

            return activity.Execute(context, next);
        }

        Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            Activity<TInstance> activity = _activityFactory(context);

            var upconvert = new WidenBehavior<TInstance, T>(next, context);

            return activity.Execute(context, upconvert);
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
            Activity<TInstance, TData> activity = _activityFactory(context);

            return activity.Execute(context, next);
        }

        Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context,
            Behavior<TInstance, TData> next)
        {
            return next.Faulted(context);
        }
    }
}
