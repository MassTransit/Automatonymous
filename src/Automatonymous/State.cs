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
    using System.Threading.Tasks;
    using Activities;


    public interface State :
        AcceptStateMachineInspector,
        IComparable<State>
    {
        /// <summary>
        /// Name of this state. Equals the property name of the state
        /// on the state-machine.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the enter-state event. Before a state is entered this event is triggered.
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
    /// A state of a state machine. Gives access to 
    /// outgoing edged (transitions) by the <see cref="Events"/>
    /// property. Accepts a visitor. An event can be raised
    /// on this instance, possibly causing a transition,
    /// if this state holds any <see cref="TransitionActivity{TInstance}"/>-ies.
    /// </summary>
    /// <typeparam name="TInstance">The state instance type</typeparam>
    public interface State<TInstance> :
        State
    {
        IEnumerable<Event> Events { get; }

        Task Raise(TInstance instance, Event @event);

        Task Raise<TData>(TInstance instance, Event<TData> @event, TData value);

        void Bind(EventActivity<TInstance> activity);
    }
}