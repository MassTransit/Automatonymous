// Copyright 2011-2014 Chris Patterson, Dru Sellers
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
    using System.Threading;
    using System.Threading.Tasks;
    using Internals.Caching;


    public class StateImpl<TInstance> :
        State<TInstance>,
        IEquatable<State>
        where TInstance : class
    {
        readonly Cache<Event, List<Activity<TInstance>>> _activityCache;
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

            _activityCache = new DictionaryCache<Event, List<Activity<TInstance>>>(x => new List<Activity<TInstance>>());
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
            inspector.Inspect(this, _ => _activityCache.Each((key, value) =>
            {
                key.Accept(inspector);
                value.ForEach(activity => activity.Accept(inspector));
            }));
        }


        Task State<TInstance>.Raise(TInstance instance, Event @event, CancellationToken cancellationToken)
        {
            return Raise(instance, @event, (a, i) => a.Execute(i, cancellationToken));
        }

        Task State<TInstance>.Raise<TData>(TInstance instance, Event<TData> @event, TData value, CancellationToken cancellationToken)
        {
            return Raise(instance, @event, (a, i) => a.Execute(i, value, cancellationToken));
        }

        public void Bind(EventActivity<TInstance> activity)
        {
            _activityCache[activity.Event].Add(activity);
        }

        public IEnumerable<Event> Events
        {
            get { return _activityCache.GetAllKeys(); }
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


        async Task Raise<TEvent>(TInstance instance, TEvent @event, Func<Activity<TInstance>, TInstance, Task> callback)
            where TEvent : Event
        {
            List<Activity<TInstance>> activities;
            if (!_activityCache.TryGetValue(@event, out activities))
                return;

            var notification = new EventNotification(instance, @event);

            _raisingObserver.OnNext(notification);

            foreach (var activity in activities)
                await callback(activity, instance);

            _raisedObserver.OnNext(notification);
        }

        public override string ToString()
        {
            return string.Format("{0} (State)", _name);
        }


        class EventNotification :
            EventRaising<TInstance>,
            EventRaised<TInstance>
        {
            public EventNotification(TInstance instance, Event @event)
            {
                Instance = instance;
                Event = @event;
            }

            public TInstance Instance { get; private set; }
            public Event Event { get; private set; }
        }
    }
}