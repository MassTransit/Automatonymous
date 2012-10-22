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
namespace Automatonymous.Impl
{
    using System;
    using System.Threading.Tasks;


    public class InstanceLiftImpl<T, TInstance> :
        InstanceLift<T>
        where T : StateMachine<TInstance>
        where TInstance : class
    {
        readonly TInstance _instance;
        readonly T _stateMachine;

        public InstanceLiftImpl(T stateMachine, TInstance instance)
        {
            _stateMachine = stateMachine;
            _instance = instance;
        }

        public async Task Raise(Event @event)
        {
            await _stateMachine.RaiseEvent(_instance, @event);
        }

        public async Task Raise<TData>(Event<TData> @event, TData value)
        {
            await _stateMachine.RaiseEvent(_instance, @event, value);
        }

        public async Task Raise(Func<T, Event> eventSelector)
        {
            Event @event = eventSelector(_stateMachine);

            await _stateMachine.RaiseEvent(_instance, @event);
        }

        public async Task Raise<TData>(Func<T, Event<TData>> eventSelector, TData data)
        {
            Event<TData> @event = eventSelector(_stateMachine);

            await _stateMachine.RaiseEvent(_instance, @event, data);
        }
    }
}