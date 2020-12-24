namespace Automatonymous.Graphing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Activities;
    using Events;


    public class GraphStateMachineVisitor<TInstance> :
        StateMachineVisitor
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
                var events = _events.Values
                    .Where(e => _edges.Any(edge => edge.From.Equals(e)));

                var states = _states.Values
                    .Where(s => _edges.Any(edge => edge.From.Equals(s) || edge.To.Equals(s)));

                var vertices = new HashSet<Vertex>(states.Union(events));

                var edges = _edges
                    .Where(e => vertices.Contains(e.From) && vertices.Contains(e.To));

                return new StateMachineGraph(vertices, edges);
            }
        }

        public void Visit(State state, Action<State> next)
        {
            _currentState = GetStateVertex(state);

            next(state);
        }

        public void Visit(Event @event, Action<Event> next)
        {
            _currentEvent = GetEventVertex(@event);

            _edges.Add(new Edge(_currentState, _currentEvent, _currentEvent.Title));

            next(@event);
        }

        public void Visit<TData>(Event<TData> @event, Action<Event<TData>> next)
        {
            _currentEvent = GetEventVertex(@event);

            _edges.Add(new Edge(_currentState, _currentEvent, _currentEvent.Title));

            next(@event);
        }

        public void Visit(Activity activity)
        {
            Visit(activity, x => { });
        }

        public void Visit<T>(Behavior<T> behavior)
        {
            Visit(behavior, x => { });
        }

        public void Visit<T>(Behavior<T> behavior, Action<Behavior<T>> next)
        {
            next(behavior);
        }

        public void Visit<T, TData>(Behavior<T, TData> behavior)
        {
            Visit(behavior, x => { });
        }

        public void Visit<T, TData>(Behavior<T, TData> behavior, Action<Behavior<T, TData>> next)
        {
            next(behavior);
        }

        public void Visit(Activity activity, Action<Activity> next)
        {
            if (activity is TransitionActivity<TInstance> transitionActivity)
            {
                InspectTransitionActivity(transitionActivity);
                next(activity);
                return;
            }

            if (activity is CompositeEventActivity<TInstance> compositeActivity)
            {
                InspectCompositeEventActivity(compositeActivity);
                next(activity);
                return;
            }

            var activityType = activity.GetType();
            var compensateType = activityType.GetTypeInfo().IsGenericType
                                 && activityType.GetGenericTypeDefinition() == typeof(CatchFaultActivity<,>)
                ? activityType.GetGenericArguments().Skip(1).First()
                : null;

            if (compensateType != null)
            {
                var previousEvent = _currentEvent;

                var eventType = typeof(DataEvent<>).MakeGenericType(compensateType);
                var evt = (Event)Activator.CreateInstance(eventType, compensateType.Name);
                _currentEvent = GetEventVertex(evt);

                _edges.Add(new Edge(previousEvent, _currentEvent, _currentEvent.Title));

                next(activity);

                _currentEvent = previousEvent;
                return;
            }

            next(activity);
        }

        void InspectCompositeEventActivity(CompositeEventActivity<TInstance> compositeActivity)
        {
            var previousEvent = _currentEvent;

            _currentEvent = GetEventVertex(compositeActivity.Event);

            _edges.Add(new Edge(previousEvent, _currentEvent, _currentEvent.Title));
        }

//        void InspectExceptionActivity(ExceptionActivity<TInstance> exceptionActivity, Action<Activity> next)
//        {
//            Vertex previousEvent = _currentEvent;
//
//            _currentEvent = GetEventVertex(exceptionActivity.Event);
//
//            _edges.Add(new Edge(previousEvent, _currentEvent, _currentEvent.Title));
//
//            next(exceptionActivity);
//
//            _currentEvent = previousEvent;
//        }

//        void InspectTryActivity(TryActivity<TInstance> exceptionActivity, Action<Activity> next)
//        {
//            Vertex previousEvent = _currentEvent;
//
//            next(exceptionActivity);
//
//            _currentEvent = previousEvent;
//        }

        void InspectTransitionActivity(TransitionActivity<TInstance> transitionActivity)
        {
            var targetState = GetStateVertex(transitionActivity.ToState);

            _edges.Add(new Edge(_currentEvent, targetState, _currentEvent.Title));
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

        static Vertex CreateStateVertex(State state)
        {
            return new Vertex(typeof(State), typeof(State), state.Name);
        }

        static Vertex CreateEventVertex(Event @event)
        {
            var targetType = @event
                .GetType()
                .GetInterfaces()
                .Where(x => x.GetTypeInfo().IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Event<>))
                .Select(x => x.GetGenericArguments()[0])
                .DefaultIfEmpty(typeof(Event))
                .Single();

            return new Vertex(typeof(Event), targetType, @event.Name);
        }

        static Vertex CreateEventVertex(Type exceptionType)
        {
            return new Vertex(typeof(Event), exceptionType, exceptionType.Name);
        }
    }
}
