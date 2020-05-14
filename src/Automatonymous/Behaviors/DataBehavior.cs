namespace Automatonymous.Behaviors
{
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Splits apart the data from the behavior so it can be invoked properly.
    /// </summary>
    /// <typeparam name="TInstance">The instance type</typeparam>
    /// <typeparam name="TData">The event data type</typeparam>
    public class DataBehavior<TInstance, TData> :
        Behavior<TInstance, TData>
    {
        readonly Behavior<TInstance> _behavior;

        public DataBehavior(Behavior<TInstance> behavior)
        {
            _behavior = behavior;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            _behavior.Accept(visitor);
        }

        public void Probe(ProbeContext context)
        {
            _behavior.Probe(context);
        }

        Task Behavior<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context)
        {
            return _behavior.Execute(context);
        }

        Task Behavior<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context)
        {
            return _behavior.Faulted(context);
        }
    }
}
