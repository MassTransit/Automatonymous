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
    using Binders;
    using Builder;
    using Contexts;
    using Events;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Observers;
    using States;


    public abstract partial class AutomatonymousStateMachine<TInstance> : StateMachine<TInstance>
        where TInstance : class
    {
        private const string InitialName = "Initial";
        private const string FinalName = "Final";

        private readonly Dictionary<string, StateMachineEvent> _eventCache;
        private readonly EventObservable<TInstance> _eventObservers;
        private readonly Lazy<ConfigurationHelpers.StateMachineRegistration[]> _registrations;
        private readonly Dictionary<string, State<TInstance>> _stateCache;
        private readonly StateObservable<TInstance> _stateObservers;
        private StateAccessor<TInstance> _accessor;
        private string _name;
        private UnhandledEventCallback<TInstance> _unhandledEventCallback;

        protected AutomatonymousStateMachine()
        {
            _registrations = new Lazy<ConfigurationHelpers.StateMachineRegistration[]>(() => ConfigurationHelpers.GetRegistrations(this));
            _stateCache = new Dictionary<string, State<TInstance>>();
            _eventCache = new Dictionary<string, StateMachineEvent>();

            _eventObservers = new EventObservable<TInstance>();
            _stateObservers = new StateObservable<TInstance>();

            _stateCache[InitialName] = new StateMachineState<TInstance>(this, InitialName, _eventObservers);
            _stateCache[FinalName] = new StateMachineState<TInstance>(this, FinalName, _eventObservers);

            _accessor = new DefaultInstanceStateAccessor<TInstance>(this, _stateCache[Initial.Name], _stateObservers);

            _unhandledEventCallback = DefaultUnhandledEventCallback;

            _name = GetType().Name;

            RegisterImplicit();
        }

        private IEnumerable<State<TInstance>> IntrospectionStates
        {
            get
            {
                yield return _stateCache[InitialName];

                foreach (var x in _stateCache.Values.Where(x => !Equals(x, Initial) && !Equals(x, Final)))
                {
                    yield return x;
                }

                yield return _stateCache[FinalName];
            }
        }

        string StateMachine.Name => _name;
        StateAccessor<TInstance> StateMachine<TInstance>.Accessor => _accessor;
        public State Initial => _stateCache[InitialName];
        public State Final => _stateCache[FinalName];

        State StateMachine.GetState(string name) =>
            _stateCache.TryGetValue(name, out var result) ? result : throw new UnknownStateException(_name, name);

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

        public State<TInstance> GetState(string name) =>
            TryGetState(name, out var result) ? result : throw new UnknownStateException(_name, name);

        public IEnumerable<State> States => _stateCache.Values;

        Event StateMachine.GetEvent(string name) =>
            _eventCache.TryGetValue(name, out var result) ? result.Event : throw new UnknownEventException(_name, name);

        public IEnumerable<Event> Events =>
            _eventCache.Values.Where(x => false == x.IsTransitionEvent).Select(x => x.Event);

        Type StateMachine.InstanceType => typeof(TInstance);

        IEnumerable<Event> StateMachine.NextEvents(State state) =>
            _stateCache.TryGetValue(state.Name, out var result) ? result.Events : throw new UnknownStateException(_name, state.Name);

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            foreach (var x in IntrospectionStates)
                x.Accept(visitor);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("stateMachine");
            scope.Add("name", GetType().Name);
            scope.Add("instanceType", TypeCache<TInstance>.ShortName);

            _accessor.Probe(scope);

            foreach (var state in IntrospectionStates)
                state.Probe(scope);
        }

        IDisposable StateMachine<TInstance>.ConnectEventObserver(EventObserver<TInstance> observer) =>
            _eventObservers.Connect(new NonTransitionEventObserver<TInstance>(_eventCache, observer));

        IDisposable StateMachine<TInstance>.ConnectEventObserver(Event @event, EventObserver<TInstance> observer) =>
            _eventObservers.Connect(new SelectedEventObserver<TInstance>(@event, observer));

        IDisposable StateMachine<TInstance>.ConnectStateObserver(StateObserver<TInstance> observer) =>
            _stateObservers.Connect(observer);

        public bool TryGetState(string name, out State<TInstance> state) =>
            _stateCache.TryGetValue(name, out state);

        private Task DefaultUnhandledEventCallback(UnhandledEventContext<TInstance> context) =>
            throw new UnhandledEventException(_name, context.Event.Name, context.CurrentState.Name);

        /// <summary>
        /// Declares what property holds the TInstance's state on the current instance of the state machine
        /// </summary>
        /// <param name="instanceStateProperty"></param>
        /// <remarks>Setting the state accessor more than once will cause the property managed by the state machine to change each time.
        /// Please note, the state machine can only manage one property at a given time per instance,
        /// and the best practice is to manage one property per machine.
        /// </remarks>
        protected internal void InstanceState(Expression<Func<TInstance, State>> instanceStateProperty) =>
            _accessor = new InitialIfNullStateAccessor<TInstance>(_stateCache[Initial.Name], new RawStateAccessor<TInstance>(this, instanceStateProperty, _stateObservers));

        /// <summary>
        /// Declares the property to hold the instance's state as a string (the state name is stored in the property)
        /// </summary>
        /// <param name="instanceStateProperty"></param>
        protected internal void InstanceState(Expression<Func<TInstance, string>> instanceStateProperty) =>
            _accessor = new InitialIfNullStateAccessor<TInstance>(_stateCache[Initial.Name], new StringStateAccessor<TInstance>(this, instanceStateProperty, _stateObservers));

        /// <summary>
        /// Declares the property to hold the instance's state as an int (0 - none, 1 = initial, 2 = final, 3... the rest)
        /// </summary>
        /// <param name="instanceStateProperty"></param>
        /// <param name="states">Specifies the states, in order, to which the int values should be assigned</param>
        protected internal void InstanceState(Expression<Func<TInstance, int>> instanceStateProperty, params State[] states) =>
            _accessor = new InitialIfNullStateAccessor<TInstance>(_stateCache[Initial.Name],
                new IntStateAccessor<TInstance>(instanceStateProperty, new StateAccessorIndex<TInstance>(this, _stateCache[InitialName], _stateCache[FinalName], states), _stateObservers));

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
        protected internal virtual void Event(Expression<Func<Event>> propertyExpression) =>
            DeclarePropertyBasedEvent(prop => DeclareTriggerEvent(prop.Name), propertyExpression.GetPropertyInfo());

        protected internal virtual Event Event(string name) =>
            DeclareTriggerEvent(name);

        private Event DeclareTriggerEvent(string name) =>
            DeclareEvent(_ => new TriggerEvent(name), name);

        /// <summary>
        /// Declares a data event on the state machine, and initializes the property
        /// </summary>
        /// <param name="propertyExpression">The event property</param>
        protected internal virtual void Event<T>(Expression<Func<Event<T>>> propertyExpression) =>
            DeclarePropertyBasedEvent(prop => DeclareDataEvent<T>(prop.Name), propertyExpression.GetPropertyInfo());

        protected internal virtual Event<T> Event<T>(string name) =>
            DeclareDataEvent<T>(name);

        private Event<T> DeclareDataEvent<T>(string name) =>
            DeclareEvent(_ => new DataEvent<T>(name), name);

        private void DeclarePropertyBasedEvent<TEvent>(Func<PropertyInfo, TEvent> ctor, PropertyInfo property)
            where TEvent : Event =>
                ConfigurationHelpers.InitializeEvent(this, property, ctor(property));

        private TEvent DeclareEvent<TEvent>(Func<string, TEvent> ctor, string name)
            where TEvent : Event
        {
            var @event = ctor(name);
            _eventCache[name] = new StateMachineEvent(@event, false);
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
            if (!(property.GetValue(this, null) is TProperty propertyValue))
                throw new ArgumentException("The property is not initialized: " + property.Name, nameof(propertyExpression));

            var eventProperty = eventPropertyExpression.GetPropertyInfo();
            var name = $"{property.Name}.{eventProperty.Name}";
            var @event = new DataEvent<T>(name);
            ConfigurationHelpers.InitializeEventProperty<TProperty, T>(eventProperty, propertyValue, @event);
            _eventCache[name] = new StateMachineEvent(@event, false);
        }

        /// <summary>
        /// Declares a state on the state machine, and initialized the property
        /// </summary>
        /// <param name="propertyExpression">The state property</param>
        protected internal virtual void State(Expression<Func<State>> propertyExpression) =>
            DeclareState(propertyExpression.GetPropertyInfo());

        protected internal virtual State<TInstance> State(string name) =>
            TryGetState(name, out var foundState) ? foundState : SetState(name, new StateMachineState<TInstance>(this, name, _eventObservers));

        private void DeclareState(PropertyInfo property)
        {
            // If the state was already defined, don't define it again
            if (property.Name.Equals((property.GetValue(this) as StateMachineState<TInstance>)?.Name))
                return;

            var state = new StateMachineState<TInstance>(this, property.Name, _eventObservers);
            ConfigurationHelpers.InitializeState(this, property, state);
            SetState(property.Name, state);
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
            if (!(property.GetValue(this, null) is TProperty propertyValue))
                throw new ArgumentException("The property is not initialized: " + property.Name, nameof(propertyExpression));

            var stateProperty = statePropertyExpression.GetPropertyInfo();

            var name = $"{property.Name}.{stateProperty.Name}";
            if (name.Equals(GetStateProperty(stateProperty, propertyValue)?.Name))
                return;

            var state = new StateMachineState<TInstance>(this, name, _eventObservers);
            ConfigurationHelpers.InitializeStateProperty(stateProperty, propertyValue, state);
            SetState(name, state);
        }

        private static StateMachineState<TInstance> GetStateProperty<TProperty>(PropertyInfo stateProperty, TProperty propertyValue)
            where TProperty : class
        {
            if (stateProperty.CanRead)
                return stateProperty.GetValue(propertyValue) as StateMachineState<TInstance>;

            var objectProperty = propertyValue.GetType().GetProperty(stateProperty.Name, typeof(State));
            return objectProperty == null || !objectProperty.CanRead
                ? throw new ArgumentException($"The state property is not readable: {stateProperty.Name}")
                : objectProperty.GetValue(propertyValue) as StateMachineState<TInstance>;
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

            var property = propertyExpression.GetPropertyInfo();

            // If the state was already defined, don't define it again
            var existingState = property.GetValue(this) as StateMachineState<TInstance>;
            if (property.Name.Equals(existingState?.Name) && superState.Name.Equals(existingState?.SuperState?.Name))
                return;

            var state = new StateMachineState<TInstance>(this, property.Name, _eventObservers, GetState(superState.Name));
            ConfigurationHelpers.InitializeState(this, property, state);
            SetState(property.Name, state);
        }

        protected internal virtual State<TInstance> SubState(string name, State superState) => superState == null
            ? throw new ArgumentNullException(nameof(superState))
            // If the state was already defined, don't define it again
            : TryGetState(name, out var existingState) && name.Equals(existingState?.Name) && superState.Name.Equals(existingState?.SuperState?.Name)
                ? existingState
                : SetState(name, new StateMachineState<TInstance>(this, name, _eventObservers, GetState(superState.Name)));

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

            var property = propertyExpression.GetPropertyInfo();
            if (!(property.GetValue(this, null) is TProperty propertyValue))
                throw new ArgumentException("The property is not initialized: " + property.Name, nameof(propertyExpression));

            var stateProperty = statePropertyExpression.GetPropertyInfo();
            var name = $"{property.Name}.{stateProperty.Name}";
            var existingState = GetStateProperty(stateProperty, propertyValue);
            if (name.Equals(existingState?.Name) && superState.Name.Equals(existingState?.SuperState?.Name))
                return;

            var state = new StateMachineState<TInstance>(this, name, _eventObservers, GetState(superState.Name));
            ConfigurationHelpers.InitializeStateProperty(stateProperty, propertyValue, state);
            SetState(name, state);
        }

        /// <summary>
        /// Adds the state, and state transition events, to the cache
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        private StateMachineState<TInstance> SetState(string name, StateMachineState<TInstance> state)
        {
            _stateCache[name] = state;
            _eventCache[state.BeforeEnter.Name] = new StateMachineEvent(state.BeforeEnter, true);
            _eventCache[state.Enter.Name] = new StateMachineEvent(state.Enter, true);
            _eventCache[state.Leave.Name] = new StateMachineEvent(state.Leave, true);
            _eventCache[state.AfterLeave.Name] = new StateMachineEvent(state.AfterLeave, true);
            return state;
        }

        /// <summary>
        /// Declares the events and associated activities that are handled during the specified state
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="activities">The event and activities</param>
        protected internal void During(State state, params EventActivities<TInstance>[] activities) =>
            BindActivitiesToState(state, activities.SelectMany(x => x.GetStateActivityBinders()).ToArray());

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

        private void BindActivitiesToState(State state, IEnumerable<ActivityBinder<TInstance>> eventActivities)
        {
            var activityState = GetState(state.Name);

            foreach (var activity in eventActivities)
                activity.Bind(activityState);
        }

        /// <summary>
        /// Declares the events and activities that are handled during the initial state
        /// </summary>
        /// <param name="activities">The event and activities</param>
        protected internal void Initially(params EventActivities<TInstance>[] activities) =>
            During(Initial, activities);

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

            BindTransitionEvents(_stateCache[InitialName], activities);
            BindTransitionEvents(_stateCache[FinalName], activities);
        }

        /// <summary>
        /// When the Final state is entered, execute the chained activities. This occurs in any state that is not the initial or final state
        /// </summary>
        /// <param name="activityCallback">Specify the activities that are executes when the Final state is entered.</param>
        protected internal void Finally(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback) =>
            DuringAny(activityCallback(When(Final.Enter)));

        private void BindTransitionEvents(State<TInstance> state, IEnumerable<EventActivities<TInstance>> activities)
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
        protected internal EventActivityBinder<TInstance> When(Event @event) =>
            new TriggerEventActivityBinder<TInstance>(this, @event);

        /// <summary>
        /// When the event is fired in this state, and the event data matches the filter expression, execute the chained activities
        /// </summary>
        /// <param name="event">The fired event</param>
        /// <param name="filter">The filter applied to the event</param>
        /// <returns></returns>
        protected internal EventActivityBinder<TInstance> When(Event @event, StateMachineEventFilter<TInstance> filter) =>
            new TriggerEventActivityBinder<TInstance>(this, @event, filter);

        /// <summary>
        /// When entering the specified state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void WhenEnter(State state, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback) =>
            During(state, activityCallback(new TriggerEventActivityBinder<TInstance>(this, GetState(state.Name).Enter)));

        /// <summary>
        /// When entering any state
        /// </summary>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void WhenEnterAny(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback) =>
            BindEveryTransitionEvent(activityCallback, x => x.Enter);

        /// <summary>
        /// When leaving any state
        /// </summary>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void WhenLeaveAny(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback) =>
            BindEveryTransitionEvent(activityCallback, x => x.Leave);

        /// <summary>
        /// Before entering any state
        /// </summary>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void BeforeEnterAny(Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback) =>
            BindEveryTransitionEvent(activityCallback, x => x.BeforeEnter);

        /// <summary>
        /// After leaving any state
        /// </summary>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void AfterLeaveAny(Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback) =>
            BindEveryTransitionEvent(activityCallback, x => x.AfterLeave);

        private void BindEveryTransitionEvent(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback, Func<State<TInstance>, Event> eventProvider) =>
            BindEveryTransitionEvent(state => activityCallback(new TriggerEventActivityBinder<TInstance>(this, eventProvider(state))));

        private void BindEveryTransitionEvent(Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback, Func<State<TInstance>, Event<State>> eventProvider) =>
            BindEveryTransitionEvent(state => activityCallback(new DataEventActivityBinder<TInstance, State>(this, eventProvider(state))));

        private void BindEveryTransitionEvent(Func<State<TInstance>, EventActivities<TInstance>> selectFunc)
        {
            var states = _stateCache.Values.ToArray();
            var binders = states
                .Select(selectFunc)
                .SelectMany(x => x.GetStateActivityBinders())
                .ToArray();

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
        protected internal void WhenLeave(State state, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback) =>
            During(state, activityCallback(new TriggerEventActivityBinder<TInstance>(this, GetState(state.Name).Leave)));

        /// <summary>
        /// Before entering the specified state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void BeforeEnter(State state, Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback) =>
            During(state, activityCallback(new DataEventActivityBinder<TInstance, State>(this, GetState(state.Name).BeforeEnter)));

        /// <summary>
        /// After leaving the specified state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected internal void AfterLeave(State state, Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback) =>
            During(state, activityCallback(new DataEventActivityBinder<TInstance, State>(this, GetState(state.Name).AfterLeave)));

        /// <summary>
        /// When the event is fired in this state, execute the chained activities
        /// </summary>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="event">The fired event</param>
        /// <returns></returns>
        protected internal EventActivityBinder<TInstance, TData> When<TData>(Event<TData> @event) =>
            new DataEventActivityBinder<TInstance, TData>(this, @event);

        /// <summary>
        /// When the event is fired in this state, and the event data matches the filter expression, execute the chained activities
        /// </summary>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="event">The fired event</param>
        /// <param name="filter">The filter applied to the event</param>
        /// <returns></returns>
        protected internal EventActivityBinder<TInstance, TData> When<TData>(Event<TData> @event, StateMachineEventFilter<TInstance, TData> filter) =>
            new DataEventActivityBinder<TInstance, TData>(this, @event, filter);

        /// <summary>
        /// Ignore the event in this state (no exception is thrown)
        /// </summary>
        /// <param name="event">The ignored event</param>
        /// <returns></returns>
        protected internal EventActivities<TInstance> Ignore(Event @event) =>
            new TriggerEventActivityBinder<TInstance>(this, @event, new IgnoreEventActivityBinder<TInstance>(@event));

        /// <summary>
        /// Ignore the event in this state (no exception is thrown)
        /// </summary>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="event">The ignored event</param>
        /// <returns></returns>
        protected internal EventActivities<TInstance> Ignore<TData>(Event<TData> @event) =>
            new DataEventActivityBinder<TInstance, TData>(this, @event, new IgnoreEventActivityBinder<TInstance>(@event));

        /// <summary>
        /// Ignore the event in this state (no exception is thrown)
        /// </summary>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="event">The ignored event</param>
        /// <param name="filter">The filter to apply to the event data</param>
        /// <returns></returns>
        protected internal EventActivities<TInstance> Ignore<TData>(Event<TData> @event, StateMachineEventFilter<TInstance, TData> filter) =>
            new DataEventActivityBinder<TInstance, TData>(this, @event, new IgnoreEventActivityBinder<TInstance, TData>(@event, filter));

        /// <summary>
        /// Specifies a callback to invoke when an event is raised in a state where the event is not handled
        /// </summary>
        /// <param name="callback">The unhandled event callback</param>
        protected internal void OnUnhandledEvent(UnhandledEventCallback<TInstance> callback) =>
            _unhandledEventCallback = callback ?? throw new ArgumentNullException(nameof(callback));

        internal Task UnhandledEvent(EventContext<TInstance> context, State state) =>
            _unhandledEventCallback(new StateUnhandledEventContext<TInstance>(context, state, this));

        /// <summary>
        /// Register all remaining events and states that have not been explicitly declared.
        /// </summary>
        private void RegisterImplicit()
        {
            foreach (var declaration in _registrations.Value)
                declaration.Declare(this);
        }

        private StateMachine<TInstance> Modify(Action<StateMachineModifier<TInstance>> modifier)
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

                foreach (var propertyInfo in GetStateMachineProperties(machineType))
                    if (propertyInfo.PropertyType.GetTypeInfo().IsGenericType)
                    {
                        if (propertyInfo.PropertyType.GetGenericTypeDefinition() != typeof(Event<>))
                            continue;
                        events.Add((StateMachineRegistration)Activator.CreateInstance(typeof(DataEventRegistration<,>).MakeGenericType(typeof(TInstance), machineType,
                            propertyInfo.PropertyType.GetGenericArguments().First()), propertyInfo));
                    }
                    else
                    {
                        var type = propertyInfo.PropertyType switch
                        {
                        {} when propertyInfo.PropertyType == typeof(Event) => typeof(TriggerEventRegistration<>),
                        {} when propertyInfo.PropertyType == typeof(State) => typeof(StateRegistration<>),
                            _ => null
                            };
                        if (type != null)
                            events.Add((StateMachineRegistration)Activator.CreateInstance(type.MakeGenericType(typeof(TInstance), machineType), propertyInfo));
                    }
                return events.ToArray();
            }

            private static IEnumerable<PropertyInfo> GetStateMachineProperties(TypeInfo typeInfo)
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

            private static bool TryGetBackingField(IReflect typeInfo, PropertyInfo property, out FieldInfo backingField) => (backingField = typeInfo
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .FirstOrDefault(field =>
                    field.Attributes.HasFlag(FieldAttributes.Private) &&
                        field.Attributes.HasFlag(FieldAttributes.InitOnly) &&
                        field.CustomAttributes.Any(attr => attr.AttributeType == typeof(CompilerGeneratedAttribute)) &&
                        field.DeclaringType == property.DeclaringType &&
                        field.FieldType.IsAssignableFrom(property.PropertyType) &&
                        field.Name.StartsWith("<" + property.Name + ">")
                )) != null;

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
                    stateProperty.SetValue(propertyValue, state);
                else
                {
                    var objectProperty = propertyValue.GetType().GetProperty(stateProperty.Name, typeof(State));
                    if (objectProperty == null || !objectProperty.CanWrite)
                        throw new ArgumentException($"The state property is not writable: {stateProperty.Name}");
                    objectProperty.SetValue(propertyValue, state);
                }
            }

            public static Event InitializeEvent(AutomatonymousStateMachine<TInstance> stateMachine, PropertyInfo property, Event @event)
            {
                if (property.CanWrite)
                    property.SetValue(stateMachine, @event);
                else if (TryGetBackingField(stateMachine.GetType().GetTypeInfo(), property, out var backingField))
                    backingField.SetValue(stateMachine, @event);
                else
                    throw new ArgumentException($"The event property is not writable: {property.Name}");
                return @event;
            }

            public static void InitializeEventProperty<TProperty, T>(PropertyInfo eventProperty, TProperty propertyValue, Event @event)
                where TProperty : class
            {
                if (eventProperty.CanWrite)
                    eventProperty.SetValue(propertyValue, @event);
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


            private abstract class AStateMachineRegistration<TStateMachine> : StateMachineRegistration
                where TStateMachine : AutomatonymousStateMachine<TInstance>
            {
                protected readonly PropertyInfo PropertyInfo;

                protected AStateMachineRegistration(PropertyInfo propertyInfo)
                {
                    PropertyInfo = propertyInfo;
                }

                public void Declare(object stateMachine)
                {
                    var machine = (TStateMachine)stateMachine;
                    if (PropertyInfo.GetValue(machine) != null)
                        return;
                    OnDeclare(machine);
                }

                protected abstract void OnDeclare(TStateMachine stateMachine);
            }


            private class StateRegistration<TStateMachine> : AStateMachineRegistration<TStateMachine>
                where TStateMachine : AutomatonymousStateMachine<TInstance>
            {
                public StateRegistration(PropertyInfo propertyInfo) : base(propertyInfo)
                {
                }

                protected override void OnDeclare(TStateMachine machine) =>
                    machine.DeclareState(PropertyInfo);
            }


            private class TriggerEventRegistration<TStateMachine> : AStateMachineRegistration<TStateMachine>
                where TStateMachine : AutomatonymousStateMachine<TInstance>
            {
                public TriggerEventRegistration(PropertyInfo propertyInfo)
                    : base(propertyInfo)
                {
                }

                protected override void OnDeclare(TStateMachine machine) =>
                    machine.DeclarePropertyBasedEvent(prop => machine.DeclareTriggerEvent(prop.Name), PropertyInfo);
            }


            private class DataEventRegistration<TStateMachine, TData> : AStateMachineRegistration<TStateMachine>
                where TStateMachine : AutomatonymousStateMachine<TInstance>
            {
                public DataEventRegistration(PropertyInfo propertyInfo)
                    : base(propertyInfo)
                {
                }

                protected override void OnDeclare(TStateMachine machine) =>
                    machine.DeclarePropertyBasedEvent(prop => machine.DeclareDataEvent<TData>(prop.Name), PropertyInfo);
            }
        }


        private class BuilderStateMachine : AutomatonymousStateMachine<TInstance>
        {
        }
    }
}
