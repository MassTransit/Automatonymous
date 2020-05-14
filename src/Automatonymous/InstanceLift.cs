namespace Automatonymous
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public interface InstanceLift<out T>
        where T : StateMachine
    {
        Task Raise(Event @event, CancellationToken cancellationToken = default);

        Task Raise<TData>(Event<TData> @event, TData data, CancellationToken cancellationToken = default);

        Task Raise(Func<T, Event> eventSelector, CancellationToken cancellationToken = default);

        Task Raise<TData>(Func<T, Event<TData>> eventSelector, TData data, CancellationToken cancellationToken = default);
    }
}
