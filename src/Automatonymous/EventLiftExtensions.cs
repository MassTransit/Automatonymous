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
    using Impl;


    public static class EventLiftExtensions
    {
        public static EventLift<TInstance> CreateEventLift<TInstance>(
            this StateMachine<TInstance> stateMachine,
            Event @event)
            where TInstance : class, StateMachineInstance
        {
            var activator = new EventLiftImpl<TInstance>(stateMachine, @event);

            return activator;
        }

        public static EventLift<TInstance, TData> CreateEventLift<TInstance, TData>(
            this StateMachine<TInstance> stateMachine,
            Event<TData> @event)
            where TInstance : class, StateMachineInstance
            where TData : class
        {
            var activator = new EventLiftImpl<TInstance, TData>(stateMachine, @event);

            return activator;
        }
    }
}