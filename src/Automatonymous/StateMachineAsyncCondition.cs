namespace Automatonymous
{
    using System.Threading.Tasks;


    /// <summary>
    /// Filters activities based on the async conditional statement
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Task<bool> StateMachineAsyncCondition<in TInstance, in TData>(BehaviorContext<TInstance, TData> context);


    /// <summary>
    /// Filters activities based on the async conditional statement
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Task<bool> StateMachineAsyncCondition<in TInstance>(BehaviorContext<TInstance> context);
}
