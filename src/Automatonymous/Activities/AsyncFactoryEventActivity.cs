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
    using System.Threading.Tasks;


    public class AsyncFactoryEventActivity<TInstance> :
        AsyncActivity<TInstance>
    {
        readonly Func<AsyncActivity<TInstance>> _activityFactory;

        public AsyncFactoryEventActivity(Func<AsyncActivity<TInstance>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        public void Execute(TInstance instance)
        {
            AsyncActivity<TInstance> activity = _activityFactory();

            activity.Execute(instance);
        }

        public void Execute<TData>(TInstance instance, TData value)
        {
            AsyncActivity<TInstance> activity = _activityFactory();

            activity.Execute(instance, value);
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }

        public Task<TInstance> ExecuteAsync(TInstance instance)
        {
            AsyncActivity<TInstance> activity = _activityFactory();

            return activity.ExecuteAsync(instance);
        }

        public Task<TInstance> ExecuteAsync<TData>(TInstance instance, TData value)
        {
            AsyncActivity<TInstance> activity = _activityFactory();

            return activity.ExecuteAsync(instance, value);
        }
    }


    public class AsyncFactoryEventActivity<TInstance, TData> :
        AsyncActivity<TInstance, TData>
    {
        readonly Func<AsyncActivity<TInstance, TData>> _activityFactory;

        public AsyncFactoryEventActivity(Func<AsyncActivity<TInstance, TData>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        public void Execute(TInstance instance, TData data)
        {
            AsyncActivity<TInstance, TData> activity = _activityFactory();

            activity.Execute(instance, data);
        }

        public Task<TInstance> ExecuteAsync(TInstance instance, TData data)
        {
            AsyncActivity<TInstance, TData> activity = _activityFactory();

            return activity.ExecuteAsync(instance, data);
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}