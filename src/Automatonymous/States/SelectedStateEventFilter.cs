namespace Automatonymous.States
{
    public class SelectedStateEventFilter<TInstance, TData> :
        StateEventFilter<TInstance>
    {
        readonly StateMachineEventFilter<TInstance, TData> _filter;

        public SelectedStateEventFilter(StateMachineEventFilter<TInstance, TData> filter)
        {
            _filter = filter;
        }

        public bool Filter<T>(EventContext<TInstance, T> context)
        {
            if (context is EventContext<TInstance, TData> filterContext)
                return _filter(filterContext);

            return false;
        }

        public bool Filter(EventContext<TInstance> context)
        {
            return false;
        }
    }
}
