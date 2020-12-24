namespace Automatonymous.Builder
{
    using System;
    using Binders;


    public interface StateMachineEventActivitiesBuilder<TInstance> :
        StateMachineModifier<TInstance>
        where TInstance : class
    {
        bool IsCommitted { get; }

        StateMachineEventActivitiesBuilder<TInstance> When(Event @event,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> configure);

        StateMachineEventActivitiesBuilder<TInstance> When(Event @event, StateMachineEventFilter<TInstance> filter,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> configure);

        StateMachineEventActivitiesBuilder<TInstance> When<TData>(Event<TData> @event,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> configure);

        StateMachineEventActivitiesBuilder<TInstance> When<TData>(Event<TData> @event,
            StateMachineEventFilter<TInstance, TData> filter,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> configure);

        StateMachineEventActivitiesBuilder<TInstance> Ignore(Event @event);
        StateMachineEventActivitiesBuilder<TInstance> Ignore<TData>(Event<TData> @event);

        StateMachineEventActivitiesBuilder<TInstance> Ignore<TData>(Event<TData> @event,
            StateMachineEventFilter<TInstance, TData> filter);

        StateMachineModifier<TInstance> CommitActivities();
    }
}
