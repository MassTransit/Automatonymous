namespace Automatonymous.Behaviors
{
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// In a catch, after the last activity, the fault is completed as handled. An activity should throw the 
    /// exception if desired.
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public class LastFaultedBehavior<TInstance> :
        Behavior<TInstance>
    {
        readonly Activity<TInstance> _activity;

        public LastFaultedBehavior(Activity<TInstance> activity)
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
            return _activity.Faulted(context, Behavior.Empty<TInstance, T>());
        }

        Task Behavior<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context)
        {
            return _activity.Faulted(context, Behavior.Empty<TInstance>());
        }
    }
}
