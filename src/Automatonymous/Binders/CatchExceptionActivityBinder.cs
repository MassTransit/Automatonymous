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


    public class CatchExceptionActivityBinder<TInstance, TException> :
        ExceptionActivityBinder<TInstance, TException>
        where TInstance : class
        where TException : Exception
    {
        readonly ActivityBinder<TInstance>[] _activities;
        readonly Event _event;
        readonly StateMachine<TInstance> _machine;

        public CatchExceptionActivityBinder(StateMachine<TInstance> machine, Event @event)
        {
            _activities = new ActivityBinder<TInstance>[0];
            _machine = machine;
            _event = @event;
        }

        CatchExceptionActivityBinder(StateMachine<TInstance> machine, Event @event,
            ActivityBinder<TInstance>[] activities,
            params ActivityBinder<TInstance>[] appendActivity)
        {
            _activities = new ActivityBinder<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);

            _machine = machine;
            _event = @event;
        }

        public IEnumerable<ActivityBinder<TInstance>> GetStateActivityBinders()
        {
            return _activities;
        }

        public StateMachine<TInstance> StateMachine
        {
            get { return _machine; }
        }

        public Event Event
        {
            get { return _event; }
        }

        public ExceptionActivityBinder<TInstance, TException> Add(Activity<TInstance> activity)
        {
            ActivityBinder<TInstance> activityBinder = new ExecuteActivityBinder<TInstance>(_event, activity);

            return new CatchExceptionActivityBinder<TInstance, TException>(_machine, _event, _activities, activityBinder);
        }

        public ExceptionActivityBinder<TInstance, TException> Catch<T>(
            Func<ExceptionActivityBinder<TInstance, T>, ExceptionActivityBinder<TInstance, T>> activityCallback) 
            where T : Exception
        {
            ExceptionActivityBinder<TInstance, T> binder = new CatchExceptionActivityBinder<TInstance, T>(_machine, _event);

            binder = activityCallback(binder);

            ActivityBinder<TInstance> activityBinder = new CatchActivityBinder<TInstance, T>(_event, binder);

            return new CatchExceptionActivityBinder<TInstance, TException>(_machine, _event, _activities, activityBinder);
        }
    }

    public class CatchExceptionActivityBinder<TInstance, TData, TException> :
        ExceptionActivityBinder<TInstance, TData, TException>
        where TInstance : class
        where TException : Exception
    {
        readonly ActivityBinder<TInstance>[] _activities;
        readonly Event<TData> _event;
        readonly StateMachine<TInstance> _machine;

        public CatchExceptionActivityBinder(StateMachine<TInstance> machine, Event<TData> @event)
        {
            _activities = new ActivityBinder<TInstance>[0];
            _machine = machine;
            _event = @event;
        }

        CatchExceptionActivityBinder(StateMachine<TInstance> machine, Event<TData> @event,
            ActivityBinder<TInstance>[] activities,
            params ActivityBinder<TInstance>[] appendActivity)
        {
            _activities = new ActivityBinder<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);

            _machine = machine;
            _event = @event;
        }

        public IEnumerable<ActivityBinder<TInstance>> GetStateActivityBinders()
        {
            return _activities;
        }

        public StateMachine<TInstance> StateMachine
        {
            get { return _machine; }
        }

        public Event<TData> Event
        {
            get { return _event; }
        }

        public ExceptionActivityBinder<TInstance, TData, TException> Add(Activity<TInstance> activity)
        {
            ActivityBinder<TInstance> activityBinder = new ExecuteActivityBinder<TInstance>(_event, activity);

            return new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, _event, _activities, activityBinder);
        }

        public ExceptionActivityBinder<TInstance, TData, TException> Add(Activity<TInstance, TData> activity)
        {
            var converterActivity = new DataConverterActivity<TInstance, TData>(activity);

            ActivityBinder<TInstance> activityBinder = new ExecuteActivityBinder<TInstance>(_event, converterActivity);

            return new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, _event, _activities, activityBinder);
        }

        public ExceptionActivityBinder<TInstance, TData, TException> Catch<T>(
            Func<ExceptionActivityBinder<TInstance, TData, T>, ExceptionActivityBinder<TInstance, TData, T>> activityCallback) where T : Exception
        {
            ExceptionActivityBinder<TInstance, TData, T> binder = new CatchExceptionActivityBinder<TInstance, TData, T>(_machine, _event);

            binder = activityCallback(binder);

            ActivityBinder<TInstance> activityBinder = new CatchActivityBinder<TInstance, T>(_event, binder);

            return new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, _event, _activities, activityBinder);
        }
    }
}