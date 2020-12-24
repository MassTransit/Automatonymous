namespace Automatonymous.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Binders;


    class InternalStateMachineModifier<TInstance> : StateMachineModifier<TInstance>
        where TInstance : class
    {
        readonly List<StateMachineEventActivitiesBuilder<TInstance>> _activityBuilders;
        readonly AutomatonymousStateMachine<TInstance> _machine;

        public InternalStateMachineModifier(AutomatonymousStateMachine<TInstance> machine)
        {
            _machine = machine ?? throw new ArgumentNullException(nameof(machine));
            _activityBuilders = new List<StateMachineEventActivitiesBuilder<TInstance>>();
        }

        public State Initial => _machine.Initial;
        public State Final => _machine.Final;

        public void Apply()
        {
            var uncommittedActivities = _activityBuilders
                .Where(builder => !builder.IsCommitted)
                .ToArray();

            foreach (var builder in uncommittedActivities)
                builder.CommitActivities();
        }

        public StateMachineEventActivitiesBuilder<TInstance> During(params State[] states)
        {
            var builder = new InternalStateMachineEventActivitiesBuilder<TInstance>(_machine, this,
                activities => _machine.During(states, activities));
            _activityBuilders.Add(builder);
            return builder;
        }

        public StateMachineEventActivitiesBuilder<TInstance> DuringAny()
        {
            var builder = new InternalStateMachineEventActivitiesBuilder<TInstance>(_machine, this,
                activities => _machine.DuringAny(activities));
            _activityBuilders.Add(builder);
            return builder;
        }

        public StateMachineEventActivitiesBuilder<TInstance> Initially()
        {
            var builder = new InternalStateMachineEventActivitiesBuilder<TInstance>(_machine, this,
                activities => _machine.Initially(activities));
            _activityBuilders.Add(builder);
            return builder;
        }

        public StateMachineModifier<TInstance> AfterLeave(State state,
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            _machine.AfterLeave(state, activityCallback);
            return this;
        }

        public StateMachineModifier<TInstance> AfterLeaveAny(
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            _machine.AfterLeaveAny(activityCallback);
            return this;
        }

        public StateMachineModifier<TInstance> BeforeEnter(State state,
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            _machine.BeforeEnter(state, activityCallback);
            return this;
        }

        public StateMachineModifier<TInstance> BeforeEnterAny(
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            _machine.BeforeEnterAny(activityCallback);
            return this;
        }

        public StateMachineModifier<TInstance> CompositeEvent(Event @event,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression, params Event[] events)
        {
            _machine.CompositeEvent(@event, trackingPropertyExpression, events);
            return this;
        }

        public StateMachineModifier<TInstance> CompositeEvent(Event @event,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression, CompositeEventOptions options,
            params Event[] events)
        {
            _machine.CompositeEvent(@event, trackingPropertyExpression, options, events);
            return this;
        }

        public StateMachineModifier<TInstance> CompositeEvent(Event @event, Expression<Func<TInstance, int>> trackingPropertyExpression,
            params Event[] events)
        {
            _machine.CompositeEvent(@event, trackingPropertyExpression, events);
            return this;
        }

        public StateMachineModifier<TInstance> CompositeEvent(Event @event, Expression<Func<TInstance, int>> trackingPropertyExpression,
            CompositeEventOptions options, params Event[] events)
        {
            _machine.CompositeEvent(@event, trackingPropertyExpression, options, events);
            return this;
        }

        public StateMachineModifier<TInstance> Event(string name, out Event @event)
        {
            @event = _machine.Event(name);
            return this;
        }

        public StateMachineModifier<TInstance> Event<T>(string name, out Event<T> @event)
        {
            @event = _machine.Event<T>(name);
            return this;
        }

        public StateMachineModifier<TInstance> Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, Event<T>>> eventPropertyExpression) where TProperty : class
        {
            _machine.Event(propertyExpression, eventPropertyExpression);
            return this;
        }

        public StateMachineModifier<TInstance> Finally(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            _machine.Finally(activityCallback);
            return this;
        }

        public StateMachineModifier<TInstance> InstanceState(Expression<Func<TInstance, State>> instanceStateProperty)
        {
            _machine.InstanceState(instanceStateProperty);
            return this;
        }

        public StateMachineModifier<TInstance> InstanceState(Expression<Func<TInstance, string>> instanceStateProperty)
        {
            _machine.InstanceState(instanceStateProperty);
            return this;
        }

        public StateMachineModifier<TInstance> InstanceState(Expression<Func<TInstance, int>> instanceStateProperty, params State[] states)
        {
            _machine.InstanceState(instanceStateProperty, states);
            return this;
        }

        public StateMachineModifier<TInstance> Name(string machineName)
        {
            _machine.Name(machineName);
            return this;
        }

        public StateMachineModifier<TInstance> OnUnhandledEvent(UnhandledEventCallback<TInstance> callback)
        {
            _machine.OnUnhandledEvent(callback);
            return this;
        }

        public StateMachineModifier<TInstance> State(string name, out State<TInstance> state)
        {
            state = _machine.State(name);
            return this;
        }

        public StateMachineModifier<TInstance> State(string name, out State state)
        {
            state = _machine.State(name);
            return this;
        }

        public StateMachineModifier<TInstance> State<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression) where TProperty : class
        {
            _machine.State(propertyExpression, statePropertyExpression);
            return this;
        }

        public StateMachineModifier<TInstance> SubState(string name, State superState, out State<TInstance> subState)
        {
            subState = _machine.SubState(name, superState);
            return this;
        }

        public StateMachineModifier<TInstance> SubState<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression, State superState) where TProperty : class
        {
            _machine.SubState(propertyExpression, statePropertyExpression, superState);
            return this;
        }

        public StateMachineModifier<TInstance> WhenEnter(State state,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            _machine.WhenEnter(state, activityCallback);
            return this;
        }

        public StateMachineModifier<TInstance> WhenEnterAny(
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            _machine.WhenEnterAny(activityCallback);
            return this;
        }

        public StateMachineModifier<TInstance> WhenLeave(State state,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            _machine.WhenLeave(state, activityCallback);
            return this;
        }

        public StateMachineModifier<TInstance> WhenLeaveAny(
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            _machine.WhenLeaveAny(activityCallback);
            return this;
        }

        public StateMachineModifier<TInstance> InstanceState(Expression<Func<TInstance, int>> instanceStateProperty,
            params string[] stateNames)
        {
            // NOTE: May need to re-think this; Assumes the states have already been declared.
            State[] states = stateNames
                .Select(name => _machine.GetState(name))
                .ToArray();

            _machine.InstanceState(instanceStateProperty, states);
            return this;
        }
    }
}
