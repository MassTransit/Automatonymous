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
    using System.Linq;
    using Activities;


    public class DataEventActivityBinder<TInstance, TData> :
        EventActivityBinder<TInstance, TData>
        where TInstance : class
    {
        readonly ActivityBinder<TInstance>[] _activities;
        readonly Event<TData> _event;
        readonly StateMachineEventFilter<TInstance, TData> _filter;
        readonly StateMachine<TInstance> _machine;

        public DataEventActivityBinder(StateMachine<TInstance> machine, Event<TData> @event,
            params ActivityBinder<TInstance>[] activities)
        {
            _event = @event;
            _activities = activities ?? new ActivityBinder<TInstance>[0];
            _machine = machine;
        }

        public DataEventActivityBinder(StateMachine<TInstance> machine, Event<TData> @event,
            StateMachineEventFilter<TInstance, TData> filter, params ActivityBinder<TInstance>[] activities)
        {
            _event = @event;
            _activities = activities ?? new ActivityBinder<TInstance>[0];
            _machine = machine;
            _filter = filter;
        }

        DataEventActivityBinder(StateMachine<TInstance> machine, Event<TData> @event,
            StateMachineEventFilter<TInstance, TData> filter, ActivityBinder<TInstance>[] activities,
            params ActivityBinder<TInstance>[] appendActivity)
        {
            _activities = new ActivityBinder<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);

            _event = @event;
            _machine = machine;
            _filter = filter;
        }

        Event<TData> EventActivityBinder<TInstance, TData>.Event => _event;

        EventActivityBinder<TInstance, TData> EventActivityBinder<TInstance, TData>.Add(Activity<TInstance> activity)
        {
            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter, _activities,
                CreateStateActivityBinder(new SlimActivity<TInstance, TData>(activity)));
        }

        EventActivityBinder<TInstance, TData> EventActivityBinder<TInstance, TData>.Add(Activity<TInstance, TData> activity)
        {
            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter, _activities, CreateStateActivityBinder(activity));
        }

        EventActivityBinder<TInstance, TData> EventActivityBinder<TInstance, TData>.Catch<T>(
            Func<ExceptionActivityBinder<TInstance, TData, T>, ExceptionActivityBinder<TInstance, TData, T>> activityCallback)
        {
            ExceptionActivityBinder<TInstance, TData, T> binder = new CatchExceptionActivityBinder<TInstance, TData, T>(_machine, _event);

            binder = activityCallback(binder);

            ActivityBinder<TInstance> activityBinder = new CatchActivityBinder<TInstance, T>(_event, binder);

            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter, _activities, activityBinder);
        }

        EventActivityBinder<TInstance, TData> EventActivityBinder<TInstance, TData>.If(StateMachineCondition<TInstance, TData> condition,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> activityCallback)
        {
            return IfElse(condition, activityCallback, _ => _);
        }

        EventActivityBinder<TInstance, TData> EventActivityBinder<TInstance, TData>.IfAsync(StateMachineAsyncCondition<TInstance, TData> condition,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> activityCallback)
        {
            return IfElseAsync(condition, activityCallback, _ => _);
        }

        public EventActivityBinder<TInstance, TData> IfElse(StateMachineCondition<TInstance, TData> condition,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> thenActivityCallback,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> elseActivityCallback)
        {
            var thenBinder = GetBinder(thenActivityCallback);
            var elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalActivityBinder<TInstance, TData>(_event, condition, thenBinder, elseBinder);

            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter, _activities, conditionBinder);
        }

        public EventActivityBinder<TInstance, TData> IfElseAsync(StateMachineAsyncCondition<TInstance, TData> condition,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> thenActivityCallback,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> elseActivityCallback)
        {
            var thenBinder = GetBinder(thenActivityCallback);
            var elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalActivityBinder<TInstance, TData>(_event, condition, thenBinder, elseBinder);

            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter, _activities, conditionBinder);
        }

        private EventActivityBinder<TInstance, TData> GetBinder(Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> activityCallback)
        {
            EventActivityBinder<TInstance, TData> binder = new DataEventActivityBinder<TInstance, TData>(_machine, _event);

            return activityCallback(binder);
        }

        StateMachine<TInstance> EventActivityBinder<TInstance, TData>.StateMachine => _machine;

        public IEnumerable<ActivityBinder<TInstance>> GetStateActivityBinders()
        {
            if (_filter != null)
                return Enumerable.Repeat(CreateConditionalActivityBinder(), 1);

            return _activities;
        }

        ActivityBinder<TInstance> CreateStateActivityBinder(Activity<TInstance, TData> activity)
        {
            var converterActivity = new DataConverterActivity<TInstance, TData>(activity);

            return new ExecuteActivityBinder<TInstance>(_event, converterActivity);
        }

        ActivityBinder<TInstance> CreateConditionalActivityBinder()
        {
            EventActivityBinder<TInstance, TData> thenBinder = new DataEventActivityBinder<TInstance, TData>(_machine, _event, _activities);
            EventActivityBinder<TInstance, TData> elseBinder = new DataEventActivityBinder<TInstance, TData>(_machine, _event);

            var conditionBinder = new ConditionalActivityBinder<TInstance, TData>(_event, context => _filter(context), thenBinder, elseBinder);

            return conditionBinder;
        }
    }
}