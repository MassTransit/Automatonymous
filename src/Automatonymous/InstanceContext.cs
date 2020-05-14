namespace Automatonymous
{
    using GreenPipes;


    public interface InstanceContext<out TInstance> :
        PipeContext
    {
        /// <summary>
        /// The state instance which is targeted by the event
        /// </summary>
        TInstance Instance { get; }
    }
}
