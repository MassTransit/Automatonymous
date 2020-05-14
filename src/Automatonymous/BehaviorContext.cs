namespace Automatonymous
{
    /// <summary>
    /// A behavior context is an event context delivered to a behavior, including the state instance
    /// </summary>
    /// <typeparam name="TInstance">The state instance type</typeparam>
    public interface BehaviorContext<out TInstance> :
        EventContext<TInstance>
    {
        /// <summary>
        /// Return a proxy of the current behavior context with the specified event
        /// </summary>
        /// <param name="event">The event for the new context</param>
        /// <returns></returns>
        BehaviorContext<TInstance> GetProxy(Event @event);

        /// <summary>
        /// Return a proxy of the current behavior context with the specified event and data
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="event">The event for the new context</param>
        /// <param name="data">The data for the event</param>
        /// <returns></returns>
        BehaviorContext<TInstance, T> GetProxy<T>(Event<T> @event, T data);
    }


    /// <summary>
    /// A behavior context include an event context, along with the behavior for a state instance.
    /// </summary>
    /// <typeparam name="TInstance">The instance type</typeparam>
    /// <typeparam name="TData">The event type</typeparam>
    public interface BehaviorContext<out TInstance, out TData> :
        BehaviorContext<TInstance>,
        EventContext<TInstance, TData>
    {
    }
}
