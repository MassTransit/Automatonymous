namespace Automatonymous
{
    using System.Threading.Tasks;


    /// <summary>
    /// Callback for an unhandled event in the state machine
    /// </summary>
    /// <typeparam name="TInstance">The state machine instance type</typeparam>
    /// <param name="context">The event context</param>
    /// <returns></returns>
    public delegate Task UnhandledEventCallback<in TInstance>(UnhandledEventContext<TInstance> context);
}
