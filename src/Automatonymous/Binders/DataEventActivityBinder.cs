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
    using Activities;


    public class DataEventActivityBinder<TInstance, TData> :
        EventActivityBinder<TInstance, TData>
        where TInstance : class
    {
        readonly StateActivityBinder<TInstance>[] _activities;
        readonly Event<TData> _event;
        readonly StateMachineEventFilter<TInstance, TData> _filter;
        readonly StateMachine<TInstance> _machine;

        public DataEventActivityBinder(StateMachine<TInstance> machine, Event<TData> @event)
        {
            _event = @event;
            _activities = new StateActivityBinder<TInstance>[0];
            _machine = machine;
        }

        public DataEventActivityBinder(StateMachine<TInstance> machine, Event<TData> @event,
            StateMachineEventFilter<TInstance, TData> filter)
        {
            _event = @event;
            _activities = new StateActivityBinder<TInstance>[0];
            _machine = machine;
            _filter = filter;
        }

        public DataEventActivityBinder(StateMachine<TInstance> machine, Event<TData> @event,
            StateMachineEventFilter<TInstance, TData> filter, StateActivityBinder<TInstance>[] activities,
            params StateActivityBinder<TInstance>[] appendActivity)
        {
            _activities = new StateActivityBinder<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);

            _event = @event;
            _machine = machine;
            _filter = filter;
        }

        public StateMachineEventFilter<TInstance, TData> Filter
        {
            get { return _filter; }
        }

        Event<TData> EventActivityBinder<TInstance, TData>.Event
        {
            get { return _event; }
        }

        EventActivityBinder<TInstance, TData> EventActivityBinder<TInstance, TData>.Add(Activity<TInstance> activity)
        {
            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter, _activities,
                CreateStateActivityBinder(new SlimActivity<TInstance, TData>(activity)));
        }

        EventActivityBinder<TInstance, TData> EventActivityBinder<TInstance, TData>.Add(Activity<TInstance, TData> activity)
        {
            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter, _activities,
                CreateStateActivityBinder(activity));
        }

        public EventActivityBinder<TInstance, TData> Ignore()
        {
            StateActivityBinder<TInstance> activityBinder = new IgnoreEventStateActivityBinder<TInstance>(_event);

            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter, _activities, activityBinder);
        }

        public EventActivityBinder<TInstance, TData> Ignore(StateMachineEventFilter<TInstance, TData> filter)
        {
            StateActivityBinder<TInstance> activityBinder = new IgnoreEventStateActivityBinder<TInstance, TData>(_event, filter);

            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, filter, _activities, activityBinder);
        }

        StateMachine<TInstance> EventActivityBinder<TInstance, TData>.StateMachine
        {
            get { return _machine; }
        }

        public IEnumerable<StateActivityBinder<TInstance>> GetStateActivityBinders()
        {
            return _activities;
        }

        StateActivityBinder<TInstance> CreateStateActivityBinder(Activity<TInstance, TData> activity)
        {
            if (_filter == null)
                return CreateEventActivity(activity);

            return CreateConditionalEventActivity(activity);
        }

        StateActivityBinder<TInstance> CreateConditionalEventActivity(Activity<TInstance, TData> activity)
        {
            var conditionalEventActivity = new ConditionalEventActivity<TInstance, TData>(activity, _filter);

            return CreateEventActivity(conditionalEventActivity);
        }

        StateActivityBinder<TInstance> CreateEventActivity(Activity<TInstance, TData> activity)
        {
            var converterActivity = new DataConverterActivity<TInstance, TData>(activity);

            return new EventStateActivityBinder<TInstance>(_event, converterActivity);
        }
    }
}