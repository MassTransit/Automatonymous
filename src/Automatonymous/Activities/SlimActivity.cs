namespace Automatonymous.Activities
{
    using System.Threading.Tasks;
    using Behaviors;
    using GreenPipes;


    /// <summary>
    /// Adapts an Activity to a Data Activity context
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class SlimActivity<TInstance, TData> :
        Activity<TInstance, TData>
    {
        readonly Activity<TInstance> _activity;

        public SlimActivity(Activity<TInstance> activity)
        {
            _activity = activity;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            _activity.Accept(visitor);
        }

        public void Probe(ProbeContext context)
        {
            _activity.Probe(context);
        }

        Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> behavior)
        {
            return _activity.Execute(context, new WidenBehavior<TInstance, TData>(behavior, context));
        }

        Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context,
            Behavior<TInstance, TData> next)
        {
            return _activity.Faulted(context, next);
        }
    }
}
