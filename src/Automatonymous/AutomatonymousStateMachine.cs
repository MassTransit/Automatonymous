// Copyright 2011 Chris Patterson, Dru Sellers, Henrik Feldt
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
    using Activities;
    using Binders;
    using Impl;
    using Internals.Caching;
    using Internals.Extensions;
    using Internals.Primitives;

    /// <summary>
    /// <para>The automatonymous state machine provides methods
    /// for triggering events (that cause state transitions to
    /// occur), inspecting the machine by using the Accept-methods,
    /// as well as programmatically inspecting its events and states.</para>
    /// 
    /// <para>The When-methods allow state machine implementors
    /// to specify activities to trigger, by calling on the returned
    /// EventActivityBinder.</para>
    /// 
    /// <para>DuringAny allows state machine implementors
    /// to bind to events irregardless of the current state
    /// at which the event occurs.</para>
    /// 
    /// <para>Finally, there's the Finally method (pun intended),
    /// which comes together with Initially method; both of which
    /// deal with the end and start of the state machine, respectively.</para>
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public abstract class AutomatonymousStateMachine<TInstance> :
        AcceptStateMachineInspector,
        StateMachine<TInstance>
        where TInstance : class
    {
        readonly Cache<string, StateMachineEvent<TInstance>> _eventCache;
        readonly EventRaisedObserver<TInstance> _eventRaisedObserver;
        readonly EventRaisingObserver<TInstance> _eventRaisingObserver;
        readonly Cache<string, State<TInstance>> _stateCache;
        readonly Observable<StateChanged<TInstance>> _stateChangedObservable;
        StateAccessor<TInstance> _instanceStateAccessor;

        /// <summary>
        /// This c'tor initializes the state machine internal state, that keeps
        /// track of what states there are. Without calling any of the methods
        /// on this object, there are already the <see cref="Initial"/> and
        /// <see cref="Final"/> states.
        /// </summary>
        protected AutomatonymousStateMachine()
        {
            _stateCache = new DictionaryCache<string, State<TInstance>>();
            _eventCache = new DictionaryCache<string, StateMachineEvent<TInstance>>();

            _stateChangedObservable = new Observable<StateChanged<TInstance>>();
            _eventRaisingObserver = new EventRaisingObserver<TInstance>(_eventCache);
            _eventRaisedObserver = new EventRaisedObserver<TInstance>(_eventCache);

            State(() => Initial);
            State(() => Final);

            _instanceStateAccessor = new DefaultInstanceStateAccessor<TInstance>(_stateCache[Initial.Name],
                _stateChangedObservable);
        }

        /// <summary>
        /// Accepts an inspector on which Accept is called for each of the states.
        /// </summary>
        /// <param name="inspector">Instance to thread through all state machine states.</param>
        public void Accept(StateMachineInspector inspector)
        {
            Initial.Accept(inspector);

            _stateCache.Each(x =>
                {
                    if (x == Initial || x == Final)
                        return;

                    x.Accept(inspector);
                });

            Final.Accept(inspector);
        }

        /// <summary>
        /// Gets the instance state accessor which can be used to
        /// get the state of the machine.
        /// </summary>
        public StateAccessor<TInstance> InstanceStateAccessor
        {
            get { return _instanceStateAccessor; }
        }

        public State Initial { get; private set; }
        public State Final { get; private set; }

        State StateMachine.GetState(string name)
        {
            return _stateCache[name];
        }

        State<TInstance> StateMachine<TInstance>.GetState(string name)
        {
            return _stateCache[name];
        }

        public IEnumerable<State> States
        {
            get { return _stateCache; }
        }

        Event StateMachine.GetEvent(string name)
        {
            return _eventCache[name].Event;
        }

        public IEnumerable<Event> Events
        {
            get { return _eventCache.Select(x => x.Event); }
        }

        public Type InstanceType
        {
            get { return typeof(TInstance); }
        }

        public IEnumerable<Event> NextEvents(State state)
        {
            return _stateCache[state.Name].Events
                .Distinct(new NameEqualityComparer());
        }

        public async Task RaiseEvent(TInstance instance, Event @event)
        {
            await WithInstance(instance, async x =>
                {
                    State<TInstance> currentState = InstanceStateAccessor.Get(instance);

                    await _stateCache[currentState.Name].Raise(instance, @event);
                });
        }

        public async Task RaiseEvent<TData>(TInstance instance, Event<TData> @event, TData value)
        {
            await WithInstance(instance, async x =>
                {
                    State<TInstance> currentState = InstanceStateAccessor.Get(instance);

                    await _stateCache[currentState.Name].Raise(instance, @event, value);
                });
        }

        public IObservable<StateChanged<TInstance>> StateChanged
        {
            get { return _stateChangedObservable; }
        }

        public IObservable<EventRaising<TInstance>> EventRaising(Event @event)
        {
            if (!_eventCache.Has(@event.Name))
                throw new ArgumentException("Unknown event: " + @event.Name, "event");

            return _eventCache[@event.Name].EventRaising;
        }

        public IObservable<EventRaised<TInstance>> EventRaised(Event @event)
        {
            if (!_eventCache.Has(@event.Name))
                throw new ArgumentException("Unknown event: " + @event.Name, "event");

            return _eventCache[@event.Name].EventRaised;
        }

        /// <summary>
        /// Declares what property holds the TInstance's state on the current instance of the state machine
        /// </summary>
        /// <param name="instanceStateProperty"></param>
        /// <remarks>Setting the state accessor more than once will cause the 
        /// property managed by the state machine to change each time.
        /// Please note, the state machine can only manage one property at a 
        /// given time per instance, and the best practice is to manage one 
        /// property per machine.</remarks>
        protected void InstanceState(Expression<Func<TInstance, State>> instanceStateProperty)
        {
            _instanceStateAccessor = new InitialIfNullStateAccessor<TInstance>(instanceStateProperty,
                _stateCache[Initial.Name], _stateChangedObservable);
        }

        Task WithInstance(TInstance instance, Func<TInstance, Task> callback)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            return callback(instance);
        }

        protected void Event(Expression<Func<Event>> propertyExpression)
        {
            PropertyInfo property = propertyExpression.GetPropertyInfo();

            string name = property.Name;

            var @event = new SimpleEvent(name);

            property.SetValue(this, @event);

            _eventCache[name] = new StateMachineEvent<TInstance>(@event);
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
            if (events.Length > 31)
                throw new ArgumentException("No more than 31 events can be combined into a single event");

            PropertyInfo eventProperty = propertyExpression.GetPropertyInfo();
            PropertyInfo trackingPropertyInfo = trackingPropertyExpression.GetPropertyInfo();

            string name = eventProperty.Name;

            var @event = new SimpleEvent(name);

            eventProperty.SetValue(this, @event);

            _eventCache[name] = new StateMachineEvent<TInstance>(@event);

            var complete = new CompositeEventStatus(Enumerable.Range(0, events.Length)
                .Aggregate(0, (current, x) => current | (1 << x)));

            for (int i = 0; i < events.Length; i++)
            {
                int flag = 1 << i;

                var activity = new CompositeEventActivity<TInstance>(trackingPropertyInfo, flag, complete,
                    async instance => await RaiseEvent(instance, @event));

                foreach (var state in _stateCache.Where(x => x != Initial))
                {
                    During(state,
                        When(events[i])
                            .Then(() => activity));
                }
            }
        }

        protected void Event<T>(Expression<Func<Event<T>>> propertyExpression)
        {
            PropertyInfo property = propertyExpression.GetPropertyInfo();

            string name = property.Name;

            var @event = new DataEvent<T>(name);

            property.SetValue(this, @event);

            _eventCache[name] = new StateMachineEvent<TInstance>(@event);
        }

        protected void State(Expression<Func<State>> propertyExpression)
        {
            PropertyInfo property = propertyExpression.GetPropertyInfo();

            string name = property.Name;

            var state = new StateImpl<TInstance>(name, _eventRaisingObserver, _eventRaisedObserver);

            property.SetValue(this, state);

            _stateCache[name] = state;
        }


        protected void During(State state, params IEnumerable<EventActivity<TInstance>>[] activities)
        {
            State<TInstance> activityState = state.For<TInstance>();

            foreach (var activity in activities.SelectMany(x => x))
                activityState.Bind(activity);
        }

        protected void Initially(params IEnumerable<EventActivity<TInstance>>[] activities)
        {
            During(_stateCache["Initial"], activities);
        }

        protected void Finally(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            EventActivityBinder<TInstance> binder = When(Final.Enter);

            binder = activityCallback(binder);

            DuringAny(binder);
        }

        protected void DuringAny(params IEnumerable<EventActivity<TInstance>>[] activities)
        {
            foreach (var state in _stateCache)
                During(state, activities);
        }

        protected EventActivityBinder<TInstance> When(Event @event)
        {
            return new SimpleEventActivityBinder<TInstance>(this, @event);
        }

        protected EventActivityBinder<TInstance, TData> When<TData>(Event<TData> @event)
        {
            return new DataEventActivityBinder<TInstance, TData>(this, @event);
        }

        protected EventActivityBinder<TInstance, TData> When<TData>(Event<TData> @event,
            Expression<Func<TData, bool>> filterExpression)
        {
            return new DataEventActivityBinder<TInstance, TData>(this, @event, filterExpression);
        }


        class NameEqualityComparer : IEqualityComparer<Event>
        {
            public bool Equals(Event x, Event y)
            {
                return Equals(x.Name, y.Name);
            }

            public int GetHashCode(Event @event)
            {
                return @event.Name.GetHashCode();
            }
        }
    }
}