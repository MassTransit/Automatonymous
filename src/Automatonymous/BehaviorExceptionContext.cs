namespace Automatonymous
{
    using System;


    /// <summary>
    /// An exceptional behavior context
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TException"></typeparam>
    public interface BehaviorExceptionContext<out TInstance, out TData, out TException> :
        BehaviorContext<TInstance, TData>,
        BehaviorExceptionContext<TInstance, TException>
        where TException : Exception
    {
        /// <summary>
        /// Return a proxy of the current behavior context with the specified event and data
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="event">The event for the new context</param>
        /// <param name="data">The data for the event</param>
        /// <returns></returns>
        new BehaviorExceptionContext<TInstance, T, TException> GetProxy<T>(Event<T> @event, T data);
    }


    /// <summary>
    /// An exceptional behavior context
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    public interface BehaviorExceptionContext<out TInstance, out TException> :
        BehaviorContext<TInstance>
        where TException : Exception
    {
        TException Exception { get; }

        /// <summary>
        /// Return a proxy of the current behavior context with the specified event and data
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="event">The event for the new context</param>
        /// <param name="data">The data for the event</param>
        /// <returns></returns>
        new BehaviorExceptionContext<TInstance, T, TException> GetProxy<T>(Event<T> @event, T data);
    }
}
