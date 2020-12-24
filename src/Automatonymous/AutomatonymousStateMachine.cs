namespace Automatonymous
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Accessors;
    using Activities;
    using Binders;
    using Builder;
    using Contexts;
    using Events;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Observers;
    using States;


    public abstract class AutomatonymousStateMachine<TInstance> :
        StateMachine<TInstance>
        where TInstance : class
    {
        readonly Dictionary<string, StateMachineEvent<TInstance>> _eventCache;
        readonly EventObservable<TInstance> _eventObservers;
        readonly State<TInstance> _final;
        readonly State<TInstance> _initial;
        readonly Lazy<ConfigurationHelpers.StateMachineRegistration[]> _registrations;
        readonly Dictionary<string, State<TInstance>> _stateCache;
        readonly StateObservable<TInstance> _stateObservers;
        StateAccessor<TInstance> _accessor;
        string _name;
        UnhandledEventCallback<TInstance> _unhandledEventCallback;

        protected AutomatonymousStateMachine()
        {
            _registrations = new Lazy<ConfigurationHelpers.StateMachineRegistration[]>(() => ConfigurationHelpers.GetRegistrations(this));
            _stateCache = new Dictionary<string, State<TInstance>>();
            _eventCache = new Dictionary<string, StateMachineEvent<TInstance>>();

            _eventObservers = new EventObservable<TInstance>();
            _stateObservers = new StateObservable<TInstance>();

            _initial = new StateMachineState<TInstance>(this, "Initial", _eventObservers);
            _stateCache[_initial.Name] = _initial;
            _final = new StateMachineState<TInstance>(this, "Final", _eventObservers);
            _stateCache[_final.Name] = _final;

            _accessor = new DefaultInstanceStateAccessor<TInstance>(this, _stateCache[Initial.Name], _stateObservers);

            _unhandledEventCallback = DefaultUnhandledEventCallback;

            _name = GetType().Name;

            RegisterImplicit();
        }

        IEnumerable<State<TInstance>> IntrospectionStates
        {
            get
            {
                yield return _initial;

                foreach (var x in _stateCache.Values)
                {
                    if (Equals(x, Initial) || Equals(x, Final))
                        continue;

                    yield return x;
                }

                yield return _final;
            }
        }

        string StateMachine.Name => _name;
        StateAccessor<TInstance> StateMachine<TInstance>.Accessor => _accessor;
        public State Initial => _initial;
        public State Final => _final;

        State StateMachine.GetState(string name)
        {
            if (_stateCache.TryGetValue(name, out var result))
                return result;

            throw new UnknownStateException(_name, name);
        }

        async Task StateMachine<TInstance>.RaiseEvent(EventContext<TInstance> context)
        {
            var state = await _accessor.Get(context).ConfigureAwait(false);

            if (!_stateCache.TryGetValue(state.Name, out var instanceState))
                throw new UnknownStateException(_name, state.Name);

            await instanceState.Raise(context).ConfigureAwait(false);
        }

        async Task StateMachine<TInstance>.RaiseEvent<T>(EventContext<TInstance, T> context)
        {
            var state = await _accessor.Get(context).ConfigureAwait(false);

            if (!_stateCache.TryGetValue(state.Name, out var instanceState))
                throw new UnknownStateException(_name, state.Name);

            await instanceState.Raise(context).ConfigureAwait(false);
        }

        public State<TInstance> GetState(string name)
        {
            if (TryGetState(name, out var result))
                return result;

            throw new UnknownStateException(_name, name);
        }

        public IEnumerable<State> States => _stateCache.Values;

        Event StateMachine.GetEvent(string name)
        {
            if (_eventCache.TryGetValue(name, out var result))
                return result.Event;

            throw new UnknownEventException(_name, name);
        }

        public IEnumerable<Event> Events
        {
            get { return _eventCache.Values.Where(x => false == x.IsTransitionEvent).Select(x => x.Event); }
        }

        Type StateMachine.InstanceType => typeof(TInstance);

        IEnumerable<Event> StateMachine.NextEvents(State state)
        {
            if (_stateCache.TryGetValue(state.Name, out var result))
                return result.Events;

            throw new UnknownStateException(_name, state.Name);
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            foreach (var x in IntrospectionStates)
                x.Accept(visitor);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("stateMachine");

            var stateMachineType = GetType();
            scope.Add("name", stateMachineType.Name);
            scope.Add("instanceType", TypeCache<TInstance>.ShortName);

            _accessor.Probe(scope);

            foreach (var state in IntrospectionStates)
                state.Probe(scope);
        }

        IDisposable StateMachine<TInstance>.ConnectEventObserver(EventObserver<TInstance> observer)
        {
            var eventObserver = new NonTransitionEventObserver<TInstance>(_eventCache, observer);

            return _eventObservers.Connect(eventObserver);
        }

        IDisposable StateMachine<TInstance>.ConnectEventObserver(Event @event, EventObserver<TInstance> observer)
        {
            var eventObserver = new SelectedEventObserver<TInstance>(@event, observer);

            return _eventObservers.Connect(eventObserver);
        }

        IDisposable StateMachine<TInstance>.ConnectStateObserver(StateObserver<TInstance> observer)
        {
            return _stateObservers.Connect(observer);
        }

        public bool TryGetState(string name, out State<TInstance> state)
        {
            return _stateCache.TryGetValue(name, out state);
        }

        Task DefaultUnhandledEventCallback(UnhandledEventContext<TInstance> context)
        {
            throw new UnhandledEventException(_name, context.Event.Name, context.CurrentState.Name);
        }

        /// <summary>
        /// Declares what property holds the TInstance's state on the current instance of the state machine
        /// </summary>
        /// <param name="instanceStateProperty"></param>
        /// <remarks>Setting the state accessor more than once will cause the property managed by the state machine to change each time.
        /// Please note, the state machine can only manage one property at a given time per instance,
        /// and the best practice is to manage one property per machine.
        /// </remarks>
        protected internal void InstanceState(Expression<Func<TInstance, State>> instanceStateProperty)
        {
            var stateAccessor = new RawStateAccessor<TInstance>(this, instanceStateProperty, _stateObservers);

            _accessor = new InitialIfNullStateAccessor<TInstance>(_stateCache[Initial.Name], stateAccessor);
        }

        /// <summary>
        /// Declares the property to hold the instance's state as a string (the state name is stored in the property)
        /// </summary>
        /// <param name="instanceStateProperty"></param>
        protected internal void InstanceState(Expression<Func<TInstance, string>> instanceStateProperty)
        {
            var stateAccessor = new StringStateAccessor<TInstance>(this, instanceStateProperty, _stateObservers);

            _accessor = new InitialIfNullStateAccessor<TInstance>(_stateCache[Initial.Name], stateAccessor);
        }

        /// <summary>
        /// Declares the property to hold the instance's state as an int (0 - none, 1 = initial, 2 = final, 3... the rest)
        /// </summary>
        /// <param name="instanceStateProperty"></param>
        /// <param name="states">Specifies the states, in order, to which the int values should be assigned</param>
        protected internal void InstanceState(Expression<Func<TInstance, int>> instanceStateProperty, params State[] states)
        {
            var stateIndex = new StateAccessorIndex<TInstance>(this, _initial, _final, states);

            var stateAccessor = new IntStateAccessor<TInstance>(instanceStateProperty, stateIndex, _stateObservers);

            _accessor = new InitialIfNullStateAccessor<TInstance>(_stateCache[Initial.Name], stateAccessor);
        }

        /// <summary>
        /// Specifies the name of the state machine
        /// </summary>
        /// <param name="machineName"></param>
        protected internal void Name(string machineName)
        {
            if (string.IsNullOrWhiteSpace(machineName))
                throw new ArgumentException("The machine name must not be empty", nameof(machineName));

            _name = machineName;
        }

        /// <summary>
        /// Declares an event, and initializes the event property
        /// </summary>
        /// <param name="propertyExpression"></param>
        protected internal virtual void Event(Expression<Func<Event>> propertyExpression)
        {
            DeclarePropertyBasedEvent(prop => DeclareTriggerEvent(prop.Name), propertyExpression.GetPropertyInfo());
        }

        protected internal virtual Event Event(string name)
        {
            return DeclareTriggerEvent(name);
        }

        Event DeclareTriggerEvent(string name)
        {
            return DeclareEvent(name => new TriggerEvent(name), name);
        }

        /// <summary>
        /// Declares a data event on the state machine, and initializes the property
        /// </summary>
        /// <param name="propertyExpression">The event property</param>
        protected internal virtual void Event<T>(Expression<Func<Event<T>>> propertyExpression)
        {
            DeclarePropertyBasedEvent(prop => DeclareDataEvent<T>(prop.Name), propertyExpression.GetPropertyInfo());
        }

        protected internal virtual Event<T> Event<T>(string name)
        {
            return DeclareDataEvent<T>(name);
        }

        Event<T> DeclareDataEvent<T>(string name)
        {
            return DeclareEvent(name => new DataEvent<T>(name), name);
        }

        void DeclarePropertyBasedEvent<TEvent>(Func<PropertyInfo, TEvent> ctor, PropertyInfo property)
            where TEvent : Event
        {
            var @event = ctor(property);
            ConfigurationHelpers.InitializeEvent(this, property, @event);
        }

        TEvent DeclareEvent<TEvent>(Func<string, TEvent> ctor, string name)
            where TEvent : Event
        {
            var @event = ctor(name);
            _eventCache[name] = new StateMachineEvent<TInstance>(@event, false);
            return @event;
        }

        /// <summary>
        /// Declares a data event on a property of the state machine, and initializes the property
        /// </summary>
        /// <param name="propertyExpression">The property</param>
        /// <param name="eventPropertyExpression">The event property on the property</param>
        protected internal virtual void Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, Event<T>>> eventPropertyExpression)
            where TProperty : class
        {
            var property = propertyExpression.GetPropertyInfo();
            var propertyValue = property.GetValue(this, null) as TProperty;
            if (propertyValue == null)
                throw new ArgumentException("The property is not initialized: " + property.Name, nameof(propertyExpression));

            var eventProperty = eventPropertyExpression.GetPropertyInfo();

            var name = $"{property.Name}.{eventProperty.Name}";

            var @event = new DataEvent<T>(name);

            ConfigurationHelpers.InitializeEventProperty<TProperty, T>(eventProperty, propertyValue, @event);

            _eventCache[name] = new StateMachineEvent<TInstance>(@event, false);
        }

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
            params Event[] events)
        {
            var trackingPropertyInfo = trackingPropertyExpression.GetPropertyInfo();

            var accessor = new StructCompositeEventStatusAccessor<TInstance>(trackingPropertyInfo);

            CompositeEvent(propertyExpression, accessor, CompositeEventOptions.None, events);
        }

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
            params Event[] events)
        {
            var trackingPropertyInfo = trackingPropertyExpression.GetPropertyInfo();

            var accessor = new StructCompositeEventStatusAccessor<TInstance>(trackingPropertyInfo);

            CompositeEvent(propertyExpression, accessor, options, events);
        }

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
            params Event[] events)
        {
            var trackingPropertyInfo = trackingPropertyExpression.GetPropertyInfo();

            var accessor = new IntCompositeEventStatusAccessor<TInstance>(trackingPropertyInfo);

            CompositeEvent(propertyExpression, accessor, CompositeEventOptions.None, events);
        }

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
            params Event[] events)
        {
            var trackingPropertyInfo = trackingPropertyExpression.GetPropertyInfo();

            var accessor = new IntCompositeEventStatusAccessor<TInstance>(trackingPropertyInfo);

            CompositeEvent(propertyExpression, accessor, options, events);
        }

        void CompositeEvent(Expression<Func<Event>> propertyExpression, CompositeEventStatusAccessor<TInstance> accessor,
            CompositeEventOptions options, Event[] events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));
            if (events.Length > 31)
                throw new ArgumentException("No more than 31 events can be combined into a single event");
            if (events.Length == 0)
                throw new ArgumentException("At least one event must be specified for a composite event");
            if (events.Any(x => x == null))
                throw new ArgumentException("One or more events specified has not yet been initialized");

            var eventProperty = propertyExpression.GetPropertyInfo();

            var name = eventProperty.Name;

            var @event = new TriggerEvent(name);

            ConfigurationHelpers.InitializeEvent(this, eventProperty, @event);

            _eventCache[name] = new StateMachineEvent<TInstance>(@event, false);

            var complete = new CompositeEventStatus(Enumerable.Range(0, events.Length)
                .Aggregate(0, (current, x) => current | (1 << x)));

            for (var i = 0; i < events.Length; i++)
            {
                var flag = 1 << i;

                var activity = new CompositeEventActivity<TInstance>(accessor, flag, complete, @event);

                bool Filter(State<TInstance> x)
                {
                    return options.HasFlag(CompositeEventOptions.IncludeInitial) || !Equals(x, Initial);
                }

                foreach (var state in _stateCache.Values.Where(Filter))
                    During(state,
                        When(events[i])
                            .Execute(activity));
            }
        }

        internal virtual void CompositeEvent(string name,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            params Event[] events)
        {
            CompositeEvent(name, new StructCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()),
                CompositeEventOptions.None, events);
        }

        internal virtual Event CompositeEvent(string name,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events)
        {
            return CompositeEvent(name, new StructCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()),
                options, events);
        }

        internal virtual Event CompositeEvent(string name,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            params Event[] events)
        {
            return CompositeEvent(name, new IntCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()),
                CompositeEventOptions.None, events);
        }

        internal virtual Event CompositeEvent(string name,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events)
        {
            return CompositeEvent(name, new IntCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()),
                options, events);
        }

        Event CompositeEvent(string name, CompositeEventStatusAccessor<TInstance> accessor,
            CompositeEventOptions options, Event[] events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));
            if (events.Length > 31)
                throw new ArgumentException("No more than 31 events can be combined into a single event");
            if (events.Length == 0)
                throw new ArgumentException("At least one event must be specified for a composite event");
            if (events.Any(x => x == null))
                throw new ArgumentException("One or more events specified has not yet been initialized");

            var @event = new TriggerEvent(name);

            _eventCache[name] = new StateMachineEvent<TInstance>(@event, false);

            var complete = new CompositeEventStatus(Enumerable.Range(0, events.Length)
                .Aggregate(0, (current, x) => current | (1 << x)));

            for (var i = 0; i < events.Length; i++)
            {
                var flag = 1 << i;

                var activity = new CompositeEventActivity<TInstance>(accessor, flag, complete, @event);

                bool Filter(State<TInstance> x)
                {
                    return options.HasFlag(CompositeEventOptions.IncludeInitial) || !Equals(x, Initial);
                }

                foreach (var state in _stateCache.Values.Where(Filter))
                    During(state,
                        When(events[i])
                            .Execute(activity));
            }

            return @event;
        }

        protected internal virtual void CompositeEvent(Event @event,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            params Event[] events)
        {
            CompositeEvent(@event, new StructCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()),
                CompositeEventOptions.None, events);
        }

        protected internal virtual Event CompositeEvent(Event @event,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events)
        {
            return CompositeEvent(@event, new StructCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()),
                options, events);
        }

        protected internal virtual Event CompositeEvent(Event @event,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            params Event[] events)
        {
            return CompositeEvent(@event, new IntCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()),
                CompositeEventOptions.None, events);
        }

        protected internal virtual Event CompositeEvent(Event @event,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events)
        {
            return CompositeEvent(@event, new IntCompositeEventStatusAccessor<TInstance>(trackingPropertyExpression.GetPropertyInfo()),
                options, events);
        }

        Event CompositeEvent(Event @event, CompositeEventStatusAccessor<TInstance> accessor,
            CompositeEventOptions options, Event[] events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));
            if (events.Length > 31)
                throw new ArgumentException("No more than 31 events can be combined into a single event");
            if (events.Length == 0)
                throw new ArgumentException("At least one event must be specified for a composite event");
            if (events.Any(x => x == null))
                throw new ArgumentException("One or more events specified has not yet been initialized");

            var complete = new CompositeEventStatus(Enumerable.Range(0, events.Length)
                .Aggregate(0, (current, x) => current | (1 << x)));

            for (var i = 0; i < events.Length; i++)
            {
                var flag = 1 << i;

                var activity = new CompositeEventActivity<TInstance>(accessor, flag, complete, @event);

                bool Filter(State<TInstance> x)
                {
                    return options.HasFlag(CompositeEventOptions.IncludeInitial) || !Equals(x, Initial);
                }

                foreach (var state in _stateCache.Values.Where(Filter))
                    During(state,
                        When(events[i])
                            .Execute(activity));
            }

            return @event;
        }

        /// <summary>
        /// Declares a state on the state machine, and initialized the property
        /// </summary>
        /// <param name="propertyExpression">The state property</param>
        protected internal virtual void State(Expression<Func<State>> propertyExpression)
        {
            var property = propertyExpression.GetPropertyInfo();

            DeclareState(property);
        }

        protected internal virtual State<TInstance> State(string name)
        {
            if (TryGetState(name, out var foundState))
                return foundState;

            var state = new StateMachineState<TInstance>(this, name, _eventObservers);
            SetState(name, state);

            return state;
        }

        void DeclareState(PropertyInfo property)
        {
            var name = property.Name;

            var propertyValue = property.GetValue(this);

            // If the state was already defined, don't define it again
            var existingState = propertyValue as StateMachineState<TInstance>;
            if (name.Equals(existingState?.Name))
                return;

            var state = new StateMachineState<TInstance>(this, name, _eventObservers);

            ConfigurationHelpers.InitializeState(this, property, state);

            SetState(name, state);
        }

        /// <summary>
        /// Declares a state on the state machine, and initialized the property
        /// </summary>
        /// <param name="propertyExpression">The property containing the state</param>
        /// <param name="statePropertyExpression">The state property</param>
        protected internal virtual void State<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression)
            where TProperty : class
        {
            var property = propertyExpression.GetPropertyInfo();
            var propertyValue = property.GetValue(this, null) as TProperty;
            if (propertyValue == null)
                throw new ArgumentException("The property is not initialized: " + property.Name, nameof(propertyExpression));

            var stateProperty = statePropertyExpression.GetPropertyInfo();

            var name = $"{property.Name}.{stateProperty.Name}";

            var existingState = GetStateProperty(stateProperty, propertyValue);
            if (name.Equals(existingState?.Name))
                return;

            var state = new StateMachineState<TInstance>(this, name, _eventObservers);

            ConfigurationHelpers.InitializeStateProperty(stateProperty, propertyValue, state);

            SetState(name, state);
        }

        static StateMachineState<TInstance> GetStateProperty<TProperty>(PropertyInfo stateProperty, TProperty propertyValue)
            where TProperty : class
        {
            if (stateProperty.CanRead)
                return stateProperty.GetValue(propertyValue) as StateMachineState<TInstance>;

            var objectProperty = propertyValue.GetType().GetProperty(stateProperty.Name, typeof(State));
            if (objectProperty == null || !objectProperty.CanRead)
                throw new ArgumentException($"The state property is not readable: {stateProperty.Name}");

            return objectProperty.GetValue(propertyValue) as StateMachineState<TInstance>;
        }

        /// <summary>
        /// Declares a sub-state on the machine. A sub-state is a state that is valid within a super-state,
        /// allowing a state machine to have multiple "states" -- nested parts of an overall state.
        /// </summary>
        /// <param name="propertyExpression">The state property expression</param>
        /// <param name="superState">The superstate of which this state is a substate</param>
        protected internal virtual void SubState(Expression<Func<State>> propertyExpression, State superState)
        {
            if (superState == null)
                throw new ArgumentNullException(nameof(superState));

            var superStateInstance = GetState(superState.Name);

            var property = propertyExpression.GetPropertyInfo();

            var name = property.Name;

            var propertyValue = property.GetValue(this);

            // If the state was already defined, don't define it again
            var existingState = propertyValue as StateMachineState<TInstance>;
            if (name.Equals(existingState?.Name) && superState.Name.Equals(existingState?.SuperState?.Name))
                return;

            var state = new StateMachineState<TInstance>(this, name, _eventObservers, superStateInstance);

            ConfigurationHelpers.InitializeState(this, property, state);

            SetState(name, state);
        }

        protected internal virtual State<TInstance> SubState(string name, State superState)
        {
            if (superState == null)
                throw new ArgumentNullException(nameof(superState));

            var superStateInstance = GetState(superState.Name);

            // If the state was already defined, don't define it again
            if (TryGetState(name, out var existingState) &&
                name.Equals(existingState?.Name) &&
                superState.Name.Equals(existingState?.SuperState?.Name))
                return existingState;

            var state = new StateMachineState<TInstance>(this, name, _eventObservers, superStateInstance);

            SetState(name, state);
            return state;
        }

        /// <summary>
        /// Declares a state on the state machine, and initialized the property
        /// </summary>
        /// <param name="propertyExpression">The property containing the state</param>
        /// <param name="statePropertyExpression">The state property</param>
        /// <param name="superState">The superstate of which this state is a substate</param>
        protected internal virtual void SubState<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression, State superState)
            where TProperty : class
        {
            if (superState == null)
                throw new ArgumentNullException(nameof(superState));

            var superStateInstance = GetState(superState.Name);

            var property = propertyExpression.GetPropertyInfo();
            var propertyValue = property.GetValue(this, null) as TProperty;
            if (propertyValue == null)
                throw new ArgumentException("The property is not initialized: " + property.Name, nameof(propertyExpression));

            var stateProperty = statePropertyExpression.GetPropertyInfo();

            var name = $"{property.Name}.{stateProperty.Name}";

            var existingState = GetStateProperty(stateProperty, propertyValue);
            if (name.Equals(existingState?.Name) && superState.Name.Equals(existingState?.SuperState?.Name))
                return;

            var state = new StateMachineState<TInstance>(this, name, _eventObservers, superStateInstance);

            ConfigurationHelpers.InitializeStateProperty(stateProperty, propertyValue, state);

            SetState(name, state);
        }

        /// <summary>
        /// Adds the state, and state transition events, to the cache
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        void SetState(string name, StateMachineState<TInstance> state)
        {
            _stateCache[name] = state;

            _eventCache[state.BeforeEnter.Name] = new StateMachineEvent<TInstance>(state.BeforeEnter, true);
            _eventCache[state.Enter.Name] = new StateMachineEvent<TInstance>(state.Enter, true);
            _eventCache[state.Leave.Name] = new StateMachineEvent<TInstance>(state.Leave, true);
            _eventCache[state.AfterLeave.Name] = new StateMachineEvent<TInstance>(state.AfterLeave, true);
        }

        /// <summary>
        /// Declares the events and associated activities that are handled during the specified state
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="activities">The event and activities</param>
        protected internal void During(State state, params EventActivities<TInstance>[] activities)
        {
            var activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            BindActivitiesToState(state, activitiesBinder);
        }

        /// <summary>
        /// Declares the events and associated activities that are handled during the specified states
        /// </summary>
        /// <param name="state1">The state</param>
        /// <param name="state2">The other state</param>
        /// <param name="activities">The event and activities</param>
        protected internal void During(State state1, State state2, params EventActivities<TInstance>[] activities)
        {
            var activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            BindActivitiesToState(state1, activitiesBinder);
            BindActivitiesToState(state2, activitiesBinder);
        }

        /// <summary>
        /// Declares the events and associated activities that are handled during the specified states
        /// </summary>
        /// <param name="state1">The state</param>
        /// <param name="state2">The other state</param>
        /// <param name="state3">The other other state</param>
        /// <param name="activities">The event and activities</param>
        protected internal void During(State state1, State state2, State state3, params EventActivities<TInstance>[] activities)
        {
            var activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            BindActivitiesToState(state1, activitiesBinder);
            BindActivitiesToState(state2, activitiesBinder);
            BindActivitiesToState(state3, activitiesBinder);
        }

        /// <summary>
        /// Declares the events and associated activities that are handled during the specified states
        /// </summary>
        /// <param name="state1">The state</param>
        /// <param name="state2">The other state</param>
        /// <param name="state3">The other other state</param>
        /// <param name="state4">Okay, this is getting a bit ridiculous at this point</param>
        /// <param name="activities">The event and activities</param>
        protected internal void During(State state1, State state2, State state3, State state4,
            params EventActivities<TInstance>[] activities)
        {
            var activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            BindActivitiesToState(state1, activitiesBinder);
            BindActivitiesToState(state2, activitiesBinder);
            BindActivitiesToState(state3, activitiesBinder);
            BindActivitiesToState(state4, activitiesBinder);
        }

        /// <summary>
        /// Declares the events and associated activities that are handled during the specified states
        /// </summary>
        /// <param name="states">The states</param>
        /// <param name="activities">The event and activities</param>
        protected internal void During(IEnumerable<State> states, params EventActivities<TInstance>[] activities)
        {
            var activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            foreach (var state in states)
                BindActivitiesToState(state, activitiesBinder);
        }

        void BindActivitiesToState(State state, IEnumerable<ActivityBinder<TInstance>> eventActivities)
        {
            var activityState = GetState(state.Name);

            foreach (var activity in eventActivities)
                activity.Bind(activityState);
        }

        /// <summary>
        /// Declares the events and activities that are handled during the initial state
        /// </summary>
        /// <param name="activities">The event and activities</param>
        protected internal void Initially(params EventActivities<TInstance>[] activities)
        {
            During(Initial, activities);
        }

        /// <summary>
        /// Declares events and activities that are handled during any state exception Initial and Final
        /// </summary>
        /// <param name="activities">The event and activities</param>
        protected internal void DuringAny(params EventActivities<TInstance>[] activities)
        {
            var states = _stateCache.Values.Where(x => !Equals(x, Initial) && !Equals(x, Final));

            // we only add DuringAny event handlers to non-initial and non-final states to avoid
            // reviving finalized state machine instances or creating new ones accidentally.
            foreach (var state in states)
                During(state, activities);

            BindTransitionEvents(_initial, activities);
            BindTransitionEvents(_final, activities);
        }

        /// <summary>
        /// When the Final state is entered, execute the chained activities. This occurs in any state that is not the initial or final state
        /// </summary>
        /// <param name="activityCallback">Specify the activities that are executes when the Final state is entered.</param>
        protected internal void Finally(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            var binder = When(Final.Enter);

            binder = activityCallback(binder);

            DuringAny(binder);
        }

        void BindTransitionEvents(State<TInstance> state, IEnumerable<EventActivities<TInstance>> activities)
        {
            var eventActivities = activities
                .SelectMany(activity => activity.GetStateActivityBinders().Where(x => x.IsStateTransitionEvent(state)));

            foreach (var eventActivity in eventActivities)
                eventActivity.Bind(state);
        }

        /// <summary>
        /// When the event is fired in this state, execute the chained activities
        /// </summary>
        /// <param name="event">The fired event</param>
        /// <returns></returns>
        protected internal EventActivityBinder<TInstance> When(Event @event)
        {
            return new TriggerEventActivityBinder<TInstance>(this, @event);
        }

        /// <summary>
        /// When the event is fired in this state, and the event data matches the filter expression, execute the chained activities
        /// </summary>
        /// <param name="event">The fired event</param>
        /// <param name="filter">The filter applied to the event</param>
        /// <returns></returns>
        protected internal EventActivityBinder<TInstance> When(Event @event, StateMachineEventFilter<TInstance> filter)
        {
            return new TriggerEventActivityBinder<TInstance>(this, @event, filter);
        }


        /// <summary>
        /// When entering the specified state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void WhenEnter(State state, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            var activityState = GetState(state.Name);

            EventActivityBinder<TInstance> binder = new TriggerEventActivityBinder<TInstance>(this, activityState.Enter);

            binder = activityCallback(binder);

            During(state, binder);
        }

        /// <summary>
        /// When entering any state
        /// </summary>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void WhenEnterAny(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            BindEveryTransitionEvent(activityCallback, x => x.Enter);
        }

        /// <summary>
        /// When leaving any state
        /// </summary>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void WhenLeaveAny(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            BindEveryTransitionEvent(activityCallback, x => x.Leave);
        }

        void BindEveryTransitionEvent(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback,
            Func<State<TInstance>, Event> eventProvider)
        {
            var states = _stateCache.Values.ToArray();

            var binders = states.Select(state =>
            {
                EventActivityBinder<TInstance> binder = new TriggerEventActivityBinder<TInstance>(this, eventProvider(state));

                return activityCallback(binder);
            }).SelectMany(x => x.GetStateActivityBinders()).ToArray();

            foreach (var state in states)
                foreach (var binder in binders)
                    binder.Bind(state);
        }

        /// <summary>
        /// Before entering any state
        /// </summary>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void BeforeEnterAny(
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            BindEveryTransitionEvent(activityCallback, x => x.BeforeEnter);
        }

        /// <summary>
        /// After leaving any state
        /// </summary>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void AfterLeaveAny(
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            BindEveryTransitionEvent(activityCallback, x => x.AfterLeave);
        }

        void BindEveryTransitionEvent(Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback,
            Func<State<TInstance>, Event<State>> eventProvider)
        {
            var states = _stateCache.Values.ToArray();

            var binders = states.Select(state =>
            {
                EventActivityBinder<TInstance, State> binder = new DataEventActivityBinder<TInstance, State>(this, eventProvider(state));

                return activityCallback(binder);
            }).SelectMany(x => x.GetStateActivityBinders()).ToArray();

            foreach (var state in states)
                foreach (var binder in binders)
                    binder.Bind(state);
        }

        /// <summary>
        /// When leaving the specified state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void WhenLeave(State state, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            var activityState = GetState(state.Name);

            EventActivityBinder<TInstance> binder = new TriggerEventActivityBinder<TInstance>(this, activityState.Leave);

            binder = activityCallback(binder);

            During(state, binder);
        }

        /// <summary>
        /// Before entering the specified state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void BeforeEnter(State state,
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            var activityState = GetState(state.Name);

            EventActivityBinder<TInstance, State> binder = new DataEventActivityBinder<TInstance, State>(this, activityState.BeforeEnter);

            binder = activityCallback(binder);

            During(state, binder);
        }

        /// <summary>
        /// After leaving the specified state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void AfterLeave(State state,
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            var activityState = GetState(state.Name);

            EventActivityBinder<TInstance, State> binder = new DataEventActivityBinder<TInstance, State>(this, activityState.AfterLeave);

            binder = activityCallback(binder);

            During(state, binder);
        }

        /// <summary>
        /// When the event is fired in this state, execute the chained activities
        /// </summary>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="event">The fired event</param>
        /// <returns></returns>
        protected internal EventActivityBinder<TInstance, TData> When<TData>(Event<TData> @event)
        {
            return new DataEventActivityBinder<TInstance, TData>(this, @event);
        }

        /// <summary>
        /// When the event is fired in this state, and the event data matches the filter expression, execute the chained activities
        /// </summary>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="event">The fired event</param>
        /// <param name="filter">The filter applied to the event</param>
        /// <returns></returns>
        protected internal EventActivityBinder<TInstance, TData> When<TData>(Event<TData> @event,
            StateMachineEventFilter<TInstance, TData> filter)
        {
            return new DataEventActivityBinder<TInstance, TData>(this, @event, filter);
        }

        /// <summary>
        /// Ignore the event in this state (no exception is thrown)
        /// </summary>
        /// <param name="event">The ignored event</param>
        /// <returns></returns>
        protected internal EventActivities<TInstance> Ignore(Event @event)
        {
            ActivityBinder<TInstance> activityBinder = new IgnoreEventActivityBinder<TInstance>(@event);

            return new TriggerEventActivityBinder<TInstance>(this, @event, activityBinder);
        }

        /// <summary>
        /// Ignore the event in this state (no exception is thrown)
        /// </summary>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="event">The ignored event</param>
        /// <returns></returns>
        protected internal EventActivities<TInstance> Ignore<TData>(Event<TData> @event)
        {
            ActivityBinder<TInstance> activityBinder = new IgnoreEventActivityBinder<TInstance>(@event);

            return new DataEventActivityBinder<TInstance, TData>(this, @event, activityBinder);
        }

        /// <summary>
        /// Ignore the event in this state (no exception is thrown)
        /// </summary>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="event">The ignored event</param>
        /// <param name="filter">The filter to apply to the event data</param>
        /// <returns></returns>
        protected internal EventActivities<TInstance> Ignore<TData>(Event<TData> @event,
            StateMachineEventFilter<TInstance, TData> filter)
        {
            ActivityBinder<TInstance> activityBinder = new IgnoreEventActivityBinder<TInstance, TData>(@event, filter);

            return new DataEventActivityBinder<TInstance, TData>(this, @event, activityBinder);
        }

        /// <summary>
        /// Specifies a callback to invoke when an event is raised in a state where the event is not handled
        /// </summary>
        /// <param name="callback">The unhandled event callback</param>
        protected internal void OnUnhandledEvent(UnhandledEventCallback<TInstance> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _unhandledEventCallback = callback;
        }

        internal Task UnhandledEvent(EventContext<TInstance> context, State state)
        {
            var unhandledEventContext = new StateUnhandledEventContext<TInstance>(context, state, this);

            return _unhandledEventCallback(unhandledEventContext);
        }

        /// <summary>
        /// Register all remaining events and states that have not been explicitly declared.
        /// </summary>
        void RegisterImplicit()
        {
            foreach (var declaration in _registrations.Value)
                declaration.Declare(this);
        }

        StateMachine<TInstance> Modify(Action<StateMachineModifier<TInstance>> modifier)
        {
            StateMachineModifier<TInstance> builder = new InternalStateMachineModifier<TInstance>(this);
            modifier(builder);
            builder.Apply();

            return this;
        }

        /// <summary>
        /// Create a new state machine using the builder pattern
        /// </summary>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public static AutomatonymousStateMachine<TInstance> New(Action<StateMachineModifier<TInstance>> modifier)
        {
            var machine = new BuilderStateMachine();
            machine.Modify(modifier);
            return machine;
        }


        protected static class ConfigurationHelpers
        {
            public static StateMachineRegistration[] GetRegistrations(AutomatonymousStateMachine<TInstance> stateMachine)
            {
                var events = new List<StateMachineRegistration>();

                var machineType = stateMachine.GetType().GetTypeInfo();

                var properties = GetStateMachineProperties(machineType);

                foreach (var propertyInfo in properties)
                    if (propertyInfo.PropertyType.GetTypeInfo().IsGenericType)
                    {
                        if (propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Event<>))
                        {
                            var declarationType = typeof(DataEventRegistration<,>).MakeGenericType(typeof(TInstance), machineType,
                                propertyInfo.PropertyType.GetGenericArguments().First());
                            var declaration = Activator.CreateInstance(declarationType, propertyInfo);
                            events.Add((StateMachineRegistration)declaration);
                        }
                    }
                    else
                    {
                        if (propertyInfo.PropertyType == typeof(Event))
                        {
                            var declarationType = typeof(TriggerEventRegistration<>).MakeGenericType(typeof(TInstance), machineType);
                            var declaration = Activator.CreateInstance(declarationType, propertyInfo);
                            events.Add((StateMachineRegistration)declaration);
                        }
                        else if (propertyInfo.PropertyType == typeof(State))
                        {
                            var declarationType = typeof(StateRegistration<>).MakeGenericType(typeof(TInstance), machineType);
                            var declaration = Activator.CreateInstance(declarationType, propertyInfo);
                            events.Add((StateMachineRegistration)declaration);
                        }
                    }

                return events.ToArray();
            }

            public static IEnumerable<PropertyInfo> GetStateMachineProperties(TypeInfo typeInfo)
            {
                if (typeInfo.IsInterface)
                    yield break;

                if (typeInfo.BaseType != null)
                    foreach (var propertyInfo in GetStateMachineProperties(typeInfo.BaseType.GetTypeInfo()))
                        yield return propertyInfo;

                var properties = typeInfo.DeclaredMethods
                    .Where(x => x.IsSpecialName && x.Name.StartsWith("get_") && !x.IsStatic)
                    .Select(x => typeInfo.GetDeclaredProperty(x.Name.Substring("get_".Length)))
                    .Where(x => x.CanRead && (x.CanWrite || TryGetBackingField(typeInfo, x, out _)));

                foreach (var propertyInfo in properties)
                    yield return propertyInfo;
            }

            public static bool TryGetBackingField(TypeInfo typeInfo, PropertyInfo property, out FieldInfo backingField)
            {
                backingField = typeInfo
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .FirstOrDefault(field =>
                        field.Attributes.HasFlag(FieldAttributes.Private) &&
                        field.Attributes.HasFlag(FieldAttributes.InitOnly) &&
                        field.CustomAttributes.Any(attr => attr.AttributeType == typeof(CompilerGeneratedAttribute)) &&
                        field.DeclaringType == property.DeclaringType &&
                        field.FieldType.IsAssignableFrom(property.PropertyType) &&
                        field.Name.StartsWith("<" + property.Name + ">")
                    );

                return backingField != null;
            }

            public static void InitializeState(AutomatonymousStateMachine<TInstance> stateMachine, PropertyInfo property,
                StateMachineState<TInstance> state)
            {
                if (property.CanWrite)
                    property.SetValue(stateMachine, state);
                else if (TryGetBackingField(stateMachine.GetType().GetTypeInfo(), property, out var backingField))
                    backingField.SetValue(stateMachine, state);
                else
                    throw new ArgumentException($"The state property is not writable: {property.Name}");
            }

            public static void InitializeStateProperty<TProperty>(PropertyInfo stateProperty, TProperty propertyValue,
                StateMachineState<TInstance> state)
                where TProperty : class
            {
                if (stateProperty.CanWrite)
                {
                    stateProperty.SetValue(propertyValue, state);
                }
                else
                {
                    var objectProperty = propertyValue.GetType().GetProperty(stateProperty.Name, typeof(State));
                    if (objectProperty == null || !objectProperty.CanWrite)
                        throw new ArgumentException($"The state property is not writable: {stateProperty.Name}");

                    objectProperty.SetValue(propertyValue, state);
                }
            }

            public static void InitializeEvent(AutomatonymousStateMachine<TInstance> stateMachine, PropertyInfo property, Event @event)
            {
                if (property.CanWrite)
                    property.SetValue(stateMachine, @event);
                else if (TryGetBackingField(stateMachine.GetType().GetTypeInfo(), property, out var backingField))
                    backingField.SetValue(stateMachine, @event);
                else
                    throw new ArgumentException($"The event property is not writable: {property.Name}");
            }

            public static void InitializeEventProperty<TProperty, T>(PropertyInfo eventProperty, TProperty propertyValue, Event @event)
                where TProperty : class
            {
                if (eventProperty.CanWrite)
                {
                    eventProperty.SetValue(propertyValue, @event);
                }
                else
                {
                    var objectProperty = propertyValue.GetType().GetProperty(eventProperty.Name, typeof(Event<T>));
                    if (objectProperty == null || !objectProperty.CanWrite)
                        throw new ArgumentException($"The event property is not writable: {eventProperty.Name}");

                    objectProperty.SetValue(propertyValue, @event);
                }
            }


            public interface StateMachineRegistration
            {
                void Declare(object stateMachine);
            }


            public class StateRegistration<TStateMachine> :
                StateMachineRegistration
                where TStateMachine : AutomatonymousStateMachine<TInstance>
            {
                readonly PropertyInfo _propertyInfo;

                public StateRegistration(PropertyInfo propertyInfo)
                {
                    _propertyInfo = propertyInfo;
                }

                public void Declare(object stateMachine)
                {
                    var machine = (TStateMachine)stateMachine;
                    var existing = _propertyInfo.GetValue(machine);
                    if (existing != null)
                        return;

                    machine.DeclareState(_propertyInfo);
                }
            }


            public class TriggerEventRegistration<TStateMachine> :
                StateMachineRegistration
                where TStateMachine : AutomatonymousStateMachine<TInstance>
            {
                readonly PropertyInfo _propertyInfo;

                public TriggerEventRegistration(PropertyInfo propertyInfo)
                {
                    _propertyInfo = propertyInfo;
                }

                public void Declare(object stateMachine)
                {
                    var machine = (TStateMachine)stateMachine;
                    var existing = _propertyInfo.GetValue(machine);
                    if (existing != null)
                        return;

                    machine.DeclarePropertyBasedEvent(prop => machine.DeclareTriggerEvent(prop.Name), _propertyInfo);
                }
            }


            public class DataEventRegistration<TStateMachine, TData> :
                StateMachineRegistration
                where TStateMachine : AutomatonymousStateMachine<TInstance>
            {
                readonly PropertyInfo _propertyInfo;

                public DataEventRegistration(PropertyInfo propertyInfo)
                {
                    _propertyInfo = propertyInfo;
                }

                public void Declare(object stateMachine)
                {
                    var machine = (TStateMachine)stateMachine;
                    var existing = _propertyInfo.GetValue(machine);
                    if (existing != null)
                        return;

                    machine.DeclarePropertyBasedEvent(prop => machine.DeclareDataEvent<TData>(prop.Name), _propertyInfo);
                }
            }
        }


        class BuilderStateMachine : AutomatonymousStateMachine<TInstance>
        {
        }
    }
}
