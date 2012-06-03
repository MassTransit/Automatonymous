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
    using Activities;
    using Binders;


    public static class TransitionExtensions
    {
        /// <summary>
        /// Transition the state machine to the specified state
        /// </summary>
        public static EventActivityBinder<TInstance> TransitionTo<TInstance>(
            this EventActivityBinder<TInstance> source, State toState)
            where TInstance : class, StateMachineInstance
        {
            State<TInstance> state = toState.For<TInstance>();

            var activity = new TransitionActivity<TInstance>(state, source.StateMachine.CurrentStateAccessor);

            return source.Add(activity);
        }

        /// <summary>
        /// Transition the state machine to the specified state
        /// </summary>
        public static EventActivityBinder<TInstance, TData> TransitionTo<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, State toState)
            where TInstance : class, StateMachineInstance
        {
            State<TInstance> state = toState.For<TInstance>();

            var activity = new TransitionActivity<TInstance>(state, source.StateMachine.CurrentStateAccessor);

            return source.Add(activity);
        }

        /// <summary>
        /// Transition the state machine to the Completed state
        /// </summary>
        public static EventActivityBinder<TInstance, TData> Finalize<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source)
            where TInstance : class, StateMachineInstance
        {
            State<TInstance> state = source.StateMachine.Final.For<TInstance>();

            var activity = new TransitionActivity<TInstance>(state, source.StateMachine.CurrentStateAccessor);

            return source.Add(activity);
        }


        /// <summary>
        /// Transition the state machine to the Completed state
        /// </summary>
        public static EventActivityBinder<TInstance> Finalize<TInstance>(
            this EventActivityBinder<TInstance> source)
            where TInstance : class, StateMachineInstance
        {
            State<TInstance> state = source.StateMachine.Final.For<TInstance>();

            var activity = new TransitionActivity<TInstance>(state, source.StateMachine.CurrentStateAccessor);

            return source.Add(activity);
        }
    }
}