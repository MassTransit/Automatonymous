// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
    using TaskComposition;


    public static class StateMachineExtensions
    {
        /// <summary>
        ///     Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="composer"></param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        public static void RaiseEvent<T, TInstance>(this T stateMachine, Composer composer, TInstance instance,
            Func<T, Event> eventSelector)
            where T : StateMachine<TInstance>
            where TInstance : class
        {
            Event @event = eventSelector(stateMachine);

            stateMachine.RaiseEvent(composer, instance, @event);
        }

        /// <summary>
        ///     Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="composer"></param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data"></param>
        public static void RaiseEvent<T, TInstance, TData>(this T stateMachine, Composer composer, TInstance instance,
            Func<T, Event<TData>> eventSelector, TData data)
            where T : StateMachine<TInstance>
            where TInstance : class
        {
            Event<TData> @event = eventSelector(stateMachine);

            stateMachine.RaiseEvent(composer, instance, @event, data);
        }

        /// <summary>
        ///     Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="cancellationToken"></param>
        public static void RaiseEvent<T, TInstance>(this T stateMachine, TInstance instance, Func<T, Event> eventSelector,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : StateMachine<TInstance>
            where TInstance : class
        {
            Event @event = eventSelector(stateMachine);

            RaiseEvent(stateMachine, instance, @event, cancellationToken);
        }

        /// <summary>
        ///     Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The data type of the event</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The data for the event</param>
        /// <param name="cancellationToken"></param>
        public static void RaiseEvent<T, TData, TInstance>(this T stateMachine, TInstance instance, Func<T, Event<TData>> eventSelector,
            TData data, CancellationToken cancellationToken = default(CancellationToken))
            where T : StateMachine<TInstance>
            where TInstance : class
        {
            Event<TData> @event = eventSelector(stateMachine);

            RaiseEvent(stateMachine, instance, @event, data, cancellationToken);
        }

        /// <summary>
        ///     Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">Event on the state machine</param>
        /// <param name="cancellationToken"></param>
        public static void RaiseEvent<T, TInstance>(this T stateMachine, TInstance instance, Event @event,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : StateMachine<TInstance>
            where TInstance : class
        {
            var composer = new TaskComposer<TInstance>(cancellationToken);

            stateMachine.RaiseEvent(composer, instance, @event);

            composer.Finish().Wait();
        }

        /// <summary>
        ///     Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="TData">The data type of the event</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event"></param>
        /// <param name="data">The data for the event</param>
        /// <param name="cancellationToken"></param>
        public static void RaiseEvent<TData, TInstance>(this StateMachine<TInstance> stateMachine, TInstance instance,
            Event<TData> @event, TData data, CancellationToken cancellationToken = default(CancellationToken))
            where TInstance : class
        {
            var composer = new TaskComposer<TInstance>(cancellationToken);

            stateMachine.RaiseEvent(composer, instance, @event, data);

            composer.Finish().Wait();
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
        /// <param name="composer"></param>
        /// <param name="instance">The state instance</param>
        /// <param name="state">The target state</param>
        public static void TransitionToState<TInstance>(this StateMachine<TInstance> stateMachine, Composer composer, TInstance instance,
            State state)
            where TInstance : class
        {
            StateAccessor<TInstance> accessor = stateMachine.InstanceStateAccessor;
            State<TInstance> toState = state.For<TInstance>();

            TransitionToState(composer, instance, accessor, toState);
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
            var composer = new TaskComposer<TInstance>(cancellationToken);

            stateMachine.TransitionToState(composer, instance, state);

            return composer.Finish();
        }

        /// <summary>
        ///     Transition a state machine instance to a specific state, producing any events related
        ///     to the transaction such as leaving the previous state and entering the target state
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <param name="composer"></param>
        /// <param name="instance">The state instance</param>
        /// <param name="accessor"></param>
        /// <param name="state">The target state</param>
        public static void TransitionToState<TInstance>(Composer composer, TInstance instance, StateAccessor<TInstance> accessor,
            State<TInstance> state)
            where TInstance : class
        {
            Activity<TInstance> activity = new TransitionActivity<TInstance>(state, accessor);

            activity.Execute(composer, instance);
        }
    }
}