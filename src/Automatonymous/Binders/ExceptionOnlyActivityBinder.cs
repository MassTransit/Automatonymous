// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Events;


    public class ExceptionOnlyActivityBinder<TInstance> :
        ExceptionActivityBinder<TInstance>
        where TInstance : class
    {
        readonly ExceptionActivity<TInstance>[] _activities;
        readonly Event _event;
        readonly StateMachine<TInstance> _machine;

        public ExceptionOnlyActivityBinder(StateMachine<TInstance> machine, Event @event)
        {
            _activities = new ExceptionActivity<TInstance>[0];
            _machine = machine;
            _event = @event;
        }

        ExceptionOnlyActivityBinder(StateMachine<TInstance> machine, Event @event, ExceptionActivity<TInstance>[] activities,
            params ExceptionActivity<TInstance>[] appendActivity)
        {
            _activities = new ExceptionActivity<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);

            _machine = machine;
            _event = @event;
        }

        public ExceptionActivityBinder<TInstance> Handle<TException>(
            Func<EventActivityBinder<TInstance, TException>, EventActivityBinder<TInstance, TException>> handler)
            where TException : Exception
        {
            EventActivityBinder<TInstance, TException> contextBinder = new DataEventActivityBinder<TInstance, TException>(_machine,
                new DataEvent<TException>(typeof(TException).Name));

            contextBinder = handler(contextBinder);

            var handlerActivity = new ExceptionHandlerActivity<TInstance, TException>(contextBinder.GetActivities(), typeof(TException), contextBinder.Event);

            return new ExceptionOnlyActivityBinder<TInstance>(_machine, _event, _activities, handlerActivity);
        }

        public IEnumerable<ExceptionActivity<TInstance>> GetActivities()
        {
            return _activities;
        }
    }
}