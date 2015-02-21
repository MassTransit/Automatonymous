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
    using Events;
    using Internals;
    using Observers;
    using States;


    public abstract class AutomatonymousStateMachine<TInstance> :
        Visitable,
        StateMachine<TInstance>
        where TInstance : class
    {
        readonly Dictionary<string, StateMachineEvent<TInstance>> _eventCache;
        readonly EventRaisedObserver<TInstance> _eventRaisedObserver;
        readonly EventRaisingObserver<TInstance> _eventRaisingObserver;
        readonly State<TInstance> _final;
        readonly State<TInstance> _initial;
        readonly Dictionary<string, State<TInstance>> _stateCache;
        readonly Observable<StateChanged<TInstance>> _stateChangedObservable;
        StateAccessor<TInstance> _instanceStateAccessor;
        string _name;

        protected AutomatonymousStateMachine()
        {
            _stateCache = new Dictionary<string, State<TInstance>>();
            _eventCache = new Dictionary<string, StateMachineEvent<TInstance>>();

            _stateChangedObservable = new Observable<StateChanged<TInstance>>();
            _eventRaisingObserver = new EventRaisingObserver<TInstance>(_eventCache);
            _eventRaisedObserver = new EventRaisedObserver<TInstance>(_eventCache);

            _initial = new StateImpl<TInstance>(this, "Initial", _eventRaisingObserver, _eventRaisedObserver);
            _stateCache[_initial.Name] = _initial;
            _final = new StateImpl<TInstance>(this, "Final", _eventRaisingObserver, _eventRaisedObserver);
            _stateCache[_final.Name] = _final;

            _instanceStateAccessor = new DefaultInstanceStateAccessor<TInstance>(this, _stateCache[Initial.Name], _stateChangedObservable);

            _name = GetType().Name;
        }

        public void Accept(StateMachineVisitor visitor)
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

        public async Task RaiseEvent(EventContext<TInstance> context)
        {
            State<TInstance> state = await _instanceStateAccessor.Get(context);

            State<TInstance> instanceState;
            if (!_stateCache.TryGetValue(state.Name, out instanceState))
                throw new UnknownStateException(_name, state.Name);

            await instanceState.Raise(context);
        }

        public async Task RaiseEvent<T>(EventContext<TInstance, T> context)
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
            get { return _eventCache.Values.Select(x => x.Event); }
        }

        public Type InstanceType
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
            _instanceStateAccessor = new InitialIfNullStateAccessor<TInstance>(this, instanceStateProperty,
                _stateCache[Initial.Name], _stateChangedObservable);
        }

        protected void Event(Expression<Func<Event>> propertyExpression)
        {
            PropertyInfo property = propertyExpression.GetPropertyInfo();

            string name = property.Name;

            var @event = new SimpleEvent(name);

            property.SetValue(this, @event);

            _eventCache[name] = new StateMachineEvent<TInstance>(this, @event);
        }

        protected void Name(string machineName)
        {
            if (string.IsNullOrWhiteSpace(machineName))
                throw new ArgumentException("The machine name must not be empty", "machineName");

            _name = machineName;
        }

        /// <summary>
        /// Adds a composite event to the state machine. A composite event is triggered when all
        /// off the required events have been raised. Note that required events cannot be in the initial
        /// state since it would cause extra instances of the state machine to be created
        /// </summary>
        /// <param name="propertyExpression">The composite event</param>
        /// <param name="trackingPropertyExpression">The property in the instance used to track the state of the composite event</param>
        /// <param name="events">The events that must be raised before the composite event is raised</param>
        protected void Event(Expression<Func<Event>> propertyExpression,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            params Event[] events)
        {
            Event(propertyExpression, trackingPropertyExpression, CompositeEventOptions.None, events);
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
        protected void Event(Expression<Func<Event>> propertyExpression,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events)
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
            PropertyInfo trackingPropertyInfo = trackingPropertyExpression.GetPropertyInfo();

            string name = eventProperty.Name;

            var @event = new SimpleEvent(name);

            eventProperty.SetValue(this, @event);

            _eventCache[name] = new StateMachineEvent<TInstance>(this, @event);

            var complete = new CompositeEventStatus(Enumerable.Range(0, events.Length)
                .Aggregate(0, (current, x) => current | (1 << x)));

            for (int i = 0; i < events.Length; i++)
            {
                int flag = 1 << i;

                var activity = new CompositeEventActivity<TInstance>(trackingPropertyInfo, flag, complete,
                    context =>
                    {
                        BehaviorContext<TInstance> compositeEventContext = context.GetProxy(@event);

                        return ((StateMachine<TInstance>)this).RaiseEvent(compositeEventContext);
                    });

                Func<State<TInstance>, bool> filter = x => options.HasFlag(CompositeEventOptions.IncludeInitial) || !Equals(x, Initial);
                foreach (var state in _stateCache.Values.Where(filter))
                {
                    During(state,
                        When(events[i])
                            .Execute(x => activity));
                }
            }
        }

        protected void Event<T>(Expression<Func<Event<T>>> propertyExpression)
        {
            PropertyInfo property = propertyExpression.GetPropertyInfo();

            string name = property.Name;

            var @event = new DataEvent<T>(name);

            property.SetValue(this, @event);

            _eventCache[name] = new StateMachineEvent<TInstance>(this, @event);
        }

        protected void State(Expression<Func<State>> propertyExpression)
        {
            PropertyInfo property = propertyExpression.GetPropertyInfo();

            string name = property.Name;

            var state = new StateImpl<TInstance>(this, name, _eventRaisingObserver, _eventRaisedObserver);

            property.SetValue(this, state);

            _stateCache[name] = state;
        }

        protected void During(State state, params EventActivities<TInstance>[] activities)
        {
            StateActivityBinder<TInstance>[] stateActivitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            BindActivitiesToState(state, stateActivitiesBinder);
        }

        protected void During(State state1, State state2, params EventActivities<TInstance>[] activities)
        {
            StateActivityBinder<TInstance>[] stateActivitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            BindActivitiesToState(state1, stateActivitiesBinder);
            BindActivitiesToState(state2, stateActivitiesBinder);
        }

        protected void During(State state1, State state2, State state3, params EventActivities<TInstance>[] activities)
        {
            StateActivityBinder<TInstance>[] stateActivitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            BindActivitiesToState(state1, stateActivitiesBinder);
            BindActivitiesToState(state2, stateActivitiesBinder);
            BindActivitiesToState(state3, stateActivitiesBinder);
        }

        protected void During(State state1, State state2, State state3, State state4,
            params EventActivities<TInstance>[] activities)
        {
            StateActivityBinder<TInstance>[] stateActivitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            BindActivitiesToState(state1, stateActivitiesBinder);
            BindActivitiesToState(state2, stateActivitiesBinder);
            BindActivitiesToState(state3, stateActivitiesBinder);
            BindActivitiesToState(state4, stateActivitiesBinder);
        }

        protected void During(IEnumerable<State> states, params EventActivities<TInstance>[] activities)
        {
            StateActivityBinder<TInstance>[] stateActivitiesBinder = activities.SelectMany(x => x.GetStateActivityBinders()).ToArray();

            foreach (State state in states)
                BindActivitiesToState(state, stateActivitiesBinder);
        }

        void BindActivitiesToState(State state, IEnumerable<StateActivityBinder<TInstance>> eventActivities)
        {
            State<TInstance> activityState = GetState(state.Name);

            foreach (var activity in eventActivities)
                activity.Bind(activityState);
        }

        protected void Initially(params EventActivities<TInstance>[] activities)
        {
            During(Initial, activities);
        }

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

        void BindTransitionEvents(State<TInstance> state, IEnumerable<EventActivities<TInstance>> activities)
        {
            IEnumerable<StateActivityBinder<TInstance>> eventActivities = activities
                .SelectMany(activity => activity.GetStateActivityBinders().Where(x => x.IsStateTransitionEvent(state)));

            foreach (var eventActivity in eventActivities)
                eventActivity.Bind(state);
        }

        protected void Finally(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            EventActivityBinder<TInstance> binder = When(Final.Enter);

            binder = activityCallback(binder);

            DuringAny(binder);
        }

        protected EventActivityBinder<TInstance> When(Event @event)
        {
            return new SimpleEventActivityBinder<TInstance>(this, @event);
        }


        protected EventActivityBinder<TInstance> Ignore(Event @event)
        {
            EventActivityBinder<TInstance> binder = new SimpleEventActivityBinder<TInstance>(this, @event);

            return binder.Ignore();
        }

        protected EventActivityBinder<TInstance, TData> When<TData>(Event<TData> @event)
        {
            return new DataEventActivityBinder<TInstance, TData>(this, @event);
        }

        protected EventActivityBinder<TInstance, TData> Ignore<TData>(Event<TData> @event)
        {
            EventActivityBinder<TInstance, TData> binder = new DataEventActivityBinder<TInstance, TData>(this, @event);

            return binder.Ignore();
        }

        protected EventActivityBinder<TInstance, TData> When<TData>(Event<TData> @event,
            Func<BehaviorContext<TInstance, TData>, bool> filterExpression)
        {
            return new DataEventActivityBinder<TInstance, TData>(this, @event, filterExpression);
        }
    }
}