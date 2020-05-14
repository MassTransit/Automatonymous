namespace Automatonymous
{
    /// <summary>
    /// Filters activities based on the conditional statement
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate bool StateMachineCondition<in TInstance>(BehaviorContext<TInstance> context);


    /// <summary>
    /// Filters activities based on the conditional statement
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate bool StateMachineCondition<in TInstance, in TData>(BehaviorContext<TInstance, TData> context);
}
