namespace Automatonymous
{
    using System.Threading.Tasks;


    public interface StateObserver<in TInstance>
    {
        /// <summary>
        /// Invoked prior to changing the state of the state machine
        /// </summary>
        /// <param name="context">The instance context of the state machine</param>
        /// <param name="currentState">The current state (after the change)</param>
        /// <param name="previousState">The previous state (before the change)</param>
        /// <returns></returns>
        Task StateChanged(InstanceContext<TInstance> context, State currentState, State previousState);
    }
}
