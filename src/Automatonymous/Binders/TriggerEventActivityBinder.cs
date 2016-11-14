// Copyright 2011-2016 Chris Patterson, Dru Sellers
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


    public class TriggerEventActivityBinder<TInstance> :
        EventActivityBinder<TInstance>
        where TInstance : class
    {
        readonly ActivityBinder<TInstance>[] _activities;
        readonly Event _event;
        readonly StateMachine<TInstance> _machine;

        public TriggerEventActivityBinder(StateMachine<TInstance> machine, Event @event, params ActivityBinder<TInstance>[] activities)
        {
            _event = @event;
            _machine = machine;
            _activities = activities ?? new ActivityBinder<TInstance>[0];
        }

        TriggerEventActivityBinder(StateMachine<TInstance> machine, Event @event, ActivityBinder<TInstance>[] activities,
            params ActivityBinder<TInstance>[] appendActivity)
        {
            _event = @event;
            _machine = machine;

            _activities = new ActivityBinder<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);
        }

        Event EventActivityBinder<TInstance>.Event => _event;

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.Add(Activity<TInstance> activity)
        {
            ActivityBinder<TInstance> activityBinder = new ExecuteActivityBinder<TInstance>(_event, activity);

            return new TriggerEventActivityBinder<TInstance>(_machine, _event, _activities, activityBinder);
        }

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.Catch<T>(
            Func<ExceptionActivityBinder<TInstance, T>, ExceptionActivityBinder<TInstance, T>> activityCallback)
        {
            ExceptionActivityBinder<TInstance, T> binder = new CatchExceptionActivityBinder<TInstance, T>(_machine, _event);

            binder = activityCallback(binder);

            ActivityBinder<TInstance> activityBinder = new CatchActivityBinder<TInstance, T>(_event, binder);

            return new TriggerEventActivityBinder<TInstance>(_machine, _event, _activities, activityBinder);
        }

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.If(StateMachineCondition<TInstance> condition,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            EventActivityBinder<TInstance> binder = new TriggerEventActivityBinder<TInstance>(_machine, _event);

            binder = activityCallback(binder);

            var conditionBinder = new ConditionalActivityBinder<TInstance>(_event, condition, binder);

            return new TriggerEventActivityBinder<TInstance>(_machine, _event, _activities, conditionBinder);
        }

        StateMachine<TInstance> EventActivityBinder<TInstance>.StateMachine => _machine;

        public IEnumerable<ActivityBinder<TInstance>> GetStateActivityBinders()
        {
            return _activities;
        }
    }
}