namespace Automatonymous.Builder
{
    using System;
    using System.Linq.Expressions;
    using Binders;


    public interface StateMachineModifier<TInstance>
        where TInstance : class
    {
        State Initial { get; }
        State Final { get; }

        StateMachineModifier<TInstance> InstanceState(Expression<Func<TInstance, State>> instanceStateProperty);
        StateMachineModifier<TInstance> InstanceState(Expression<Func<TInstance, string>> instanceStateProperty);
        StateMachineModifier<TInstance> InstanceState(Expression<Func<TInstance, int>> instanceStateProperty, params State[] states);
        StateMachineModifier<TInstance> Name(string machineName);
        StateMachineModifier<TInstance> Event(string name, out Event @event);
        StateMachineModifier<TInstance> Event<T>(string name, out Event<T> @event);

        StateMachineModifier<TInstance> Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, Event<T>>> eventPropertyExpression)
            where TProperty : class;

        StateMachineModifier<TInstance> CompositeEvent(Event @event,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            params Event[] events);

        StateMachineModifier<TInstance> CompositeEvent(Event @event,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events);

        StateMachineModifier<TInstance> CompositeEvent(Event @event,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            params Event[] events);

        StateMachineModifier<TInstance> CompositeEvent(Event @event,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events);

        StateMachineModifier<TInstance> State(string name, out State<TInstance> state);
        StateMachineModifier<TInstance> State(string name, out State state);

        StateMachineModifier<TInstance> State<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression)
            where TProperty : class;

        StateMachineModifier<TInstance> SubState(string name, State superState, out State<TInstance> subState);

        StateMachineModifier<TInstance> SubState<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression, State superState)
            where TProperty : class;

        StateMachineEventActivitiesBuilder<TInstance> During(params State[] states);
        StateMachineEventActivitiesBuilder<TInstance> Initially();
        StateMachineEventActivitiesBuilder<TInstance> DuringAny();
        StateMachineModifier<TInstance> Finally(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback);

        StateMachineModifier<TInstance> WhenEnter(State state,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback);

        StateMachineModifier<TInstance> WhenEnterAny(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback);
        StateMachineModifier<TInstance> WhenLeaveAny(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback);

        StateMachineModifier<TInstance> BeforeEnterAny(
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback);

        StateMachineModifier<TInstance> AfterLeaveAny(
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback);

        StateMachineModifier<TInstance> WhenLeave(State state,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback);

        StateMachineModifier<TInstance> BeforeEnter(State state,
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback);

        StateMachineModifier<TInstance> AfterLeave(State state,
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback);

        StateMachineModifier<TInstance> OnUnhandledEvent(UnhandledEventCallback<TInstance> callback);

        void Apply();
    }
}
