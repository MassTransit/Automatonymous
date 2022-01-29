namespace Automatonymous.Graphing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Activities;
    using Events;


    public class GraphStateMachineVisitor<TInstance> : StateMachineVisitor
        where TInstance : class
    {
        readonly HashSet<Edge> _edges;
        readonly Dictionary<Event, Vertex> _events;
        readonly Dictionary<State, Vertex> _states;
        Vertex _currentEvent;
        Vertex _currentState;

        public GraphStateMachineVisitor()
        {
            _edges = new HashSet<Edge>();
            _states = new Dictionary<State, Vertex>();
            _events = new Dictionary<Event, Vertex>();
        }

        public StateMachineGraph Graph
        {
            get
            {
                var events = _events.Values.Where(e => _edges.Any(edge => edge.From.Equals(e)));
                var states = _states.Values.Where(s => _edges.Any(edge => edge.From.Equals(s) || edge.To.Equals(s)));
                var vertices = new HashSet<Vertex>(states.Union(events));
                return new StateMachineGraph(vertices, _edges.Where(e => vertices.Contains(e.From) && vertices.Contains(e.To)));
            }
        }

        public void Visit(State state, Action<State> next = null)
        {
            _currentState = GetStateVertex(state);
            next?.Invoke(state);
        }

        public void Visit(Event @event, Action<Event> next = null)
        {
            _currentEvent = GetEventVertex(@event);
            if (!_currentEvent.IsComposite)
            {
                _edges.Add(new Edge(_currentState, _currentEvent, _currentEvent.Title));
            }
            next?.Invoke(@event);
        }

        public void Visit<TData>(Event<TData> @event, Action<Event<TData>> next = null)
        {
            _currentEvent = GetEventVertex(@event);
            if (!_currentEvent.IsComposite)
            {
                _edges.Add(new Edge(_currentState, _currentEvent, _currentEvent.Title));
            }
            next?.Invoke(@event);
        }

        public void Visit<T>(Behavior<T> behavior, Action<Behavior<T>> next = null) => next?.Invoke(behavior);

        public void Visit<T, TData>(Behavior<T, TData> behavior, Action<Behavior<T, TData>> next = null) => next?.Invoke(behavior);

        public void Visit(Activity activity, Action<Activity> next = null)
        {
            switch (activity)
            {
                case TransitionActivity<TInstance> transitionActivity:
                    InspectTransitionActivity(transitionActivity);
                    next?.Invoke(activity);
                    return;
                case CompositeEventActivity<TInstance> compositeActivity:
                    InspectCompositeEventActivity(compositeActivity);
                    next?.Invoke(activity);
                    return;
            }

            var activityType = activity.GetType();
            var compensateType = activityType.GetTypeInfo().IsGenericType && activityType.GetGenericTypeDefinition() == typeof(CatchFaultActivity<,>)
                ? activityType.GetGenericArguments().Skip(1).First()
                : null;

            if (compensateType != null)
            {
                var previousEvent = _currentEvent;
                _currentEvent = GetEventVertex((Event)Activator.CreateInstance(typeof(DataEvent<>).MakeGenericType(compensateType), compensateType.Name));
                _edges.Add(new Edge(previousEvent, _currentEvent, _currentEvent.Title));
                next?.Invoke(activity);
                _currentEvent = previousEvent;
                return;
            }

            next?.Invoke(activity);
        }

        void InspectCompositeEventActivity(CompositeEventActivity<TInstance> compositeActivity)
        {
            var previousEvent = _currentEvent;
            _currentEvent = GetEventVertex(compositeActivity.Event);
            _edges.Add(new Edge(previousEvent, _currentEvent, _currentEvent.Title));
        }

        void InspectTransitionActivity(TransitionActivity<TInstance> transitionActivity)
        {
            var next = GetStateVertex(transitionActivity.ToState);
            _edges.Add(new Edge(_currentEvent, GetStateVertex(transitionActivity.ToState), _currentEvent.Title));
        }

        Vertex GetStateVertex(State state)
        {
            if (_states.TryGetValue(state, out var vertex))
                return vertex;

            vertex = CreateStateVertex(state);
            _states.Add(state, vertex);

            return vertex;
        }

        Vertex GetEventVertex(Event state)
        {
            if (_events.TryGetValue(state, out var vertex))
                return vertex;

            vertex = CreateEventVertex(state);
            _events.Add(state, vertex);

            return vertex;
        }

        static Vertex CreateStateVertex(State state) =>
            new Vertex(typeof(State), typeof(State), state.Name, false);

        static Vertex CreateEventVertex(Event @event) => new Vertex(typeof(Event), @event
            .GetType()
            .GetInterfaces()
            .Where(x => x.GetTypeInfo().IsGenericType)
            .Where(x => x.GetGenericTypeDefinition() == typeof(Event<>))
            .Select(x => x.GetGenericArguments()[0])
            .DefaultIfEmpty(typeof(Event))
            .Single(), @event.Name, @event.IsComposite);
    }
}
