namespace Automatonymous.Binders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class TriggerEventActivityBinder<TInstance> :
        EventActivityBinder<TInstance>
        where TInstance : class
    {
        readonly ActivityBinder<TInstance>[] _activities;
        readonly StateMachineEventFilter<TInstance> _filter;
        readonly StateMachine<TInstance> _machine;
        readonly Event _event;

        public TriggerEventActivityBinder(StateMachine<TInstance> machine, Event @event, params ActivityBinder<TInstance>[] activities)
        {
            _event = @event;
            _machine = machine;
            _activities = activities ?? Array.Empty<ActivityBinder<TInstance>>();
        }

        public TriggerEventActivityBinder(StateMachine<TInstance> machine, Event @event, StateMachineEventFilter<TInstance> filter,
            params ActivityBinder<TInstance>[] activities) : this(machine, @event, activities)
        {
            _filter = filter;
        }

        TriggerEventActivityBinder(StateMachine<TInstance> machine, Event @event, StateMachineEventFilter<TInstance> filter,
            ActivityBinder<TInstance>[] activities,
            params ActivityBinder<TInstance>[] appendActivity) : this(machine, @event, filter, activities)
        {
            _activities = new ActivityBinder<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);
        }

        Event EventActivityBinder<TInstance>.Event => _event;

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.Add(Activity<TInstance> activity) =>
            new TriggerEventActivityBinder<TInstance>(_machine, _event, _filter, _activities, new ExecuteActivityBinder<TInstance>(_event, activity));

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.Catch<T>(
            Func<ExceptionActivityBinder<TInstance, T>, ExceptionActivityBinder<TInstance, T>> activityCallback)
        {
            ExceptionActivityBinder<TInstance, T> binder = new CatchExceptionActivityBinder<TInstance, T>(_machine, _event);
            binder = activityCallback(binder);
            return new TriggerEventActivityBinder<TInstance>(_machine, _event, _filter, _activities, new CatchActivityBinder<TInstance, T>(_event, binder));
        }

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.If(StateMachineCondition<TInstance> condition,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback) =>
                IfElse(condition, activityCallback, _ => _);

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.IfAsync(StateMachineAsyncCondition<TInstance> condition,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback) =>
                IfElseAsync(condition, activityCallback, _ => _);

        public EventActivityBinder<TInstance> IfElse(StateMachineCondition<TInstance> condition,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> thenActivityCallback,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> elseActivityCallback) =>
                new TriggerEventActivityBinder<TInstance>(_machine, _event, _filter, _activities,
                    new ConditionalActivityBinder<TInstance>(_event, condition, GetBinder(thenActivityCallback), GetBinder(elseActivityCallback)));

        public EventActivityBinder<TInstance> IfElseAsync(StateMachineAsyncCondition<TInstance> condition,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> thenActivityCallback,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> elseActivityCallback) =>
                new TriggerEventActivityBinder<TInstance>(_machine, _event, _filter, _activities,
                    new ConditionalActivityBinder<TInstance>(_event, condition, GetBinder(thenActivityCallback), GetBinder(elseActivityCallback)));

        StateMachine<TInstance> EventActivityBinder<TInstance>.StateMachine => _machine;

        public IEnumerable<ActivityBinder<TInstance>> GetStateActivityBinders() =>
            _filter != null ? Enumerable.Repeat(CreateConditionalActivityBinder(), 1) : _activities;

        EventActivityBinder<TInstance> GetBinder(
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback) =>
                activityCallback(new TriggerEventActivityBinder<TInstance>(_machine, _event));

        ActivityBinder<TInstance> CreateConditionalActivityBinder() => new ConditionalActivityBinder<TInstance>(_event,
            context => _filter(context),
            new TriggerEventActivityBinder<TInstance>(_machine, _event, _activities),
            new TriggerEventActivityBinder<TInstance>(_machine, _event));
    }
}
