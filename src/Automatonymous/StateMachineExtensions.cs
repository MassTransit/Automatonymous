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
namespace Automatonymous
{
    using System;


    public static class StateMachineExtensions
    {
        public static void RaiseEvent<T, TInstance>(this T stateMachine, TInstance instance,
                                                    Func<T, Event> eventSelector)
            where T : StateMachine<TInstance>
            where TInstance : class, StateMachineInstance
        {
            Event @event = eventSelector(stateMachine);

            stateMachine.RaiseEvent(instance, @event);
        }

        public static void RaiseEvent<T, TData, TInstance>(this T stateMachine, TInstance instance,
                                                           Func<T, Event<TData>> eventSelector, TData data)
            where T : StateMachine<TInstance>
            where TData : class
            where TInstance : class, StateMachineInstance
        {
            Event<TData> @event = eventSelector(stateMachine);

            stateMachine.RaiseEvent(instance, @event, data);
        }
    }
}