namespace Automatonymous.Binders
{
    using Behaviors;


    /// <summary>
    /// Routes event activities to an activities
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public class ExecuteActivityBinder<TInstance> :
        ActivityBinder<TInstance>
    {
        readonly Activity<TInstance> _activity;
        readonly Event _event;

        public ExecuteActivityBinder(Event @event, Activity<TInstance> activity)
        {
            _event = @event;
            _activity = activity;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(_event, state.Enter) || Equals(_event, state.BeforeEnter)
                   || Equals(_event, state.AfterLeave) || Equals(_event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            state.Bind(_event, _activity);
        }

        public void Bind(BehaviorBuilder<TInstance> builder)
        {
            builder.Add(_activity);
        }
    }
}
