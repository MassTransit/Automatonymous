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
namespace Automatonymous.Behaviors
{
    using System;
    using System.Collections.Generic;


    public class ActivityBehaviorBuilder<TInstance> :
        BehaviorBuilder<TInstance>
    {
        readonly List<Activity<TInstance>> _activities;
        readonly Lazy<Behavior<TInstance>> _behavior;

        public ActivityBehaviorBuilder()
        {
            _activities = new List<Activity<TInstance>>();
            _behavior = new Lazy<Behavior<TInstance>>(CreateBehavior);
        }

        public Behavior<TInstance> Behavior
        {
            get { return _behavior.Value; }
        }

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

            Behavior<TInstance> current = new LastBehavior<TInstance>(_activities[_activities.Count - 1]);

            for (int i = _activities.Count - 2; i >= 0; i--)
                current = new ActivityBehavior<TInstance>(_activities[i], current);

            return current;
        }
    }
}