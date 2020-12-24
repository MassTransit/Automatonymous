namespace Automatonymous.Binders
{
    using System;
    using Activities;
    using Behaviors;


    /// <summary>
    /// Creates a compensation activity with the compensation behavior
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    public class CatchActivityBinder<TInstance, TException> :
        ActivityBinder<TInstance>
        where TInstance : class
        where TException : Exception
    {
        readonly EventActivities<TInstance> _activities;
        readonly Event _event;

        public CatchActivityBinder(Event @event, EventActivities<TInstance> activities)
        {
            _event = @event;
            _activities = activities;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(_event, state.Enter) || Equals(_event, state.BeforeEnter)
                   || Equals(_event, state.AfterLeave) || Equals(_event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            var builder = new CatchBehaviorBuilder<TInstance>();
            foreach (var activity in _activities.GetStateActivityBinders())
                activity.Bind(builder);

            var compensateActivity = new CatchFaultActivity<TInstance, TException>(builder.Behavior);

            state.Bind(_event, compensateActivity);
        }

        public void Bind(BehaviorBuilder<TInstance> builder)
        {
            foreach (var activity in _activities.GetStateActivityBinders())
                activity.Bind(builder);
        }
    }
}
