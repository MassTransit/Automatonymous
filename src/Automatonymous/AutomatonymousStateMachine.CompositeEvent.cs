namespace Automatonymous
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Accessors;
    using Activities;
    using Events;
    using GreenPipes.Internals.Extensions;


    public abstract partial class AutomatonymousStateMachine<TInstance>
        where TInstance : class
    {
        private Func<State<TInstance>, CompositeEventOptions, bool> DefaultFilter =>
            (state, options) => options.HasFlag(CompositeEventOptions.IncludeInitial) || !Equals(state, Initial);

        /// <summary>
        /// Adds a composite event to the state machine. A composite event is triggered when all
        /// off the required events have been raised. Note that required events cannot be in the initial
        /// state since it would cause extra instances of the state machine to be created
        /// </summary>
        /// <param name="propertyExpression">The composite event</param>
        /// <param name="trackingPropertyExpression">The property in the instance used to track the state of the composite event</param>
        /// <param name="events">The events that must be raised before the composite event is raised</param>
        protected internal virtual void CompositeEvent(Expression<Func<Event>> propertyExpression,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            params Event[] events) =>
                CompositeEvent(propertyExpression, trackingPropertyExpression, CompositeEventOptions.None, events);

        /// <summary>
        /// Adds a composite event to the state machine. A composite event is triggered when all
        /// off the required events have been raised. Note that required events cannot be in the initial
        /// state since it would cause extra instances of the state machine to be created
        /// </summary>
        /// <param name="propertyExpression">The composite event</param>
        /// <param name="trackingPropertyExpression">The property in the instance used to track the state of the composite event</param>
        /// <param name="options">Options on the composite event</param>
        /// <param name="events">The events that must be raised before the composite event is raised</param>
        protected internal virtual void CompositeEvent(Expression<Func<Event>> propertyExpression,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events) =>
                CompositeEvent(propertyExpression, x => DefaultFilter(x, options), trackingPropertyExpression, events);

        /// <summary>
        /// Adds a composite event to the state machine. A composite event is triggered when all
        /// off the required events have been raised. Note that required events cannot be in the initial
        /// state since it would cause extra instances of the state machine to be created.
        /// Using the assignToStatesFilter allows one to assign the composite event to one or more specific states in stead of the composite
        /// event being assigned to every state in the state machine.
        /// </summary>
        /// <param name="propertyExpression">The composite event</param>
        /// <param name="assignToStatesFilter">Filter to apply on the state machine states</param>
        /// <param name="trackingPropertyExpression">The property in the instance used to track the state of the composite event</param>
        /// <param name="events">The events that must be raised before the composite event is raised</param>
        protected internal virtual void CompositeEvent(
            Expression<Func<Event>> propertyExpression,
            Func<State<TInstance>, bool> assignToStatesFilter,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            params Event[] events) => CompositeEvent(() =>
            {
                var eventProperty = propertyExpression.GetPropertyInfo();
                return ConfigurationHelpers.InitializeEvent(this, eventProperty, new TriggerEvent(eventProperty.Name, true));
            }, new StructCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()), events, assignToStatesFilter);

        /// <summary>
        /// Adds a composite event to the state machine. A composite event is triggered when all
        /// off the required events have been raised. Note that required events cannot be in the initial
        /// state since it would cause extra instances of the state machine to be created
        /// </summary>
        /// <param name="propertyExpression">The composite event</param>
        /// <param name="trackingPropertyExpression">The property in the instance used to track the state of the composite event</param>
        /// <param name="events">The events that must be raised before the composite event is raised</param>
        protected internal virtual void CompositeEvent(Expression<Func<Event>> propertyExpression,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            params Event[] events) =>
                CompositeEvent(propertyExpression, x => DefaultFilter(x, CompositeEventOptions.None), trackingPropertyExpression, events);

        protected internal virtual void CompositeEvent(Expression<Func<Event>> propertyExpression,
            Func<State<TInstance>, bool> assignToStatesFilter,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            params Event[] events) => CompositeEvent(() =>
            {
                var eventProperty = propertyExpression.GetPropertyInfo();
                return ConfigurationHelpers.InitializeEvent(this, eventProperty, new TriggerEvent(eventProperty.Name, true));
            }, new IntCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()), events, assignToStatesFilter);

        /// <summary>
        /// Adds a composite event to the state machine. A composite event is triggered when all
        /// off the required events have been raised. Note that required events cannot be in the initial
        /// state since it would cause extra instances of the state machine to be created
        /// </summary>
        /// <param name="propertyExpression">The composite event</param>
        /// <param name="trackingPropertyExpression">The property in the instance used to track the state of the composite event</param>
        /// <param name="options">Options on the composite event</param>
        /// <param name="events">The events that must be raised before the composite event is raised</param>
        protected internal virtual void CompositeEvent(Expression<Func<Event>> propertyExpression,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events) => CompositeEvent(() =>
            {
                var eventProperty = propertyExpression.GetPropertyInfo();
                return ConfigurationHelpers.InitializeEvent(this, eventProperty, new TriggerEvent(eventProperty.Name, true));
            }, new IntCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()), events, x => DefaultFilter(x, options));

        internal virtual void CompositeEvent(string name,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            params Event[] events) =>
                CompositeEvent(name, trackingPropertyExpression, CompositeEventOptions.None, events);

        internal virtual Event CompositeEvent(string name,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events) =>
                CompositeEvent(name, new StructCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()), options, events);

        internal virtual Event CompositeEvent(string name,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            params Event[] events) =>
                CompositeEvent(name, trackingPropertyExpression, CompositeEventOptions.None, events);

        internal virtual Event CompositeEvent(string name,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events) =>
                CompositeEvent(name, new IntCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()), options, events);

        protected internal virtual void CompositeEvent(Event @event,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            params Event[] events) =>
                CompositeEvent(@event, trackingPropertyExpression, CompositeEventOptions.None, events);

        protected internal virtual Event CompositeEvent(Event @event,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events) =>
                CompositeEvent(() => @event, new StructCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()), events, x => DefaultFilter(x, options));

        protected internal virtual Event CompositeEvent(Event @event,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            params Event[] events) => CompositeEvent(@event, trackingPropertyExpression, CompositeEventOptions.None, events);

        protected internal virtual Event CompositeEvent(Event @event,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events) =>
                CompositeEvent(() => @event, new IntCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()), events, x => DefaultFilter(x, options));

        private Event CompositeEvent(string name, CompositeEventStatusAccessor<TInstance> accessor, CompositeEventOptions options, Event[] events) =>
            CompositeEvent(() =>
            {
                var @event = new TriggerEvent(name, true);
                _eventCache[name] = new StateMachineEvent(@event, false);
                return @event;
            }, accessor, events, x => DefaultFilter(x, options));

        private Event CompositeEvent(Func<Event> getEventFunc, CompositeEventStatusAccessor<TInstance> accessor, Event[] events, Func<State<TInstance>, bool> filter = null)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }
            if (events.Length > 31)
            {
                throw new ArgumentException("No more than 31 events can be combined into a single event");
            }
            if (events.Length == 0)
            {
                throw new ArgumentException("At least one event must be specified for a composite event");
            }
            if (events.Any(x => x == null))
            {
                throw new ArgumentException("One or more events specified has not yet been initialized");
            }

            var complete = new CompositeEventStatus(Enumerable.Range(0, events.Length).Aggregate(0, (current, x) => current | (1 << x)));
            var @event = getEventFunc();
            @event.IsComposite = true;
            _eventCache[@event.Name].Event = @event;
            for (var i = 0; i < events.Length; i++)
            {
                var flag = 1 << i;
                var activity = new CompositeEventActivity<TInstance>(accessor, flag, complete, @event);

                var states = _stateCache.Values.Where(x => filter?.Invoke(x) ?? !Equals(x, Initial));
                foreach (var state in states)
                {
                    During(state, When(events[i]).Execute(activity));
                }
            }

            return @event;
        }
    }
}
