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
    using Internal.Caching;


    public class TryActivity<TInstance> :
        Activity<TInstance>
        where TInstance : StateMachineInstance
    {
        readonly List<Activity<TInstance>> _activities;
        readonly Cache<Type, List<Activity<TInstance>>> _exceptionHandlers;

        public TryActivity(Event @event, EventActivityBinder<TInstance> activities,
                           ExceptionActivityBinder<TInstance> exceptionBinder)
        {
            _activities = new List<Activity<TInstance>>(activities
                .Select(x => new EventActivityImpl<TInstance>(@event, x)));

            _exceptionHandlers =
                new DictionaryCache<Type, List<Activity<TInstance>>>(x => new List<Activity<TInstance>>());

            foreach (var exceptionActivity in exceptionBinder)
                _exceptionHandlers[exceptionActivity.ExceptionType].Add(exceptionActivity);
        }

        public void Execute(TInstance instance, object value)
        {
            try
            {
                _activities.ForEach(activity => activity.Execute(instance, value));
            }
            catch (Exception ex)
            {
                Type exceptionType = ex.GetType();
                while (exceptionType != typeof(Exception).BaseType)
                {
                    if (_exceptionHandlers.WithValue(exceptionType,
                        x => x.ForEach(activity => activity.Execute(instance, ex))))
                        return;

                    exceptionType = exceptionType.BaseType;
                }

                throw;
            }
        }

        public void Inspect(StateMachineInspector inspector)
        {
            inspector.Inspect(this);
        }
    }
}