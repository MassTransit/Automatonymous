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


    public class FactoryEventActivity<TActivity, TInstance, TData> :
        Activity<TInstance>
        where TActivity : Activity<TInstance, TData>
        where TInstance : StateMachineInstance
        where TData : class
    {
        readonly Func<TActivity> _activityFactory;

        public FactoryEventActivity(Func<TActivity> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        public void Execute(TInstance instance, object value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "The data argument cannot be null");

            var data = value as TData;
            if (data == null)
            {
                throw new ArgumentException("Expected: " + typeof(TData).Name + ", Received: " + value.GetType().Name,
                    "value");
            }

            Activity<TInstance, TData> activity = _activityFactory();

            activity.Execute(instance, data);
        }

        public void Inspect(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}