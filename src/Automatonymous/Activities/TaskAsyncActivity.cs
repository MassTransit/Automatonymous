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


    public class TaskAsyncActivity<TInstance> :
        AsyncActivity<TInstance>
    {
        readonly Func<TInstance, Task<TInstance>> _action;

        public TaskAsyncActivity(Func<TInstance, Task<TInstance>> action)
        {
            _action = action;
        }

        public TaskAsyncActivity(Func<TInstance, Task> action)
        {
            _action = instance =>
                {
                    Task task = action(instance);
                    return task.ContinueWith(x => instance);
                };
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }

        public void Execute(TInstance instance)
        {
            Task<TInstance> task = _action(instance);
            task.Wait();
        }

        public void Execute<TData>(TInstance instance, TData value)
        {
            Task<TInstance> task = _action(instance);
            task.Wait();
        }

        public Task<TInstance> ExecuteAsync(TInstance instance)
        {
            return _action(instance);
        }

        public Task<TInstance> ExecuteAsync<TData>(TInstance instance, TData value)
        {
            return _action(instance);
        }
    }


    public class TaskAsyncActivity<TInstance, TData> :
        AsyncActivity<TInstance, TData>
    {
        readonly Func<TInstance, TData, Task<TInstance>> _action;

        public TaskAsyncActivity(Func<TInstance, TData, Task<TInstance>> action)
        {
            _action = action;
        }

        public TaskAsyncActivity(Func<TInstance, TData, Task> action)
        {
            _action = (instance, data) =>
                {
                    Task task = action(instance, data);
                    return task.ContinueWith(x => instance);
                };
        }


        public void Execute(TInstance instance, TData data)
        {
            Task<TInstance> task = _action(instance, data);
            task.Wait();
        }

        public Task<TInstance> ExecuteAsync(TInstance instance, TData data)
        {
            return _action(instance, data);
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}