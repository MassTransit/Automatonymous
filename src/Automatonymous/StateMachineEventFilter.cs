namespace Automatonymous
{
    /// <summary>
    /// Delegate for an event filter, which can examine an event and return true if the filter matches the event instance
    /// </summary>
    /// <typeparam name="TInstance">The state machine instance type</typeparam>
    /// <param name="context">The event context</param>
    /// <returns>True if the filter matches the data, otherwise false.</returns>
    public delegate bool StateMachineEventFilter<in TInstance>(EventContext<TInstance> context);


    /// <summary>
    /// Delegate for an event filter, which can examine an event and return true if the filter matches the event data
    /// </summary>
    /// <typeparam name="TInstance">The state machine instance type</typeparam>
    /// <typeparam name="TData">The event data type</typeparam>
    /// <param name="context">The event context</param>
    /// <returns>True if the filter matches the data, otherwise false.</returns>
    public delegate bool StateMachineEventFilter<in TInstance, in TData>(EventContext<TInstance, TData> context);
}
