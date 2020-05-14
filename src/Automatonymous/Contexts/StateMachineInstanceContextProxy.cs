namespace Automatonymous.Contexts
{
    using GreenPipes;


    public class StateMachineInstanceContextProxy<TInstance> :
        ProxyPipeContext,
        InstanceContext<TInstance>
        where TInstance : class
    {
        public StateMachineInstanceContextProxy(PipeContext context, TInstance instance)
            : base(context)
        {
            Instance = instance;
        }

        public TInstance Instance { get; }
    }
}
