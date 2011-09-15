// Copyright 2011 Chris Patterson, Dru Sellers
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
namespace Automatonymous.Impl.Activities
{
    using System;
    using System.Collections.Generic;


    public class ExceptionHandlerActivity<TInstance, TException> :
        ExceptionActivity<TInstance>
        where TInstance : StateMachineInstance
        where TException : class
    {
        readonly List<Activity<TInstance>> _activities;
        readonly Type _exceptionType;
        Event<TException> _event;

        public ExceptionHandlerActivity(IEnumerable<EventActivity<TInstance>> activities, Type exceptionType,
                                        Event<TException> @event)
        {
            _exceptionType = exceptionType;
            _event = @event;
            _activities = new List<Activity<TInstance>>(activities);
        }

        public void Execute(TInstance instance, object value)
        {
            _activities.ForEach(activity => activity.Execute(instance, value));
        }

        public void Inspect(StateMachineInspector inspector)
        {
            inspector.Inspect(this, _ => { _activities.ForEach(activity => activity.Inspect(inspector)); });
        }

        public Type ExceptionType
        {
            get { return _exceptionType; }
        }

        public Event Event
        {
            get { return _event; }
        }
    }
}