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
namespace Automatonymous.Activities
{
    using System;


    public class FactoryEventActivity<TInstance> :
        Activity<TInstance>
    {
        readonly Func<Activity<TInstance>> _activityFactory;

        public FactoryEventActivity(Func<Activity<TInstance>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        public void Execute(TInstance instance)
        {
            Activity<TInstance> activity = _activityFactory();

            activity.Execute(instance);
        }

        public void Execute<TData>(TInstance instance, TData value)
        {
            Activity<TInstance> activity = _activityFactory();

            activity.Execute(instance, value);
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }


    public class FactoryEventActivity<TInstance, TData> :
        Activity<TInstance, TData>
        where TData : class
    {
        readonly Func<Activity<TInstance, TData>> _activityFactory;

        public FactoryEventActivity(Func<Activity<TInstance, TData>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        public void Execute(TInstance instance, TData data)
        {
            Activity<TInstance, TData> activity = _activityFactory();

            activity.Execute(instance, data);
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}