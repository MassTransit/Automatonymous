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
namespace Automatonymous
{
    using System;
    using System.Collections.Generic;


    public static class IntrospectionExtensions
    {
        /// <summary>
        /// Gets the next events from the state machine, given the state instance
        /// of that machine.
        /// </summary>
        /// <typeparam name="TInstance">The instance type that the state machine operates on</typeparam>
        /// <param name="machine">The state machine itself that this method is being called on</param>
        /// <param name="instance">The state instance</param>
        /// <returns>An IEnumerable of Event; and subsequent possible events.</returns>
        public static IEnumerable<Event> NextEvents<TInstance>(this StateMachine<TInstance> machine,
                                                               TInstance instance)
            where TInstance : class
        {
            if (machine == null)
                throw new ArgumentNullException("machine");
            if (instance == null)
                throw new ArgumentNullException("instance");

            return machine.NextEvents(machine.InstanceStateAccessor.Get(instance));
        }
    }
}