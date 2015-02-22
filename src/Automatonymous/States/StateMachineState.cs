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
namespace Automatonymous.States
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Behaviors;
    using Contexts;
    using Events;


    public class StateMachineState<TInstance> :
        State<TInstance>,
        IEquatable<State>
        where TInstance : class
    {
        readonly Dictionary<Event, ActivityBehaviorBuilder<TInstance>> _behaviors;
        readonly Dictionary<Event, StateEventFilter<TInstance>> _ignoredEvents;
        readonly AutomatonymousStateMachine<TInstance> _machine;
        readonly string _name;
        readonly IObserver<EventRaised<TInstance>> _raisedObserver;
        readonly IObserver<EventRaising<TInstance>> _raisingObserver;

        public StateMachineState(AutomatonymousStateMachine<TInstance> machine, string name, IObserver<EventRaising<TInstance>> raisingObserver,
            IObserver<EventRaised<TInstance>> raisedObserver)
        {
            _machine = machine;
            _name = name;
            _raisingObserver = raisingObserver;
            _raisedObserver = raisedObserver;
            _behaviors = new Dictionary<Event, ActivityBehaviorBuilder<TInstance>>();
            _ignoredEvents = new Dictionary<Event, StateEventFilter<TInstance>>();

            Enter = new SimpleEvent(name + ".Enter");
            Ignore(Enter);
            Leave = new SimpleEvent(name + ".Leave");
            Ignore(Leave);

            BeforeEnter = new DataEvent<State>(name + ".BeforeEnter");
            Ignore(BeforeEnter);
            AfterLeave = new DataEvent<State>(name + ".AfterLeave");
            Ignore(AfterLeave);
        }

        public bool Equals(State other)
        {
            return string.CompareOrdinal(_name, other.Name) == 0;
        }

        public string Name
        {
            get { return _name; }
        }

        public Event Enter { get; private set; }
        public Event Leave { get; private set; }

        public Event<State> BeforeEnter { get; private set; }
        public Event<State> AfterLeave { get; private set; }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, _ =>
            {
                foreach (var behavior in _behaviors)
                {
                    behavior.Key.Accept(visitor);
                    behavior.Value.Accept(visitor);
                }
            });
        }

        async Task State<TInstance>.Raise(EventContext<TInstance> context)
        {
            ActivityBehaviorBuilder<TInstance> activities;
            if (!_behaviors.TryGetValue(context.Event, out activities))
            {
                StateEventFilter<TInstance> filter;
                if (_ignoredEvents.TryGetValue(context.Event, out filter) && filter.Filter(context))
                    return;

                await _machine.UnhandledEvent(context, this);
                return;
            }

            var notification = new EventNotification(context);
            _raisingObserver.OnNext(notification);

            var behaviorContext = new EventBehaviorContext<TInstance>(context);

            await activities.Behavior.Execute(behaviorContext);

            _raisedObserver.OnNext(notification);
        }

        async Task State<TInstance>.Raise<T>(EventContext<TInstance, T> context)
        {
            ActivityBehaviorBuilder<TInstance> activities;
            if (!_behaviors.TryGetValue(context.Event, out activities))
            {
                StateEventFilter<TInstance> filter;
                if (_ignoredEvents.TryGetValue(context.Event, out filter) && filter.Filter(context))
                    return;

                await _machine.UnhandledEvent(context, this);
                return;
            }

            var notification = new EventNotification(context);

            _raisingObserver.OnNext(notification);

            var behaviorContext = new EventBehaviorContext<TInstance, T>(context);

            await activities.Behavior.Execute(behaviorContext);

            _raisedObserver.OnNext(notification);
        }

        public void Bind(Event @event, Activity<TInstance> activity)
        {
            ActivityBehaviorBuilder<TInstance> builder;
            if (!_behaviors.TryGetValue(@event, out builder))
            {
                builder = new ActivityBehaviorBuilder<TInstance>();
                _behaviors.Add(@event, builder);
            }
            builder.Add(activity);
        }

        public void Ignore(Event @event)
        {
            _ignoredEvents[@event] = new AllStateEventFilter<TInstance>();
        }

        public void Ignore<T>(Event<T> @event, StateMachineEventFilter<TInstance, T> filter)
        {
            _ignoredEvents[@event] = new SelectedStateEventFilter<TInstance, T>(filter);
        }

        public IEnumerable<Event> Events
        {
            get { return _behaviors.Keys; }
        }

        public int CompareTo(State other)
        {
            return string.CompareOrdinal(_name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            var other = obj as State;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public static bool operator ==(State<TInstance> left, StateMachineState<TInstance> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(State<TInstance> left, StateMachineState<TInstance> right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(StateMachineState<TInstance> left, State<TInstance> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StateMachineState<TInstance> left, State<TInstance> right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(StateMachineState<TInstance> left, StateMachineState<TInstance> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StateMachineState<TInstance> left, StateMachineState<TInstance> right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("{0} (State)", _name);
        }


        class EventNotification :
            EventRaising<TInstance>,
            EventRaised<TInstance>
        {
            public EventNotification(EventContext<TInstance> context)
            {
                Instance = context.Instance;
                Event = context.Event;
            }

            public TInstance Instance { get; private set; }
            public Event Event { get; private set; }
        }
    }
}