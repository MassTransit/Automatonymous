namespace Automatonymous
{
    using System;


    public interface StateMachineVisitor
    {
        void Visit(State state, Action<State> next);

        void Visit(Event @event, Action<Event> next);

        void Visit<TData>(Event<TData> @event, Action<Event<TData>> next);

        void Visit(Activity activity);

        void Visit<T>(Behavior<T> behavior);

        void Visit<T>(Behavior<T> behavior, Action<Behavior<T>> next);

        void Visit<T, TData>(Behavior<T, TData> behavior);
        void Visit<T, TData>(Behavior<T, TData> behavior, Action<Behavior<T, TData>> next);

        void Visit(Activity activity, Action<Activity> next);
    }
}
