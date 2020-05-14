namespace Automatonymous.States
{
    public interface StateEventFilter<in TInstance>
    {
        bool Filter(EventContext<TInstance> context);

        bool Filter<T>(EventContext<TInstance, T> context);
    }
}
