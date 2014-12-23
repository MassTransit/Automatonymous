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
namespace Automatonymous.Graphing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Activities;


    public class GraphStateMachineVisitor<TInstance> :
        StateMachineInspector
        where TInstance : class
    {
        readonly List<Edge> _edges = new List<Edge>();
        readonly Dictionary<Event, Vertex> _events;
        readonly Dictionary<State, Vertex> _states;
        Vertex _currentEvent;
        Vertex _currentState;

        public GraphStateMachineVisitor()
        {
            _states = new Dictionary<State, Vertex>();
            _events = new Dictionary<Event, Vertex>();
        }

        public StateMachineGraph Graph
        {
            get { return new StateMachineGraph(_states.Values.Union(_events.Values), _edges); }
        }

        public void Inspect(State state, Action<State> next)
        {
            State<TInstance> s = state.For<TInstance>();

            _currentState = GetStateVertex(s);

            next(s);
        }

        public void Inspect(Event @event, Action<Event> next)
        {
            _currentEvent = GetEventVertex(@event);

            _edges.Add(new Edge(_currentState, _currentEvent, _currentEvent.Title));

            next(@event);
        }

        public void Inspect<TData>(Event<TData> @event, Action<Event<TData>> next)
        {
            _currentEvent = GetEventVertex(@event);

            _edges.Add(new Edge(_currentState, _currentEvent, _currentEvent.Title));

            next(@event);
        }

        public void Inspect(Activity activity)
        {
        }

        public void Inspect<T>(Behavior<T> behavior)
        {
        }

        public void Inspect<T>(Behavior<T> behavior, Action<Behavior<T>> next)
        {
        }

        public void Inspect<T, TData>(Behavior<T, TData> behavior)
        {
        }

        public void Inspect<T, TData>(Behavior<T, TData> behavior, Action<Behavior<T, TData>> next)
        {
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

            _currentEvent = GetEventVertex(exceptionActivity.Event);

            _edges.Add(new Edge(previousEvent, _currentEvent, _currentEvent.Title));

            next(exceptionActivity);

            _currentEvent = previousEvent;
        }

        void InspectTransitionActivity(TransitionActivity<TInstance> transitionActivity)
        {
            Vertex targetState = GetStateVertex(transitionActivity.ToState);

            _edges.Add(new Edge(_currentEvent, targetState, _currentEvent.Title));
        }

        Vertex GetStateVertex(State state)
        {
            Vertex vertex;
            if (_states.TryGetValue(state, out vertex))
                return vertex;

            vertex = CreateStateVertex(state);
            _states.Add(state, vertex);

            return vertex;
        }

        Vertex GetEventVertex(Event state)
        {
            Vertex vertex;
            if (_events.TryGetValue(state, out vertex))
                return vertex;

            vertex = CreateEventVertex(state);
            _events.Add(state, vertex);

            return vertex;
        }

        static Vertex CreateStateVertex(State state)
        {
            return new Vertex(typeof(State), typeof(State), state.Name);
        }

        static Vertex CreateEventVertex(Event @event)
        {
            Type targetType = @event
                .GetType()
                .GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Event<>))
                .Select(x => x.GetGenericArguments()[0])
                .DefaultIfEmpty(typeof(Event))
                .Single();

            return new Vertex(typeof(Event), targetType, @event.Name);
        }
    }
}