namespace Automatonymous.Contexts
{
    using System.Threading;
    using GreenPipes;


    public class StateMachineInstanceContext<TInstance> :
        BasePipeContext,
        InstanceContext<TInstance>
        where TInstance : class
    {
        public StateMachineInstanceContext(TInstance instance)
        {
            Instance = instance;
        }

        public StateMachineInstanceContext(TInstance instance, params object[] payloads)
            : base(payloads)
        {
            Instance = instance;
        }

        public StateMachineInstanceContext(TInstance instance, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            Instance = instance;
        }

        public StateMachineInstanceContext(TInstance instance, CancellationToken cancellationToken, params object[] payloads)
            : base(cancellationToken, payloads)
        {
            Instance = instance;
        }

        public TInstance Instance { get; }
    }
}
