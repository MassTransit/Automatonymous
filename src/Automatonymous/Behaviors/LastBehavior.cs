namespace Automatonymous.Behaviors
{
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// The last behavior either completes the last activity in the behavior or
    /// throws the exception if a compensation is in progress
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public class LastBehavior<TInstance> :
        Behavior<TInstance>
    {
        readonly Activity<TInstance> _activity;

        public LastBehavior(Activity<TInstance> activity)
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

        Task Behavior<TInstance>.Execute(BehaviorContext<TInstance> context)
        {
            return _activity.Execute(context, Behavior.Empty<TInstance>());
        }

        Task Behavior<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context)
        {
            return _activity.Execute(context, Behavior.Empty<TInstance, T>());
        }

        Task Behavior<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context)
        {
            return _activity.Faulted(context, Behavior.Exception<TInstance, T>());
        }

        Task Behavior<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context)
        {
            return _activity.Faulted(context, Behavior.Exception<TInstance>());
        }
    }
}
