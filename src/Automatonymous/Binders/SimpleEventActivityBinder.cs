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
namespace Automatonymous.Binders
{
    using System;
    using System.Collections.Generic;


    public class SimpleEventActivityBinder<TInstance> :
        EventActivityBinder<TInstance>
        where TInstance : class
    {
        readonly StateActivityBinder<TInstance>[] _activities;
        readonly Event _event;
        readonly StateMachine<TInstance> _machine;

        public SimpleEventActivityBinder(StateMachine<TInstance> machine, Event @event)
        {
            _event = @event;
            _machine = machine;
            _activities = new StateActivityBinder<TInstance>[0];
        }

        SimpleEventActivityBinder(StateMachine<TInstance> machine, Event @event, StateActivityBinder<TInstance>[] activities,
            params StateActivityBinder<TInstance>[] appendActivity)
        {
            _event = @event;
            _machine = machine;

            _activities = new StateActivityBinder<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);
        }

        Event EventActivityBinder<TInstance>.Event
        {
            get { return _event; }
        }

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.Add(Activity<TInstance> activity)
        {
            StateActivityBinder<TInstance> activityBinder = new EventStateActivityBinder<TInstance>(_event, activity);

            return new SimpleEventActivityBinder<TInstance>(_machine, _event, _activities, activityBinder);
        }

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.Ignore()
        {
            StateActivityBinder<TInstance> activityBinder = new IgnoreEventStateActivityBinder<TInstance>(_event);

            return new SimpleEventActivityBinder<TInstance>(_machine, _event, _activities, activityBinder);
        }

        StateMachine<TInstance> EventActivityBinder<TInstance>.StateMachine
        {
            get { return _machine; }
        }

        public IEnumerable<StateActivityBinder<TInstance>> GetStateActivityBinders()
        {
            return _activities;
        }
    }
}