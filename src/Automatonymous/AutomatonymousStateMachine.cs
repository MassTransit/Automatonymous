// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	using Activities;

	using Binders;

	using Impl;

	using Internals.Caching;
	using Internals.Extensions;
	using Internals.Primitives;

	using Taskell;


	public abstract class AutomatonymousStateMachine
	{
		private static readonly object PropertyLocker = new object();
		private static readonly IDictionary<Type, IEnumerable<PropertyInfo>> Properties = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

		protected static readonly Type OpenGenericDataEventType = typeof (DataEvent<>);

		// TODO: make this an Extension Method of Type
		private static IEnumerable<Type> GetInheritenceChain(Type type)
		{
			var result = new List<Type>
			{
				type
			};

			var b = type.BaseType;

			while (b != null)
			{
				result.Add(b);

				b = b.BaseType;
			}

			return result;
		}

		protected static IEnumerable<PropertyInfo> GetProperties(Type type)
		{
			return GetProperties(type, BindingFlags.Public | BindingFlags.Instance);
		}

		protected static IEnumerable<PropertyInfo> GetProperties(Type type, BindingFlags flags)
		{
			if (Properties.ContainsKey(type) == false)
			{
				lock (PropertyLocker)
				{
					if (Properties.ContainsKey(type) == false)
					{
						var properties = GetInheritenceChain(type)
							.SelectMany(x => x.GetProperties(flags | BindingFlags.DeclaredOnly));

						Properties[type] = properties;
					}
				}
			}

			return Properties[type];
		}
	}

	public abstract class AutomatonymousStateMachine<TInstance> :
		AutomatonymousStateMachine,
		AcceptStateMachineInspector,
		StateMachine<TInstance>
		where TInstance : class
	{
		void RegisterEvents()
		{
			var properties = GetProperties(GetType())
				.Where(x => typeof (Event).IsAssignableFrom(x.PropertyType)); // TODO: this may just need to be equals

			foreach (var property in properties)
			{
				var name = property.Name;

				Event @event;

				if (property.PropertyType.IsGenericType)
				{
					var dataEventType = OpenGenericDataEventType.MakeGenericType(property.PropertyType.GetGenericArguments());

					@event = (Event) Activator.CreateInstance(dataEventType, name);
				}
				else
				{
					@event = new SimpleEvent(name);
				}

				_eventCache[name] = new StateMachineEvent<TInstance>(@event);

				property.SetValue(this, @event);
			}
		}

		void RegisterStates()
		{
			var properties = GetProperties(GetType())
				.Where(x => typeof (State).IsAssignableFrom(x.PropertyType)); // TODO: this may just need to be equals

			foreach (var property in properties)
			{
				var name = property.Name;

				State<TInstance> state = new StateImpl<TInstance>(name, _eventRaisingObserver, _eventRaisedObserver);

				_stateCache[name] = state;

				property.SetValue(this, state);
			}
		}

		readonly Cache<string, StateMachineEvent<TInstance>> _eventCache;
		readonly EventRaisedObserver<TInstance> _eventRaisedObserver;
		readonly EventRaisingObserver<TInstance> _eventRaisingObserver;
		readonly Cache<string, State<TInstance>> _stateCache;
		readonly Observable<StateChanged<TInstance>> _stateChangedObservable;
		StateAccessor<TInstance> _instanceStateAccessor;

		protected AutomatonymousStateMachine()
		{
			_stateCache = new DictionaryCache<string, State<TInstance>>();
			_eventCache = new DictionaryCache<string, StateMachineEvent<TInstance>>();

			_stateChangedObservable = new Observable<StateChanged<TInstance>>();
			_eventRaisingObserver = new EventRaisingObserver<TInstance>(_eventCache);
			_eventRaisedObserver = new EventRaisedObserver<TInstance>(_eventCache);

			RegisterStates();
			RegisterEvents();

			_instanceStateAccessor = new DefaultInstanceStateAccessor<TInstance>(_stateCache[Initial.Name], _stateChangedObservable);
		}

		public void Accept(StateMachineInspector inspector)
		{
			Initial.Accept(inspector);

			_stateCache.Each(x =>
			{
				if (Equals(x, Initial) || Equals(x, Final))
					return;

				x.Accept(inspector);
			});

			Final.Accept(inspector);
		}

		StateAccessor<TInstance> StateMachine<TInstance>.InstanceStateAccessor
		{
			get { return _instanceStateAccessor; }
		}

		public State Initial { get; protected set; }
		public State Final { get; private set; }

		State StateMachine.GetState(string name)
		{
			return _stateCache[name];
		}

		void StateMachine<TInstance>.RaiseEvent(Composer composer, TInstance instance, Event @event)
		{
			composer.Execute(() =>
			{
				State<TInstance> state = _instanceStateAccessor.Get(instance);

				State<TInstance> instanceState = _stateCache[state.Name];

				return composer.ComposeEvent(instance, instanceState, @event);
			});
		}

		void StateMachine<TInstance>.RaiseEvent<TData>(Composer composer, TInstance instance, Event<TData> @event, TData data)
		{
			composer.Execute(() =>
			{
				State<TInstance> state = _instanceStateAccessor.Get(instance);

				State<TInstance> instanceState = _stateCache[state.Name];

				return composer.ComposeEvent(instance, instanceState, @event, data);
			});
		}

		public State<TInstance> GetState(string name)
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
			return _stateCache[state.Name].Events;
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
		/// <remarks>Setting the state accessor more than once will cause the property managed by the state machine to change each time.
		/// Please note, the state machine can only manage one property at a given time per instance, 
		/// and the best practice is to manage one property per machine.
		/// </remarks>
		protected void InstanceState(Expression<Func<TInstance, State>> instanceStateProperty)
		{
			_instanceStateAccessor = new InitialIfNullStateAccessor<TInstance>(instanceStateProperty,
				_stateCache[Initial.Name], _stateChangedObservable);
		}

		//protected void Event(Expression<Func<Event>> propertyExpression)
		//{
		//	PropertyInfo property = propertyExpression.GetPropertyInfo();

		//	string name = property.Name;

		//	var @event = new SimpleEvent(name);

		//	property.SetValue(this, @event);

		//	_eventCache[name] = new StateMachineEvent<TInstance>(@event);
		//}

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

			_eventCache[name] = new StateMachineEvent<TInstance>(@event);

			var complete = new CompositeEventStatus(Enumerable.Range(0, events.Length)
				.Aggregate(0, (current, x) => current | (1 << x)));

			for (int i = 0; i < events.Length; i++)
			{
				int flag = 1 << i;

				var activity = new CompositeEventActivity<TInstance>(trackingPropertyInfo, flag, complete,
					(consumer, instance) => ((StateMachine<TInstance>)this).RaiseEvent(consumer, instance, @event));

				foreach (var state in _stateCache.Where(x => !Equals(x, Initial)))
				{
					During(state,
						When(events[i])
							.Then(() => activity));
				}
			}
		}

		//protected void Event<T>(Expression<Func<Event<T>>> propertyExpression)
		//{
		//	PropertyInfo property = propertyExpression.GetPropertyInfo();

		//	string name = property.Name;

		//	var @event = new DataEvent<T>(name);

		//	property.SetValue(this, @event);

		//	_eventCache[name] = new StateMachineEvent<TInstance>(@event);
		//}

		//protected void State(Expression<Func<State>> propertyExpression)
		//{
		//	PropertyInfo property = propertyExpression.GetPropertyInfo();

		//	string name = property.Name;

		//	var state = new StateImpl<TInstance>(name, _eventRaisingObserver, _eventRaisedObserver);

		//	property.SetValue(this, state);

		//	_stateCache[name] = state;
		//}

		protected void During(State state, params IEnumerable<EventActivity<TInstance>>[] activities)
		{
			EventActivity<TInstance>[] eventActivities = activities.SelectMany(x => x).ToArray();

			BindActivitiesToState(state, eventActivities);
		}

		protected void During(State state1, State state2, params IEnumerable<EventActivity<TInstance>>[] activities)
		{
			EventActivity<TInstance>[] eventActivities = activities.SelectMany(x => x).ToArray();

			BindActivitiesToState(state1, eventActivities);
			BindActivitiesToState(state2, eventActivities);
		}

		protected void During(State state1, State state2, State state3, params IEnumerable<EventActivity<TInstance>>[] activities)
		{
			EventActivity<TInstance>[] eventActivities = activities.SelectMany(x => x).ToArray();

			BindActivitiesToState(state1, eventActivities);
			BindActivitiesToState(state2, eventActivities);
			BindActivitiesToState(state3, eventActivities);
		}

		protected void During(State state1, State state2, State state3, State state4,
			params IEnumerable<EventActivity<TInstance>>[] activities)
		{
			EventActivity<TInstance>[] eventActivities = activities.SelectMany(x => x).ToArray();

			BindActivitiesToState(state1, eventActivities);
			BindActivitiesToState(state2, eventActivities);
			BindActivitiesToState(state3, eventActivities);
			BindActivitiesToState(state4, eventActivities);
		}

		protected void During(IEnumerable<State> states, params IEnumerable<EventActivity<TInstance>>[] activities)
		{
			EventActivity<TInstance>[] eventActivities = activities.SelectMany(x => x).ToArray();

			foreach (var state in states)
			{
				BindActivitiesToState(state, eventActivities);
			}
		}

		static void BindActivitiesToState(State state, IEnumerable<EventActivity<TInstance>> eventActivities)
		{
			State<TInstance> activityState = state.For<TInstance>();

			foreach (var activity in eventActivities)
				activityState.Bind(activity);
		}

		protected void Initially(params IEnumerable<EventActivity<TInstance>>[] activities)
		{
			During(Initial, activities);
		}

		protected void DuringAny(params IEnumerable<EventActivity<TInstance>>[] activities)
		{
			IEnumerable<State<TInstance>> states = _stateCache.Where(x => !Equals(x, Initial) && !Equals(x, Final));

			// we only add DuringAny event handlers to non-initial and non-final states to avoid
			// reviving finalized state machine instances or creating new ones accidentally.
			foreach (var state in states)
				During(state, activities);

			BindTransitionEvents(Initial, activities);
			BindTransitionEvents(Final, activities);
		}

		void BindTransitionEvents(State state, IEnumerable<IEnumerable<EventActivity<TInstance>>> activities)
		{
			IEnumerable<EventActivity<TInstance>> eventActivities = activities
				.SelectMany(activity => activity.Where(x => IsTransitionEvent(state, x.Event)));

			foreach (var eventActivity in eventActivities)
				During(state, new[] { eventActivity });
		}

		bool IsTransitionEvent(State state, Event eevent)
		{
			return Equals(eevent, state.Enter) || Equals(eevent, state.BeforeEnter)
					|| Equals(eevent, state.AfterLeave) || Equals(eevent, state.Leave);
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

		protected EventActivityBinder<TInstance, TData> When<TData>(Event<TData> @event)
		{
			return new DataEventActivityBinder<TInstance, TData>(this, @event);
		}

		protected EventActivityBinder<TInstance, TData> When<TData>(Event<TData> @event,
			Expression<Func<TData, bool>> filterExpression)
		{
			return new DataEventActivityBinder<TInstance, TData>(this, @event, filterExpression);
		}
	}
}
