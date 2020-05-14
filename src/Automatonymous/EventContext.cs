namespace Automatonymous
{
    using System.Threading.Tasks;


    public interface EventContext<out TInstance> :
        InstanceContext<TInstance>
    {
        Event Event { get; }

        /// <summary>
        /// Raise an event on the current instance, pushing the current event on the stack
        /// </summary>
        /// <param name="event">The event to raise</param>
        /// <returns>An awaitable Task</returns>
        Task Raise(Event @event);

        /// <summary>
        /// Raise an event on the current instance, pushing the current event on the stack
        /// </summary>
        /// <param name="event">The event to raise</param>
        /// <param name="data">THe event data</param>
        /// <returns>An awaitable Task</returns>
        Task Raise<TData>(Event<TData> @event, TData data);
    }


    /// <summary>
    /// Encapsulates an event that was raised which includes data
    /// </summary>
    /// <typeparam name="TInstance">The state instance the event is targeting</typeparam>
    /// <typeparam name="TData">The event data type</typeparam>
    public interface EventContext<out TInstance, out TData> :
        EventContext<TInstance>
    {
        new Event<TData> Event { get; }

        /// <summary>
        /// The data from the event
        /// </summary>
        TData Data { get; }
    }
}
