using Automatonymous.Binders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Automatonymous.Builder
{
    internal class InternalStateMachineEventActivitiesBuilder<TInstance> : StateMachineEventActivitiesBuilder<TInstance>
        where TInstance : class
    {
        readonly AutomatonymousStateMachine<TInstance> _machine;
        readonly StateMachineModifier<TInstance> _modifier;
        readonly Action<EventActivities<TInstance>[]> _committer;
        readonly List<EventActivities<TInstance>> _activities;

        public InternalStateMachineEventActivitiesBuilder(AutomatonymousStateMachine<TInstance> machine, StateMachineModifier<TInstance> modifier, Action<EventActivities<TInstance>[]> committer)
        {
            _machine = machine ?? throw new ArgumentNullException(nameof(machine));
            _modifier = modifier ?? throw new ArgumentNullException(nameof(modifier));
            _committer = committer ?? throw new ArgumentNullException(nameof(committer));
            _activities = new List<EventActivities<TInstance>>();
            IsCommitted = false;
        }

        public bool IsCommitted { get; private set; }

        public State Initial => _modifier.Initial;
        public State Final => _modifier.Final;

        public StateMachineModifier<TInstance> CommitActivities()
        {
            _committer(_activities.ToArray());
            IsCommitted = true;
            return _modifier;
        }

        #region Pass-through Modifier
        public StateMachineModifier<TInstance> AfterLeave(State state, Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
            => CommitActivities().AfterLeave(state, activityCallback);

        public StateMachineModifier<TInstance> AfterLeaveAny(Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
            => CommitActivities().AfterLeaveAny(activityCallback);

        public StateMachineModifier<TInstance> BeforeEnter(State state, Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
            => CommitActivities().BeforeEnter(state, activityCallback);

        public StateMachineModifier<TInstance> BeforeEnterAny(Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
            => CommitActivities().BeforeEnterAny(activityCallback);

        public StateMachineModifier<TInstance> CompositeEvent(Event @event, Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression, params Event[] events)
            => CommitActivities().CompositeEvent(@event, trackingPropertyExpression, events);

        public StateMachineModifier<TInstance> CompositeEvent(Event @event, Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression, CompositeEventOptions options, params Event[] events)
            => CommitActivities().CompositeEvent(@event, trackingPropertyExpression, options, events);

        public StateMachineModifier<TInstance> CompositeEvent(Event @event, Expression<Func<TInstance, int>> trackingPropertyExpression, params Event[] events)
            => CommitActivities().CompositeEvent(@event, trackingPropertyExpression, events);

        public StateMachineModifier<TInstance> CompositeEvent(Event @event, Expression<Func<TInstance, int>> trackingPropertyExpression, CompositeEventOptions options, params Event[] events)
            => CommitActivities().CompositeEvent(@event, trackingPropertyExpression, options, events);

        public StateMachineEventActivitiesBuilder<TInstance> During(params State[] states)
            => CommitActivities().During(states);

        public StateMachineEventActivitiesBuilder<TInstance> DuringAny()
            => CommitActivities().DuringAny();

        public StateMachineModifier<TInstance> Event(string name, out Event @event)
            => CommitActivities().Event(name, out @event);

        public StateMachineModifier<TInstance> Event<T>(string name, out Event<T> @event)
            => CommitActivities().Event(name, out @event);

        public StateMachineModifier<TInstance> Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression, Expression<Func<TProperty, Event<T>>> eventPropertyExpression) where TProperty : class
            => CommitActivities().Event(propertyExpression, eventPropertyExpression);

        public StateMachineModifier<TInstance> Finally(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
            => CommitActivities().Finally(activityCallback);

        public StateMachineEventActivitiesBuilder<TInstance> Initially()
            => CommitActivities().Initially();

        public StateMachineModifier<TInstance> InstanceState(Expression<Func<TInstance, State>> instanceStateProperty)
            => CommitActivities().InstanceState(instanceStateProperty);

        public StateMachineModifier<TInstance> InstanceState(Expression<Func<TInstance, string>> instanceStateProperty)
            => CommitActivities().InstanceState(instanceStateProperty);

        public StateMachineModifier<TInstance> InstanceState(Expression<Func<TInstance, int>> instanceStateProperty, params State[] states)
            => CommitActivities().InstanceState(instanceStateProperty, states);

        public StateMachineModifier<TInstance> Name(string machineName)
            => CommitActivities().Name(machineName);

        public StateMachineModifier<TInstance> OnUnhandledEvent(UnhandledEventCallback<TInstance> callback)
            => CommitActivities().OnUnhandledEvent(callback);

        public StateMachineModifier<TInstance> State(string name, out State<TInstance> state)
            => CommitActivities().State(name, out state);

        public StateMachineModifier<TInstance> State(string name, out State state)
            => CommitActivities().State(name, out state);

        public StateMachineModifier<TInstance> State<TProperty>(Expression<Func<TProperty>> propertyExpression, Expression<Func<TProperty, State>> statePropertyExpression) where TProperty : class
            => CommitActivities().State(propertyExpression, statePropertyExpression);

        public StateMachineModifier<TInstance> SubState(string name, State superState, out State<TInstance> subState)
            => CommitActivities().SubState(name, superState, out subState);

        public StateMachineModifier<TInstance> SubState<TProperty>(Expression<Func<TProperty>> propertyExpression, Expression<Func<TProperty, State>> statePropertyExpression, State superState) where TProperty : class
            => CommitActivities().SubState(propertyExpression, statePropertyExpression, superState);

        public StateMachineModifier<TInstance> WhenEnter(State state, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
            => CommitActivities().WhenEnter(state, activityCallback);

        public StateMachineModifier<TInstance> WhenEnterAny(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
            => CommitActivities().WhenEnterAny(activityCallback);

        public StateMachineModifier<TInstance> WhenLeave(State state, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
            => CommitActivities().WhenLeave(state, activityCallback);

        public StateMachineModifier<TInstance> WhenLeaveAny(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
            => CommitActivities().WhenLeaveAny(activityCallback);
        #endregion

        public StateMachineEventActivitiesBuilder<TInstance> When(Event @event, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> configure)
        {
            _activities.Add(configure(_machine.When(@event)));
            return this;
        }

        public StateMachineEventActivitiesBuilder<TInstance> When(Event @event, StateMachineEventFilter<TInstance> filter, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> configure)
        {
            _activities.Add(configure(_machine.When(@event, filter)));
            return this;
        }

        public StateMachineEventActivitiesBuilder<TInstance> When<TData>(Event<TData> @event, Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> configure)
        {
            _activities.Add(configure(_machine.When(@event)));
            return this;
        }

        public StateMachineEventActivitiesBuilder<TInstance> When<TData>(Event<TData> @event, StateMachineEventFilter<TInstance, TData> filter, Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> configure)
        {
            _activities.Add(configure(_machine.When(@event, filter)));
            return this;
        }

        public StateMachineEventActivitiesBuilder<TInstance> Ignore(Event @event)
        {
            _activities.Add(_machine.Ignore(@event));
            return this;
        }

        public StateMachineEventActivitiesBuilder<TInstance> Ignore<TData>(Event<TData> @event)
        {
            _activities.Add(_machine.Ignore(@event));
            return this;
        }

        public StateMachineEventActivitiesBuilder<TInstance> Ignore<TData>(Event<TData> @event, StateMachineEventFilter<TInstance, TData> filter)
        {
            _activities.Add(_machine.Ignore(@event, filter));
            return this;
        }

        public void Apply()
            => CommitActivities().Apply();
    }
}
