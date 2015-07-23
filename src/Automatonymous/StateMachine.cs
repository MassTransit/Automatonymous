// Copyright 2011-2014 Chris Patterson, Dru Sellers
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
    using System.Threading.Tasks;


    /// <summary>
    /// A state machine definition
    /// </summary>
    public interface StateMachine :
        Visitable
    {
        /// <summary>
        /// The name of the state machine (defaults to the state machine type name)
        /// </summary>
        string Name { get; }

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
        /// The initial state of a new state machine instance
        /// </summary>
        State Initial { get; }

        /// <summary>
        /// The final state of a state machine instance
        /// </summary>
        State Final { get; }

        /// <summary>
        /// Returns the event requested
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Event GetEvent(string name);

        /// <summary>
        /// Returns the state requested
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        State GetState(string name);

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
    public interface StateMachine<TInstance> :
        StateMachine
        where TInstance : class
    {
        /// <summary>
        /// Exposes state change events to observers
        /// </summary>
        IObservable<StateChanged<TInstance>> StateChanged { get; }

        /// <summary>
        /// Exposes the current state on the given instance
        /// </summary>
        StateAccessor<TInstance> Accessor { get; }

        /// <summary>
        /// Returns the state requested bound to the instance
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        new State<TInstance> GetState(string name);

        /// <summary>
        /// Raise a simple event on the state machine instance asynchronously
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Task for the instance once completed</returns>
        Task RaiseEvent(EventContext<TInstance> context);

        /// <summary>
        /// Raise a data event on the state machine instance
        /// </summary>
        /// <param name="context"></param>
        Task RaiseEvent<T>(EventContext<TInstance, T> context);

        /// <summary>
        /// Exposes a raised event to observers before it is raised on the instance
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        IObservable<EventRaising<TInstance>> EventRaising(Event @event);

        /// <summary>
        /// Exposes a raised event to observers after it is raised on the instance
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        IObservable<EventRaised<TInstance>> EventRaised(Event @event);
    }
}