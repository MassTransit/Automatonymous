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
    using System.Threading;
    using System.Threading.Tasks;
    using Activities;


    public static class StateMachineExtensions
    {
        /// <summary>
        ///     Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance>(this T stateMachine, TInstance instance,
            Func<T, Event> eventSelector, CancellationToken cancellationToken = default(CancellationToken))
            where T : StateMachine<TInstance>
            where TInstance : class
        {
            Event @event = eventSelector(stateMachine);

            return stateMachine.RaiseEvent(instance, @event, cancellationToken);
        }

        /// <summary>
        ///     Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, TData>(this T stateMachine, TInstance instance,
            Func<T, Event<TData>> eventSelector, TData data, CancellationToken cancellationToken = default(CancellationToken))
            where T : StateMachine<TInstance>
            where TInstance : class
        {
            Event<TData> @event = eventSelector(stateMachine);

            return stateMachine.RaiseEvent(instance, @event, data, cancellationToken);
        }

        /// <summary>
        ///     Returns an instance-specific version of the state machine (smart cast essentially)
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

        /// <summary>
        ///     Transition a state machine instance to a specific state, producing any events related
        ///     to the transaction such as leaving the previous state and entering the target state
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="instance">The state instance</param>
        /// <param name="state">The target state</param>
        /// <param name="cancellationToken"></param>
        public static Task TransitionToState<TInstance>(this StateMachine<TInstance> stateMachine, TInstance instance,
            State state, CancellationToken cancellationToken = default(CancellationToken))
            where TInstance : class
        {
            StateAccessor<TInstance> accessor = stateMachine.InstanceStateAccessor;
            State<TInstance> toState = state.For<TInstance>();

            return TransitionToState(instance, accessor, toState, cancellationToken);
        }

        /// <summary>
        ///     Transition a state machine instance to a specific state, producing any events related
        ///     to the transaction such as leaving the previous state and entering the target state
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <param name="instance">The state instance</param>
        /// <param name="accessor"></param>
        /// <param name="state">The target state</param>
        /// <param name="cancellationToken"></param>
        public static Task TransitionToState<TInstance>(TInstance instance, StateAccessor<TInstance> accessor,
            State<TInstance> state, CancellationToken cancellationToken)
            where TInstance : class
        {
            Activity<TInstance> activity = new TransitionActivity<TInstance>(state, accessor);

            return activity.Execute(instance, cancellationToken);
        }
    }
}