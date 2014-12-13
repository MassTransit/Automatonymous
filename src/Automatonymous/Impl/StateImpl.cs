// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Automatonymous.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public class StateImpl<TInstance> :
        State<TInstance>,
        IEquatable<State>
        where TInstance : class
    {
        readonly Dictionary<Event, BehaviorBuilder> _behaviors;
        readonly string _name;
        readonly IObserver<EventRaised<TInstance>> _raisedObserver;
        readonly IObserver<EventRaising<TInstance>> _raisingObserver;

        public StateImpl(string name, IObserver<EventRaising<TInstance>> raisingObserver,
            IObserver<EventRaised<TInstance>> raisedObserver)
        {
            _name = name;
            _raisingObserver = raisingObserver;
            _raisedObserver = raisedObserver;

            Enter = new SimpleEvent(name + ".Enter");
            Leave = new SimpleEvent(name + ".Leave");

            BeforeEnter = new DataEvent<State>(name + ".BeforeEnter");
            AfterLeave = new DataEvent<State>(name + ".AfterLeave");

            _behaviors = new Dictionary<Event, BehaviorBuilder>();
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

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, _ =>
            {
                foreach (var behavior in _behaviors)
                {
                    behavior.Key.Accept(inspector);
                    behavior.Value.Accept(inspector);
                }
            });
        }

        async Task State<TInstance>.Raise<T>(EventContext<TInstance, T> context)
        {
            BehaviorBuilder activities;
            if (!_behaviors.TryGetValue(context.Event, out activities))
                throw new AutomatonymousException("The event is not valid in the current state: " + context.Event.Name);

            var notification = new EventNotification(context);

            _raisingObserver.OnNext(notification);

            var behaviorContext = new BehaviorContextImpl<TInstance, T>(context);

            await activities.GetBehavior.Execute(behaviorContext);

            _raisedObserver.OnNext(notification);
        }

        public void Bind(EventActivity<TInstance> activity)
        {
            _behaviors[activity.Event].Add(activity);
        }

        public IEnumerable<Event> Events
        {
            get { return _behaviors.Keys; }
        }

        public int CompareTo(State other)
        {
            return string.CompareOrdinal(_name, other.Name);
        }

        async Task State<TInstance>.Raise(EventContext<TInstance> context)
        {
            BehaviorBuilder activities;
            if (!_behaviors.TryGetValue(context.Event, out activities))
                throw new AutomatonymousException("The event is not valid in the current state: " + context.Event.Name);

            var notification = new EventNotification(context);

            _raisingObserver.OnNext(notification);

            var behaviorContext = new BehaviorContextImpl<TInstance>(context);

            await activities.GetBehavior.Execute(behaviorContext);

            _raisedObserver.OnNext(notification);
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

        public static bool operator ==(State<TInstance> left, StateImpl<TInstance> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(State<TInstance> left, StateImpl<TInstance> right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(StateImpl<TInstance> left, State<TInstance> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StateImpl<TInstance> left, State<TInstance> right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(StateImpl<TInstance> left, StateImpl<TInstance> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StateImpl<TInstance> left, StateImpl<TInstance> right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("{0} (State)", _name);
        }


        class BehaviorBuilder
        {
            readonly List<Activity<TInstance>> _activities;
            readonly Lazy<Behavior<TInstance>> _behavior;

            public BehaviorBuilder()
            {
                _activities = new List<Activity<TInstance>>();
                _behavior = new Lazy<Behavior<TInstance>>(CreateBehavior);
            }

            public Behavior<TInstance> GetBehavior
            {
                get { return _behavior.Value; }
            }

            Behavior<TInstance> CreateBehavior()
            {
                if (_activities.Count == 0)
                    return Behavior.Empty<TInstance>();

                Behavior<TInstance> current = new LastBehavior<TInstance>(_activities[_activities.Count - 1]);

                for (int i = _activities.Count - 2; i >= 0; i--)
                    current = new ActivityBehavior<TInstance>(_activities[i], current);

                return current;
            }

            public void Add(Activity<TInstance> activity)
            {
                if (_behavior.IsValueCreated)
                    throw new AutomatonymousException("The behavior was already built, additional activities cannot be added.");

                _activities.Add(activity);
            }

            public void Accept(StateMachineInspector inspector)
            {
                foreach (var activity in _activities)
                    activity.Accept(inspector);
            }
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