// Copyright 2011 Chris Patterson, Dru Sellers, Henrik Feldt
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


    public class ActionActivity<TInstance> :
        Activity<TInstance>
    {
        // pre-computed task per-instance of this activity
        readonly Task _finishedTask = Task.Factory.StartNew(() => { });
        readonly Action<TInstance> _action;

        public ActionActivity(Action<TInstance> action)
        {
            _action = action;
        }

        public Task Execute(TInstance instance)
        {
            _action(instance);
            return _finishedTask;
        }

        public Task Execute<TData>(TInstance instance, TData value)
        {
            _action(instance);
            return _finishedTask;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }


    public class ActionActivity<TInstance, TData> :
        Activity<TInstance, TData>
    {
        // pre-computed task per-instance of this activity
        readonly Task _finishedTask = Task.Factory.StartNew(() => { });
        readonly Action<TInstance, TData> _action;

        public ActionActivity(Action<TInstance, TData> action)
        {
            _action = action;
        }

        public Task Execute(TInstance instance, TData data)
        {
            _action(instance, data);
            return _finishedTask;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}