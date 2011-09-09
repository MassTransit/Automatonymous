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
    using System.Linq;


    public class ExceptionHandlerActivity<TInstance, TException> :
        ExceptionActivity<TInstance>
        where TInstance : StateMachineInstance
        where TException : class
    {
        readonly Type _exceptionType;
        readonly List<Activity<TInstance>> _activities;

        public ExceptionHandlerActivity(IEnumerable<EventActivity<TInstance>> activities, Type exceptionType)
        {
            _exceptionType = exceptionType;
            _activities = new List<Activity<TInstance>>(activities);
                //.Select(x => new ExceptionActivityImpl<TInstance, TException>(x)));
        }

        public void Execute(TInstance instance, object value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "The exception argument cannot be null");

            _activities.ForEach(activity => activity.Execute(instance, value));
        }

        public void Inspect(StateMachineInspector inspector)
        {
            inspector.Inspect(this);
        }

        public Type ExceptionType
        {
            get { return _exceptionType; }
        }
    }
}