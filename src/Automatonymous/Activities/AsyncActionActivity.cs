// Copyright 2012 Henrik Feldt
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


    public class AsyncActionActivity<TInstance>
        : Activity<TInstance>
        where TInstance : class
    {
        readonly Func<TInstance, Task> _func;

        public AsyncActionActivity(Func<TInstance, Task> func)
        {
            _func = func;
        }

        public async Task Execute(TInstance instance)
        {
            await _func(instance);
        }

        public async Task Execute<TData>(TInstance instance, TData value)
        {
            await _func(instance);
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }

    public class AsyncActionActivity<TInstance, TData>
        : Activity<TInstance, TData>
    {
        readonly Func<TInstance, TData, Task> _func;

        public AsyncActionActivity(Func<TInstance, TData, Task> func)
        {
            _func = func;
        }

        public async Task Execute(TInstance instance, TData data)
        {
            await _func(instance, data);
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}