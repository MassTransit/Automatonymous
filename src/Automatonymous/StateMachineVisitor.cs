namespace Automatonymous
{
    using System;


    public interface StateMachineVisitor
    {
        void Visit(State state, Action<State> next = null);

        void Visit(Event @event, Action<Event> next = null);

        void Visit<TData>(Event<TData> @event, Action<Event<TData>> next = null);

        void Visit<T>(Behavior<T> behavior, Action<Behavior<T>> next = null);

        void Visit<T, TData>(Behavior<T, TData> behavior, Action<Behavior<T, TData>> next = null);

        void Visit(Activity activity, Action<Activity> next = null);
    }
}
