namespace Automatonymous.Behaviors
{
    using System;
    using System.Collections.Generic;


    public class CatchBehaviorBuilder<TInstance> :
        BehaviorBuilder<TInstance>
    {
        readonly List<Activity<TInstance>> _activities;
        readonly Lazy<Behavior<TInstance>> _behavior;

        public CatchBehaviorBuilder()
        {
            _activities = new List<Activity<TInstance>>();
            _behavior = new Lazy<Behavior<TInstance>>(CreateBehavior);
        }

        public Behavior<TInstance> Behavior => _behavior.Value;

        public void Add(Activity<TInstance> activity)
        {
            if (_behavior.IsValueCreated)
                throw new AutomatonymousException("The behavior was already built, additional activities cannot be added.");

            _activities.Add(activity);
        }

        Behavior<TInstance> CreateBehavior()
        {
            if (_activities.Count == 0)
                return Automatonymous.Behavior.Empty<TInstance>();

            Behavior<TInstance> current = new LastFaultedBehavior<TInstance>(_activities[_activities.Count - 1]);

            for (var i = _activities.Count - 2; i >= 0; i--)
                current = new ActivityBehavior<TInstance>(_activities[i], current);

            return current;
        }
    }
}
