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
    using System.Collections.Generic;

    /// <summary>
    /// A state machine definition
    /// </summary>
    public interface StateMachine
    {
        /// <summary>
        /// The events defined in the state machine
        /// </summary>
        IEnumerable<Event> Events { get; }

        /// <summary>
        /// The states defined in the state machine
        /// </summary>
        IEnumerable<State> States { get; }

        /// <summary>
        /// The instance type associated with the state machine
        /// </summary>
        Type InstanceType { get; }

        /// <summary>
        /// The valid events that can be raised during the specified state
        /// </summary>
        /// <param name="state">The state to query</param>
        /// <returns>An enumeration of valid events</returns>
        IEnumerable<Event> NextEvents(State state);
    }


    /// <summary>
    /// A defined state machine that operations against the specified instance
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public interface StateMachine<in TInstance> :
        StateMachine
        where TInstance : class, StateMachineInstance
    {
        /// <summary>
        /// Raise a simple event on the state machine instance
        /// </summary>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        void RaiseEvent(TInstance instance, Event @event);

        /// <summary>
        /// Raise a data event on the state machine instance
        /// </summary>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="value">The data value associated with the event</param>
        void RaiseEvent<TData>(TInstance instance, Event<TData> @event, TData value)
            where TData : class;
    }
}