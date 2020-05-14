namespace Automatonymous
{
    using System;


    /// <summary>
    /// Filters activities based on the conditional statement
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate bool StateMachineExceptionCondition<in TInstance, in TData, in TException>(
        BehaviorExceptionContext<TInstance, TData, TException> context)
        where TException : Exception;


    /// <summary>
    /// Filters activities based on the conditional statement
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate bool StateMachineExceptionCondition<in TInstance, in TException>(BehaviorExceptionContext<TInstance, TException> context
        )
        where TException : Exception;
}
