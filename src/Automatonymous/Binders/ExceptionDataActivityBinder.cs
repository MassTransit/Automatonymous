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
    using System.Linq;
    using Activities;
    using Events;


    /// <summary>
    /// Binds an exception handler with a data event
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class ExceptionDataActivityBinder<TInstance, TData> :
        ExceptionActivityBinder<TInstance, TData>
        where TInstance : class
    {
        readonly ExceptionActivity<TInstance, TData>[] _activities;
        readonly Event<TData> _event;
        readonly StateMachine<TInstance> _machine;

        public ExceptionDataActivityBinder(StateMachine<TInstance> machine, Event<TData> @event)
        {
            _activities = new ExceptionActivity<TInstance, TData>[0];
            _machine = machine;
            _event = @event;
        }

        ExceptionDataActivityBinder(StateMachine<TInstance> machine, Event<TData> @event, ExceptionActivity<TInstance, TData>[] activities,
            params ExceptionActivity<TInstance, TData>[] appendActivity)
        {
            _activities = new ExceptionActivity<TInstance, TData>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);

            _machine = machine;
            _event = @event;
        }

        public ExceptionActivityBinder<TInstance, TData> Handle<TException>(
            Func<EventActivityBinder<TInstance, Tuple<TData, TException>>, EventActivityBinder<TInstance, Tuple<TData, TException>>> handler)
            where TException : Exception
        {
            EventActivityBinder<TInstance, Tuple<TData, TException>> contextBinder = new DataEventActivityBinder
                <TInstance, Tuple<TData, TException>>(_machine,
                    new DataEvent<Tuple<TData, TException>>(typeof(TData).Name + "." + typeof(TException).Name));

            contextBinder = handler(contextBinder);

            var handlerActivity = new ExceptionHandlerActivity<TInstance, TData, TException>(contextBinder.GetStateActivityBinders(), typeof(TException));

            return new ExceptionDataActivityBinder<TInstance, TData>(_machine, _event, _activities, handlerActivity);
        }

        public IEnumerable<ExceptionActivity<TInstance, TData>> GetActivities()
        {
            return _activities;
        }
    }
}