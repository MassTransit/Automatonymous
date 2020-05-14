namespace Automatonymous.States
{
    public class AllStateEventFilter<TInstance> :
        StateEventFilter<TInstance>
    {
        public bool Filter(EventContext<TInstance> context)
        {
            return true;
        }

        public bool Filter<T>(EventContext<TInstance, T> context)
        {
            return true;
        }
    }
}
