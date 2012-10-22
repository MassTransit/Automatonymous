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
    using System.Threading.Tasks;


    public static class StateMachineExtensions
    {
        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        public static async Task RaiseEvent<T, TInstance>(this T stateMachine, TInstance instance,
                                                    Func<T, Event> eventSelector)
            where T : StateMachine<TInstance>
            where TInstance : class
        {
            Event @event = eventSelector(stateMachine);

            await stateMachine.RaiseEvent(instance, @event);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The data type of the event</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The data for the event</param>
        public static async Task RaiseEvent<T, TData, TInstance>(this T stateMachine, TInstance instance,
                                                           Func<T, Event<TData>> eventSelector, TData data)
            where T : StateMachine<TInstance>
            where TInstance : class
        {
            Event<TData> @event = eventSelector(stateMachine);

            await stateMachine.RaiseEvent(instance, @event, data);
        }

        /// <summary>
        /// Returns an instance-specific version of the state machine (smart cast essentially)
        /// </summary>
        /// <typeparam name="TInstance">The instance type requested</typeparam>
        /// <param name="stateMachine">The untyped state machine interface</param>
        /// <returns>The typed static machine reference</returns>
        public static StateMachine<TInstance> For<TInstance>(this StateMachine stateMachine)
            where TInstance : class
        {
            if (stateMachine == null)
                throw new ArgumentNullException("stateMachine");

            var result = stateMachine as StateMachine<TInstance>;
            if (result == null)
                throw new ArgumentException("The state machine is not of the instance type: " + typeof(TInstance).Name);

            return result;
        }

        public static async Task WithStateMachine<TInstance>(this StateMachine stateMachine,
                                                       Func<StateMachine<TInstance>, Task> callback)
            where TInstance : class
        {
            StateMachine<TInstance> machine = stateMachine.For<TInstance>();

            await callback(machine);
        }
    }
}