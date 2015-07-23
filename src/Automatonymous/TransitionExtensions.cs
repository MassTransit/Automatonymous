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
    using Activities;
    using Binders;


    public static class TransitionExtensions
    {
        /// <summary>
        /// Transition the state machine to the specified state
        /// </summary>
        public static EventActivityBinder<TInstance> TransitionTo<TInstance>(
            this EventActivityBinder<TInstance> source, State toState)
            where TInstance : class
        {
            State<TInstance> state = source.StateMachine.GetState(toState.Name);

            var activity = new TransitionActivity<TInstance>(state, source.StateMachine.Accessor);

            return source.Add(activity);
        }

        /// <summary>
        /// Transition the state machine to the specified state in response to an exception
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="source"></param>
        /// <param name="toState"></param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TException> TransitionTo<TInstance, TException>(
            this ExceptionActivityBinder<TInstance, TException> source, State toState)
            where TInstance : class where TException : Exception
        {
            State<TInstance> state = source.StateMachine.GetState(toState.Name);

            var activity = new TransitionActivity<TInstance>(state, source.StateMachine.Accessor);

            var compensateActivity = new ExecuteOnFaultedActivity<TInstance>(activity);

            return source.Add(compensateActivity);
        }

        /// <summary>
        /// Transition the state machine to the specified state
        /// </summary>
        public static EventActivityBinder<TInstance, TData> TransitionTo<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, State toState)
            where TInstance : class
        {
            State<TInstance> state = source.StateMachine.GetState(toState.Name);

            var activity = new TransitionActivity<TInstance>(state, source.StateMachine.Accessor);

            return source.Add(activity);
        }

        /// <summary>
        /// Transition the state machine to the specified state in response to an exception
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="source"></param>
        /// <param name="toState"></param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TData, TException> TransitionTo<TInstance, TData, TException>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, State toState)
            where TInstance : class
            where TException : Exception
        {
            State<TInstance> state = source.StateMachine.GetState(toState.Name);

            var activity = new TransitionActivity<TInstance>(state, source.StateMachine.Accessor);

            var compensateActivity = new ExecuteOnFaultedActivity<TInstance>(activity);

            return source.Add(compensateActivity);
        }

        /// <summary>
        /// Transition the state machine to the Completed state
        /// </summary>
        public static EventActivityBinder<TInstance, TData> Finalize<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source)
            where TInstance : class
        {
            State<TInstance> state = source.StateMachine.GetState(source.StateMachine.Final.Name);

            var activity = new TransitionActivity<TInstance>(state, source.StateMachine.Accessor);

            return source.Add(activity);
        }


        /// <summary>
        /// Transition the state machine to the Completed state
        /// </summary>
        public static EventActivityBinder<TInstance> Finalize<TInstance>(
            this EventActivityBinder<TInstance> source)
            where TInstance : class
        {
            State<TInstance> state = source.StateMachine.GetState(source.StateMachine.Final.Name);

            var activity = new TransitionActivity<TInstance>(state, source.StateMachine.Accessor);

            return source.Add(activity);
        }
    }
}