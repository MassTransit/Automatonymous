// Copyright 2011-2015 Chris Patterson, Dru Sellers
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Automatonymous
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Accessors;
    using Activities;
    using Binders;
    using Contexts;
    using Events;
    using Internals;
    using Observers;
    using States;


    public abstract class AutomatonymousStateMachine<TInstance> :
        StateMachine<TInstance>
        where TInstance : class
    {
        readonly Dictionary<string, StateMachineEvent<TInstance>> _eventCache;
        readonly EventRaisedObserver<TInstance> _eventRaisedObserver;
        readonly EventRaisingObserver<TInstance> _eventRaisingObserver;
        readonly State<TInstance> _final;
        readonly State<TInstance> _initial;
        readonly Lazy<StateMachineRegistration[]> _registrations;
        readonly Dictionary<string, State<TInstance>> _stateCache;
        readonly Observable<StateChanged<TInstance>> _stateChangedObservable;
        StateAccessor<TInstance> _instanceStateAccessor;
        string _name;
        UnhandledEventCallback<TInstance> _unhandledEventCallback;

        protected AutomatonymousStateMachine()
        {
            _registrations = new Lazy<StateMachineRegistration[]>(GetRegistrations);
            _stateCache = new Dictionary<string, State<TInstance>>();
            _eventCache = new Dictionary<string, StateMachineEvent<TInstance>>();

            _stateChangedObservable = new Observable<StateChanged<TInstance>>();
            _eventRaisingObserver = new EventRaisingObserver<TInstance>(_eventCache);
            _eventRaisedObserver = new EventRaisedObserver<TInstance>(_eventCache);

            _initial = new StateMachineState<TInstance>(this, "Initial", _eventRaisingObserver, _eventRaisedObserver);
            _stateCache[_initial.Name] = _initial;
            _final = new StateMachineState<TInstance>(this, "Final", _eventRaisingObserver, _eventRaisedObserver);
            _stateCache[_final.Name] = _final;

            _instanceStateAccessor = new DefaultInstanceStateAccessor<TInstance>(this, _stateCache[Initial.Name], _stateChangedObservable);

            _unhandledEventCallback = DefaultUnhandledEventCallback;

            _name = GetType().Name;

            RegisterImplicit();
        }

        string StateMachine.Name
        {
            get { return _name; }
        }

        StateAccessor<TInstance> StateMachine<TInstance>.InstanceStateAccessor
        {
            get { return _instanceStateAccessor; }
        }

        public State Initial
        {
            get { return _initial; }
        }

        public State Final
        {
            get { return _final; }
        }

        State StateMachine.GetState(string name)
        {
            State<TInstance> result;
            if (_stateCache.TryGetValue(name, out result))
                return result;

            throw new UnknownStateException(_name, name);
        }

        async Task StateMachine<TInstance>.RaiseEvent(EventContext<TInstance> context)
        {
            State<TInstance> state = await _instanceStateAccessor.Get(context);

            State<TInstance> instanceState;
            if (!_stateCache.TryGetValue(state.Name, out instanceState))
                throw new UnknownStateException(_name, state.Name);

            await instanceState.Raise(context);
        }

        async Task StateMachine<TInstance>.RaiseEvent<T>(EventContext<TInstance, T> context)
        {
            State<TInstance> state = await _instanceStateAccessor.Get(context);

            State<TInstance> instanceState;
            if (!_stateCache.TryGetValue(state.Name, out instanceState))
                throw new UnknownStateException(_name, state.Name);

            await instanceState.Raise(context);
        }

        public State<TInstance> GetState(string name)
        {
            State<TInstance> result;
            if (_stateCache.TryGetValue(name, out result))
                return result;

            throw new UnknownStateException(_name, name);
        }

        public IEnumerable<State> States
        {
            get { return _stateCache.Values; }
        }

        Event StateMachine.GetEvent(string name)
        {
            StateMachineEvent<TInstance> result;
            if (_eventCache.TryGetValue(name, out result))
                return result.Event;

            throw new UnknownEventException(_name, name);
        }

        public IEnumerable<Event> Events
        {
            get { return _eventCache.Values.Where(x => false == x.IsTransitionEvent).Select(x => x.Event); }
        }

        Type StateMachine.InstanceType
        {
            get { return typeof(TInstance); }
        }

        public IEnumerable<Event> NextEvents(State state)
        {
            State<TInstance> result;
            if (_stateCache.TryGetValue(state.Name, out result))
                return result.Events;

            throw new UnknownStateException(_name, state.Name);
        }

        public IObservable<StateChanged<TInstance>> StateChanged
        {
            get { return _stateChangedObservable; }
        }

        public IObservable<EventRaising<TInstance>> EventRaising(Event @event)
        {
            StateMachineEvent<TInstance> result;
            if (_eventCache.TryGetValue(@event.Name, out result))
                return result.EventRaising;

            throw new UnknownEventException(_name, @event.Name);
        }

        public IObservable<EventRaised<TInstance>> EventRaised(Event @event)
        {
            StateMachineEvent<TInstance> result;
            if (_eventCache.TryGetValue(@event.Name, out result))
                return result.EventRaised;

            throw new UnknownEventException(_name, @event.Name);
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            Initial.Accept(visitor);

            foreach (var x in _stateCache.Values)
            {
                if (Equals(x, Initial) || Equals(x, Final))
                    continue;

                x.Accept(visitor);
            }

            Final.Accept(visitor);
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
        protected void InstanceState(Expression<Func<TInstance, State>> instanceStateProperty)
        {
            var stateAccessor = new RawStateAccessor<TInstance>(this, instanceStateProperty, _stateChangedObservable);

            _instanceStateAccessor = new InitialIfNullStateAccessor<TInstance>(_stateCache[Initial.Name], stateAccessor);
        }

        /// <summary>
        /// Declares the property to hold the instance's state as a string (the state name is stored in the property)
        /// </summary>
        /// <param name="instanceStateProperty"></param>
        protected void InstanceState(Expression<Func<TInstance, string>> instanceStateProperty)
        {
            var stateAccessor = new StringStateAccessor<TInstance>(this, instanceStateProperty, _stateChangedObservable);

            _instanceStateAccessor = new InitialIfNullStateAccessor<TInstance>(_stateCache[Initial.Name], stateAccessor);
        }

        /// <summary>
        /// Declares the property to hold the instance's state as an int (0 - none, 1 = initial, 2 = final, 3... the rest)
        /// </summary>
        /// <param name="instanceStateProperty"></param>
        /// <param name="states">Specifies the states, in order, to which the int values should be assigned</param>
        protected void InstanceState(Expression<Func<TInstance, int>> instanceStateProperty, params State[] states)
        {
            var stateIndex = new StateAccessorIndex<TInstance>(this, _initial, _final, states);

            var stateAccessor = new IntStateAccessor<TInstance>(instanceStateProperty, stateIndex, _stateChangedObservable);

            _instanceStateAccessor = new InitialIfNullStateAccessor<TInstance>(_stateCache[Initial.Name], stateAccessor);
        }

        /// <summary>
        /// Specifies the name of the state machine
        /// </summary>
        /// <param name="machineName"></param>
        protected void Name(string machineName)
        {
            if (string.IsNullOrWhiteSpace(machineName))
                throw new ArgumentException("The machine name must not be empty", "machineName");

            _name = machineName;
        }

        /// <summary>
        /// Declares an event, and initializes the event property
        /// </summary>
        /// <param name="propertyExpression"></param>
        protected void Event(Expression<Func<Event>> propertyExpression)
        {
            PropertyInfo property = propertyExpression.GetPropertyInfo();

            DeclareTriggerEvent(property);
        }

        void DeclareTriggerEvent(PropertyInfo property)
        {
            string name = property.Name;

            var @event = new TriggerEvent(name);

            property.SetValue(this, @event);

            _eventCache[name] = new StateMachineEvent<TInstance>(@event, false);
        }

        /// <summary>
        /// Declares a data event on the state machine, and initializes the property
        /// </summary>
        /// <param name="propertyExpression">The event property</param>
        protected void Event<T>(Expression<Func<Event<T>>> propertyExpression)
        {
            PropertyInfo property = propertyExpression.GetPropertyInfo();

            DeclareDataEvent<T>(property);
        }

        void DeclareDataEvent<T>(PropertyInfo property)
        {
            string name = property.Name;

            var @event = new DataEvent<T>(name);

            property.SetValue(this, @event);

            _eventCache[name] = new StateMachineEvent<TInstance>(@event, false);
        }

        /// <summary>
        /// Declares a data event on a property of the state machine, and initializes the property
        /// </summary>
        /// <param name="propertyExpression">The property</param>
        /// <param name="eventPropertyExpression">The event property on the property</param>
        protected void Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, Event<T>>> eventPropertyExpression)
            where TProperty : class
        {
            PropertyInfo property = propertyExpression.GetPropertyInfo();
            var propertyValue = property.GetValue(this, null) as TProperty;
            if (propertyValue == null)
                throw new ArgumentException("The property is not initialized: " + property.Name, "propertyExpression");

            PropertyInfo eventProperty = eventPropertyExpression.GetPropertyInfo();

            string name = string.Format("{0}.{1}", property.Name, eventProperty.Name);

            var @event = new DataEvent<T>(name);

            eventProperty.SetValue(propertyValue, @event);

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
        protected void CompositeEvent(Expression<Func<Event>> propertyExpression,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            params Event[] events)
        {
            PropertyInfo trackingPropertyInfo = trackingPropertyExpression.GetPropertyInfo();

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
        protected void CompositeEvent(Expression<Func<Event>> propertyExpression,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events)
        {
            PropertyInfo trackingPropertyInfo = trackingPropertyExpression.GetPropertyInfo();

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
        protected void CompositeEvent(Expression<Func<Event>> propertyExpression,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            params Event[] events)
        {
            PropertyInfo trackingPropertyInfo = trackingPropertyExpression.GetPropertyInfo();

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
        protected void CompositeEvent(Expression<Func<Event>> propertyExpression,
            Expression<Func<TInstance, int>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events)
        {
            PropertyInfo trackingPropertyInfo = trackingPropertyExpression.GetPropertyInfo();

            var accessor = new IntCompositeEventStatusAccessor<TInstance>(trackingPropertyInfo);

            CompositeEvent(propertyExpression, accessor, options, events);
        }


        void CompositeEvent(Expression<Func<Event>> propertyExpression, CompositeEventStatusAccessor<TInstance> accessor,
            CompositeEventOptions options, Event[] events)
        {
            if (events == null)
                throw new ArgumentNullException("events");
            if (events.Length > 31)
                throw new ArgumentException("No more than 31 events can be combined into a single event");
            if (events.Length == 0)
                throw new ArgumentException("At least one event must be specified for a composite event");
            if (events.Any(x => x == null))
                throw new ArgumentException("One or more events specified has not yet been initialized");

            PropertyInfo eventProperty = propertyExpression.GetPropertyInfo();

            string name = eventProperty.Name;

            var @event = new TriggerEvent(name);

            eventProperty.SetValue(this, @event);

            _eventCache[name] = new StateMachineEvent<TInstance>(@event, false);

            var complete = new CompositeEventStatus(Enumerable.Range(0, events.Length)
                .Aggregate(0, (current, x) => current | (1 << x)));

            for (int i = 0; i < events.Length; i++)
            {
                int flag = 1 << i;

                var activity = new CompositeEventActivity<TInstance>(accessor, flag, complete,
                    this, @event);

                Func<State<TInstance>, bool> filter = x => options.HasFlag(CompositeEventOptions.IncludeInitial) || !Equals(x, Initial);
                foreach (var state in _stateCache.Values.Where(filter))
                {
                    During(state,
                        When(events[i])
                            .Execute(x => activity));
                }
            }
        }

        /// <summary>
        /// Declares a state on the state machine, and initialized the property
        /// </summary>
        /// <param name="propertyExpression">The state property</param>
        protected void State(Expression<Func<State>> propertyExpression)
        {
            PropertyInfo property = propertyExpression.GetPropertyInfo();

            DeclareState(property);
        }

        void DeclareState(PropertyInfo property)
        {
            string name = property.Name;

            var state = new StateMachineState<TInstance>(this, name, _eventRaisingObserver, _eventRaisedObserver);

            property.SetValue(this, state);

            SetState(name, state);
        }

        /// <summary>
        /// Declares a state on the state machine, and initialized the property
        /// </summary>
        /// <param name="propertyExpression">The property containing the state</param>
        /// <param name="statePropertyExpression">The state property</param>
        protected void State<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression)
            where TProperty : class
        {
            PropertyInfo property = propertyExpression.GetPropertyInfo();
            var propertyValue = property.GetValue(this, null) as TProperty;
            if (propertyValue == null)
                throw new ArgumentException("The property is not initialized: " + property.Name, "propertyExpression");

            PropertyInfo stateProperty = statePropertyExpression.GetPropertyInfo();

            string name = string.Format("{0}.{1}", property.Name, stateProperty.Name);

            var state = new StateMachineState<TInstance>(this, name, _eventRaisingObserver, _eventRaisedObserver);

            stateProperty.SetValue(propertyValue, state);

            SetState(name, state);
        }

        /// <summary>
        /// Declares a sub-state on the machine. A sub-state is a state that is valid within a super-state,
        /// allowing a state machine to have multiple "states" -- nested parts of an overall state.
        /// </summary>
        /// <param name="propertyExpression">The state property expression</param>
        /// <param name="superState">The superstate of which this state is a substate</param>
        protected void SubState(Expression<Func<State>> propertyExpression, State superState)
        {
            if (superState == null)
                throw new ArgumentNullException("superState");

            State<TInstance> superStateInstance = GetState(superState.Name);

            PropertyInfo property = propertyExpression.GetPropertyInfo();

            string name = property.Name;

            var state = new StateMachineState<TInstance>(this, name, _eventRaisingObserver, _eventRaisedObserver, superStateInstance);

            property.SetValue(this, state);

            SetState(name, state);
        }

        /// <summary>
        /// Declares a state on the state machine, and initialized the property
        /// </summary>
        /// <param name="propertyExpression">The property containing the state</param>
        /// <param name="statePropertyExpression">The state property</param>
        /// <param name="superState">The superstate of which this state is a substate</param>
        protected void SubState<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression, State superState)
            where TProperty : class
        {
            if (superState == null)
                throw new ArgumentNullException("superState");

            State<TInstance> superStateInstance = GetState(superState.Name);

            PropertyInfo property = propertyExpression.GetPropertyInfo();
            var propertyValue = property.GetValue(this, null) as TProperty;
            if (propertyValue == null)
                throw new ArgumentException("The property is not initialized: " + property.Name, "propertyExpression");

            PropertyInfo stateProperty = statePropertyExpression.GetPropertyInfo();

            string name = string.Format("{0}.{1}", property.Name, stateProperty.Name);

            var state = new StateMachineState<TInstance>(this, name, _eventRaisingObserver, _eventRaisedObserver, superStateInstance);

            stateProperty.SetValue(propertyValue, state);

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
        protected void During(State state, params EventActivities<TInstance>[] activities)
        {
            ActivityBinder<TInstance>[] activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            BindActivitiesToState(state, activitiesBinder);
        }

        /// <summary>
        /// Declares the events and associated activities that are handled during the specified states
        /// </summary>
        /// <param name="state1">The state</param>
        /// <param name="state2">The other state</param>
        /// <param name="activities">The event and activities</param>
        protected void During(State state1, State state2, params EventActivities<TInstance>[] activities)
        {
            ActivityBinder<TInstance>[] activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

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
        protected void During(State state1, State state2, State state3, params EventActivities<TInstance>[] activities)
        {
            ActivityBinder<TInstance>[] activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

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
        protected void During(State state1, State state2, State state3, State state4,
            params EventActivities<TInstance>[] activities)
        {
            ActivityBinder<TInstance>[] activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

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
        protected void During(IEnumerable<State> states, params EventActivities<TInstance>[] activities)
        {
            ActivityBinder<TInstance>[] activitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            foreach (State state in states)
                BindActivitiesToState(state, activitiesBinder);
        }

        void BindActivitiesToState(State state, IEnumerable<ActivityBinder<TInstance>> eventActivities)
        {
            State<TInstance> activityState = GetState(state.Name);

            foreach (var activity in eventActivities)
                activity.Bind(activityState);
        }

        /// <summary>
        /// Declares the events and activities that are handled during the initial state
        /// </summary>
        /// <param name="activities">The event and activities</param>
        protected void Initially(params EventActivities<TInstance>[] activities)
        {
            During(Initial, activities);
        }

        /// <summary>
        /// Declares events and activities that are handled during any state exception Initial and Final
        /// </summary>
        /// <param name="activities">The event and activities</param>
        protected void DuringAny(params EventActivities<TInstance>[] activities)
        {
            IEnumerable<State<TInstance>> states = _stateCache.Values.Where(x => !Equals(x, Initial) && !Equals(x, Final));

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
        protected void Finally(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            EventActivityBinder<TInstance> binder = When(Final.Enter);

            binder = activityCallback(binder);

            DuringAny(binder);
        }

        void BindTransitionEvents(State<TInstance> state, IEnumerable<EventActivities<TInstance>> activities)
        {
            IEnumerable<ActivityBinder<TInstance>> eventActivities = activities
                .SelectMany(activity => activity.GetStateActivityBinders().Where(x => x.IsStateTransitionEvent(state)));

            foreach (var eventActivity in eventActivities)
                eventActivity.Bind(state);
        }

        /// <summary>
        /// When the event is fired in this state, execute the chained activities
        /// </summary>
        /// <param name="event">The fired event</param>
        /// <returns></returns>
        protected EventActivityBinder<TInstance> When(Event @event)
        {
            return new TriggerEventActivityBinder<TInstance>(this, @event);
        }

        /// <summary>
        /// When entering the specified state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected void WhenEnter(State state, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            State<TInstance> activityState = GetState(state.Name);

            EventActivityBinder<TInstance> binder = new TriggerEventActivityBinder<TInstance>(this, activityState.Enter);

            binder = activityCallback(binder);

            During(state, binder);
        }

        /// <summary>
        /// When leaving the specified state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        protected void WhenLeave(State state, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            State<TInstance> activityState = GetState(state.Name);

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
        protected void BeforeEnter(State state,
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            State<TInstance> activityState = GetState(state.Name);

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
        protected void AfterLeave(State state,
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            State<TInstance> activityState = GetState(state.Name);

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
        protected EventActivityBinder<TInstance, TData> When<TData>(Event<TData> @event)
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
        protected EventActivityBinder<TInstance, TData> When<TData>(Event<TData> @event,
            StateMachineEventFilter<TInstance, TData> filter)
        {
            return new DataEventActivityBinder<TInstance, TData>(this, @event, filter);
        }

        /// <summary>
        /// Ignore the event in this state (no exception is thrown)
        /// </summary>
        /// <param name="event">The ignored event</param>
        /// <returns></returns>
        protected EventActivities<TInstance> Ignore(Event @event)
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
        protected EventActivities<TInstance> Ignore<TData>(Event<TData> @event)
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
        protected EventActivities<TInstance> Ignore<TData>(Event<TData> @event,
            StateMachineEventFilter<TInstance, TData> filter)
        {
            ActivityBinder<TInstance> activityBinder = new IgnoreEventActivityBinder<TInstance, TData>(@event, filter);

            return new DataEventActivityBinder<TInstance, TData>(this, @event, activityBinder);
        }

        /// <summary>
        /// Specifies a callback to invoke when an event is raised in a state where the event is not handled
        /// </summary>
        /// <param name="callback">The unhandled event callback</param>
        protected void OnUnhandledEvent(UnhandledEventCallback<TInstance> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

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
            foreach (StateMachineRegistration declaration in _registrations.Value)
                declaration.Declare(this);
        }


        static IEnumerable<PropertyInfo> GetStateMachineProperties(TypeInfo typeInfo)
        {
            if (typeInfo.IsInterface)
                yield break;

            if (typeInfo.BaseType != null)
            {
                foreach (PropertyInfo propertyInfo in GetStateMachineProperties(typeInfo.BaseType.GetTypeInfo()))
                    yield return propertyInfo;
            }

            IEnumerable<PropertyInfo> properties = typeInfo.DeclaredMethods
                .Where(x => x.IsSpecialName && x.Name.StartsWith("get_") && !x.IsStatic)
                .Select(x => typeInfo.GetDeclaredProperty(x.Name.Substring("get_".Length)))
                .Where(x => x.CanRead && x.CanWrite);

            foreach (PropertyInfo propertyInfo in properties)
                yield return propertyInfo;
        }

        StateMachineRegistration[] GetRegistrations()
        {
            var events = new List<StateMachineRegistration>();

            Type machineType = GetType();

            IEnumerable<PropertyInfo> properties = GetStateMachineProperties(machineType.GetTypeInfo());

            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.PropertyType.IsGenericType)
                {
                    if (propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Event<>))
                    {
                        Type declarationType = typeof(DataEventRegistration<,>).MakeGenericType(typeof(TInstance), machineType,
                            propertyInfo.PropertyType.GetGenericArguments().First());
                        object declaration = Activator.CreateInstance(declarationType, propertyInfo);
                        events.Add((StateMachineRegistration)declaration);
                    }
                }
                else
                {
                    if (propertyInfo.PropertyType == typeof(Event))
                    {
                        Type declarationType = typeof(TriggerEventRegistration<>).MakeGenericType(typeof(TInstance), machineType);
                        object declaration = Activator.CreateInstance(declarationType, propertyInfo);
                        events.Add((StateMachineRegistration)declaration);
                    }
                    else if (propertyInfo.PropertyType == typeof(State))
                    {
                        Type declarationType = typeof(StateRegistration<>).MakeGenericType(typeof(TInstance), machineType);
                        object declaration = Activator.CreateInstance(declarationType, propertyInfo);
                        events.Add((StateMachineRegistration)declaration);
                    }
                }
            }

            return events.ToArray();
        }


        class DataEventRegistration<TStateMachine, TData> :
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
                var machine = ((TStateMachine)stateMachine);
                object existing = _propertyInfo.GetValue(machine);
                if (existing != null)
                    return;

                machine.DeclareDataEvent<TData>(_propertyInfo);
            }
        }


        interface StateMachineRegistration
        {
            void Declare(object stateMachine);
        }


        class StateRegistration<TStateMachine> :
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
                var machine = ((TStateMachine)stateMachine);
                object existing = _propertyInfo.GetValue(machine);
                if (existing != null)
                    return;

                machine.DeclareState(_propertyInfo);
            }
        }


        class TriggerEventRegistration<TStateMachine> :
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
                var machine = ((TStateMachine)stateMachine);
                object existing = _propertyInfo.GetValue(machine);
                if (existing != null)
                    return;

                machine.DeclareTriggerEvent(_propertyInfo);
            }
        }
    }
}