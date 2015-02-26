// Copyright 2011-2015 Chris Patterson, Dru Sellers
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
    using Behaviors;
    using Contexts;


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
            if (@event == null)
                throw new ArgumentNullException("eventSelector", "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(stateMachine, instance, @event, cancellationToken);

            return stateMachine.RaiseEvent(context);
        }

        /// <summary>
        ///     Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance>(this T machine, TInstance instance, Event @event,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : StateMachine<TInstance>
            where TInstance : class
        {
            if (@event == null)
                throw new ArgumentNullException("event", "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        ///     Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, TData>(this T machine, TInstance instance, Event<TData> @event, TData data,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : StateMachine<TInstance>
            where TInstance : class
        {
            if (@event == null)
                throw new ArgumentNullException("event", "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);

            return machine.RaiseEvent(context);
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

            var context = new StateMachineEventContext<TInstance, TData>(stateMachine, instance, @event, data, cancellationToken);

            return stateMachine.RaiseEvent(context);
        }

        /// <summary>
        ///     Transition a state machine instance to a specific state, producing any events related
        ///     to the transaction such as leaving the previous state and entering the target state
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state instance</param>
        /// <param name="state">The target state</param>
        /// <param name="cancellationToken"></param>
        public static Task TransitionToState<TInstance>(this StateMachine<TInstance> machine, TInstance instance,
            State state, CancellationToken cancellationToken = default(CancellationToken))
            where TInstance : class
        {
            StateAccessor<TInstance> accessor = machine.InstanceStateAccessor;
            State<TInstance> toState = machine.GetState(state.Name); // state.For<TInstance>();

            Activity<TInstance> activity = new TransitionActivity<TInstance>(toState, accessor);
            Behavior<TInstance> behavior = new LastBehavior<TInstance>(activity);

            var eventContext = new StateMachineEventContext<TInstance>(machine, instance, toState.Enter, cancellationToken);

            BehaviorContext<TInstance> behaviorContext = new EventBehaviorContext<TInstance>(eventContext);

            return behavior.Execute(behaviorContext);
        }
    }
}