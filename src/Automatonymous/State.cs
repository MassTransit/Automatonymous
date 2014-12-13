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


    public interface State :
        AcceptStateMachineInspector,
        IComparable<State>
    {
        string Name { get; }

        /// <summary>
        /// Raised when the state is entered
        /// </summary>
        Event Enter { get; }

        /// <summary>
        /// Raised when the state is about to be left
        /// </summary>
        Event Leave { get; }

        /// <summary>
        /// Raised just before the state is about to change to a new state
        /// </summary>
        Event<State> BeforeEnter { get; }

        /// <summary>
        /// Raised just after the state has been left and a new state is selected
        /// </summary>
        Event<State> AfterLeave { get; }
    }


    /// <summary>
    /// A state within a state machine that can be targeted with events
    /// </summary>
    /// <typeparam name="TInstance">The instance type to which the state applies</typeparam>
    public interface State<TInstance> :
        State
    {
        IEnumerable<Event> Events { get; }

        Task Raise(EventContext<TInstance> context);

        /// <summary>
        /// Raise an event to the state, passing the instance
        /// </summary>
        /// <typeparam name="T">The event data type</typeparam>
        /// <param name="context">The event context</param>
        /// <returns></returns>
        Task Raise<T>(EventContext<TInstance, T> context);


        void Bind(EventActivity<TInstance> activity);
    }
}