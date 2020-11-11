namespace Automatonymous.States
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Behaviors;
    using Contexts;
    using Events;
    using GreenPipes;


    public class StateMachineState<TInstance> :
        State<TInstance>,
        IEquatable<State>
        where TInstance : class
    {
        readonly Dictionary<Event, ActivityBehaviorBuilder<TInstance>> _behaviors;
        readonly Dictionary<Event, StateEventFilter<TInstance>> _ignoredEvents;
        readonly AutomatonymousStateMachine<TInstance> _machine;
        readonly string _name;
        readonly EventObserver<TInstance> _observer;
        readonly HashSet<State<TInstance>> _subStates;
        State<TInstance> _superState;

        public StateMachineState(AutomatonymousStateMachine<TInstance> machine, string name,
            EventObserver<TInstance> observer, State<TInstance> superState = null)
        {
            _machine = machine;
            _name = name;
            _observer = observer;

            _behaviors = new Dictionary<Event, ActivityBehaviorBuilder<TInstance>>();
            _ignoredEvents = new Dictionary<Event, StateEventFilter<TInstance>>();

            Enter = new TriggerEvent(name + ".Enter");
            Ignore(Enter);
            Leave = new TriggerEvent(name + ".Leave");
            Ignore(Leave);

            BeforeEnter = new DataEvent<State>(name + ".BeforeEnter");
            Ignore(BeforeEnter);
            AfterLeave = new DataEvent<State>(name + ".AfterLeave");
            Ignore(AfterLeave);

            _subStates = new HashSet<State<TInstance>>();

            _superState = superState;
            superState?.AddSubstate(this);
        }

        public bool Equals(State other)
        {
            return string.CompareOrdinal(_name, other?.Name ?? "") == 0;
        }

        public State<TInstance> SuperState { get; }
        public string Name => _name;
        public Event Enter { get; }
        public Event Leave { get; }
        public Event<State> BeforeEnter { get; }
        public Event<State> AfterLeave { get; }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, _ =>
            {
                foreach (var behavior in _behaviors)
                {
                    behavior.Key.Accept(visitor);
                    behavior.Value.Behavior.Accept(visitor);
                }
            });
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("state");
            scope.Add("name", _name);

            if (_subStates.Any())
            {
                var subStateScope = scope.CreateScope("substates");
                foreach (var subState in _subStates)
                {
                    subStateScope.Add("name", subState.Name);
                }
            }

            if (_behaviors.Any())
            {
                foreach (var behavior in _behaviors)
                {
                    var eventScope = scope.CreateScope("event");
                    behavior.Key.Probe(eventScope);

                    behavior.Value.Behavior.Probe(eventScope.CreateScope("behavior"));
                }
            }

            var ignored = _ignoredEvents.Where(x => IsRealEvent(x.Key)).ToList();
            if (ignored.Any())
            {
                foreach (var ignoredEvent in ignored)
                {
                    ignoredEvent.Key.Probe(scope.CreateScope("event-ignored"));
                }
            }
        }

        async Task State<TInstance>.Raise(EventContext<TInstance> context)
        {
            if (!_behaviors.TryGetValue(context.Event, out var activities))
            {
                if (_ignoredEvents.TryGetValue(context.Event, out var filter) && filter.Filter(context))
                    return;

                if (_superState != null)
                {
                    try
                    {
                        await _superState.Raise(context).ConfigureAwait(false);
                        return;
                    }
                    catch (UnhandledEventException)
                    {
                        // the exception is better if it's from the substate
                    }
                }

                await _machine.UnhandledEvent(context, this).ConfigureAwait(false);
                return;
            }

            try
            {
                await _observer.PreExecute(context).ConfigureAwait(false);

                await activities.Behavior.Execute(new EventBehaviorContext<TInstance>(context)).ConfigureAwait(false);

                await _observer.PostExecute(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _observer.ExecuteFault(context, ex).ConfigureAwait(false);

                throw;
            }
        }

        async Task State<TInstance>.Raise<T>(EventContext<TInstance, T> context)
        {
            if (!_behaviors.TryGetValue(context.Event, out var activities))
            {
                if (_ignoredEvents.TryGetValue(context.Event, out var filter) && filter.Filter(context))
                    return;

                if (_superState != null)
                {
                    try
                    {
                        await _superState.Raise(context).ConfigureAwait(false);
                        return;
                    }
                    catch (UnhandledEventException)
                    {
                        // the exception is better if it's from the substate
                    }
                }

                await _machine.UnhandledEvent(context, this).ConfigureAwait(false);
                return;
            }

            try
            {
                await _observer.PreExecute(context).ConfigureAwait(false);

                await activities.Behavior.Execute(new EventBehaviorContext<TInstance, T>(context)).ConfigureAwait(false);

                await _observer.PostExecute(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _observer.ExecuteFault(context, ex).ConfigureAwait(false);

                throw;
            }
        }

        public void Bind(Event @event, Activity<TInstance> activity)
        {
            if (!_behaviors.TryGetValue(@event, out var builder))
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

        public void AddSubstate(State<TInstance> subState)
        {
            if (subState == null)
                throw new ArgumentNullException(nameof(subState));

            if (_name.Equals(subState.Name))
                throw new ArgumentException("A state cannot be a substate of itself", nameof(subState));

            _subStates.Add(subState);
        }

        public bool HasState(State<TInstance> state)
        {
            return _name.Equals(state.Name) || _subStates.Any(s => s.HasState(state));
        }

        public bool IsStateOf(State<TInstance> state)
        {
            return _name.Equals(state.Name) || (_superState != null && _superState.IsStateOf(state));
        }

        public IEnumerable<Event> Events
        {
            get
            {
                if (_superState != null)
                    return _superState.Events.Union(GetStateEvents()).Distinct();

                return GetStateEvents();
            }
        }

        public int CompareTo(State other)
        {
            return string.CompareOrdinal(_name, other.Name);
        }

        bool IsRealEvent(Event @event)
        {
            if (Equals(@event, Enter) || Equals(@event, Leave) || Equals(@event, BeforeEnter) || Equals(@event, AfterLeave))
                return false;

            return true;
        }

        IEnumerable<Event> GetStateEvents()
        {
            return _behaviors.Keys
                .Union(_ignoredEvents.Keys)
                .Where(IsRealEvent)
                .Distinct();
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
            return _name?.GetHashCode() ?? 0;
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
            return $"{_name} (State)";
        }
    }
}
