// Copyright 2011 Chris Patterson, Dru Sellers
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
    using Impl;
    using Impl.Activities;
    using Internal.Caching;


    public abstract class StateMachine<TInstance>
        where TInstance : StateMachineInstance
    {
        readonly Cache<string, Event> _eventCache;
        readonly Activity<TInstance> _initialActivity;
        readonly Cache<string, StateImpl<TInstance>> _stateCache;

        protected StateMachine()
        {
            _stateCache = new DictionaryCache<string, StateImpl<TInstance>>();
            _eventCache = new DictionaryCache<string, Event>();

            State(() => Initial);
            State(() => Completed);

            _initialActivity = new TransitionActivity<TInstance>(Initial);
        }

        public State Initial { get; private set; }
        public State Completed { get; private set; }

        public void RaiseEvent(TInstance instance, Event @event)
        {
            WithInstance(instance, x => { _stateCache[instance.CurrentState.Name].Raise(instance, @event, null); });
        }

        void WithInstance(TInstance instance, Action<TInstance> callback)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            if (instance.CurrentState == null)
                _initialActivity.Execute(instance);

            callback(instance);
        }

        protected void Event(Expression<Func<Event>> propertyExpression)
        {
            PropertyInfo property = GetPropertyInfo(propertyExpression);

            string name = property.Name;

            var @event = new SimpleEvent<TInstance>(name);

            property.SetValue(this, @event, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                              null, null);

            _eventCache[name] = @event;
        }

        protected void Event<T>(Expression<Func<Event<T>>> propertyExpression)
        {
            PropertyInfo property = GetPropertyInfo(propertyExpression);

            string name = property.Name;

            var @event = new DataEvent<TInstance, T>(name);

            property.SetValue(this, @event, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                              null, null);

            _eventCache[name] = @event;
        }

        protected void State(Expression<Func<State>> propertyExpression)
        {
            PropertyInfo property = GetPropertyInfo(propertyExpression);

            string name = property.Name;

            var state = new StateImpl<TInstance>(name);

            property.SetValue(this, state, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                              null, null);

            _stateCache[name] = state;
        }

        static PropertyInfo GetPropertyInfo<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Must be a member expression");

            if (memberExpression.Member.MemberType != MemberTypes.Property)
                throw new ArgumentException("Must be a property expression");

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("Not a property, wtF?");

            return property;
        }

        protected void During(State state, params IEnumerable<EventActivity<TInstance>>[] activities)
        {
            State<TInstance> activityState = state.For<TInstance>();

            foreach (var activity in activities.SelectMany(x => x))
                activityState.Bind(activity);
        }

        protected void Initially(params EventActivity<TInstance>[] args)
        {
            During(_stateCache["Initial"], args);
        }

        protected IEnumerable<EventActivity<TInstance>> When(Event @event, params Activity<TInstance>[] activities)
        {
            return activities.Select(activity => new EventActivityImpl<TInstance>(@event, activity));
        }

        protected int When<TData>(Event<TData> @event, params Activity<TInstance>[] activities)
        {
            return 0;
        }

        protected Activity<TInstance> TransitionTo(State toState)
        {
            State<TInstance> state = toState.For<TInstance>();

            var activity = new TransitionActivity<TInstance>(state);

            return activity;
        }


        protected Activity<TInstance> Then(Action<TInstance> action)
        {
            return new ActionEventActivity<TInstance>(action);
        }
    }
}