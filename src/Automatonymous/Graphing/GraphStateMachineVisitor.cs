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
namespace Automatonymous.Graphing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Activities;
    using Internals.Caching;


    public class GraphStateMachineVisitor<TInstance> :
        StateMachineInspector
        where TInstance : class
    {
        readonly List<Edge> _edges = new List<Edge>();
        readonly Cache<Event, Vertex> _events;
        readonly Cache<State, Vertex> _states;
        Vertex _currentEvent;
        Vertex _currentState;

        public GraphStateMachineVisitor()
        {
            _states = new DictionaryCache<State, Vertex>(GetStateVertex);
            _events = new DictionaryCache<Event, Vertex>(GetEventVertex);
        }

        public StateMachineGraph Graph
        {
            get { return new StateMachineGraph(_states.Union(_events), _edges); }
        }


        public void Inspect(State state, Action<State> next)
        {
            State<TInstance> s = state.For<TInstance>();

            _currentState = _states[s];

            next(s);
        }

        public void Inspect(Event @event, Action<Event> next)
        {
            _currentEvent = _events[@event];

            _edges.Add(new Edge(_currentState, _currentEvent, _currentEvent.Title));

            next(@event);
        }

        public void Inspect<TData>(Event<TData> @event, Action<Event<TData>> next)
        {
            _currentEvent = _events[@event];

            _edges.Add(new Edge(_currentState, _currentEvent, _currentEvent.Title));

            next(@event);
        }

        public void Inspect(Activity activity, Action<Activity> next)
        {
            var transitionActivity = activity as TransitionActivity<TInstance>;
            if (transitionActivity != null)
            {
                InspectTransitionActivity(transitionActivity);
                next(activity);
                return;
            }

            var compositeActivity = activity as CompositeEventActivity<TInstance>;
            if (compositeActivity != null)
            {
                InspectCompositeEventActivity(compositeActivity);
                next(activity);
                return;
            }

            var exceptionActivity = activity as ExceptionActivity<TInstance>;
            if (exceptionActivity != null)
            {
                InspectExceptionActivity(exceptionActivity, next);
                return;
            }

            next(activity);
        }

        void InspectCompositeEventActivity(CompositeEventActivity<TInstance> compositeActivity)
        {
        }

        void InspectExceptionActivity(ExceptionActivity<TInstance> exceptionActivity, Action<Activity> next)
        {
            Vertex previousEvent = _currentEvent;

            _currentEvent = _events[exceptionActivity.Event];

            _edges.Add(new Edge(previousEvent, _currentEvent, _currentEvent.Title));

            next(exceptionActivity);

            _currentEvent = previousEvent;
        }

        void InspectTransitionActivity(TransitionActivity<TInstance> transitionActivity)
        {
            Vertex targetState = _states[transitionActivity.ToState];

            _edges.Add(new Edge(_currentEvent, targetState, _currentEvent.Title));
        }

        static Vertex GetStateVertex(State state)
        {
            return new Vertex(typeof(State), typeof(State), state.Name);
        }

        static Vertex GetEventVertex(Event @event)
        {
            Type targetType = @event
                .GetType()
                .GetTypeInfo()
                .GetInterfaces()
                .Where(x => x.GetTypeInfo().IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Event<>))
                .Select(x => x.GetTypeInfo().GetGenericArguments()[0])
                .DefaultIfEmpty(typeof(Event))
                .Single();

            return new Vertex(typeof(Event), targetType, @event.Name);
        }
    }
}