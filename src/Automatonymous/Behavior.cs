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
    using System.Threading.Tasks;
    using Behaviors;


    /// <summary>
    /// A behavior is invoked by a state when an event is raised on the instance and embodies
    /// the activities that are executed in response to the event.
    /// </summary>
    public static class Behavior
    {
        /// <summary>
        /// Returns an empty pipe of the specified context type
        /// </summary>
        /// <typeparam name="T">The context type</typeparam>
        /// <returns></returns>
        public static Behavior<T> Empty<T>()
        {
            return Cached<T>.EmptyBehavior;
        }

        public static Behavior<T, TData> Empty<T, TData>()
        {
            return Cached<T, TData>.EmptyBehavior;
        }

        public static Behavior<T> Exception<T>()
        {
            return Cached<T>.ExceptionBehavior;
        }

        public static Behavior<T, TData> Exception<T, TData>()
        {
            return Cached<T, TData>.ExceptionBehavior;
        }


        static class Cached<T>
        {
            internal static readonly Behavior<T> EmptyBehavior = new EmptyBehavior<T>();
            internal static readonly Behavior<T> ExceptionBehavior = new ExceptionBehavior<T>();
        }


        static class Cached<T, TData>
        {
            internal static readonly Behavior<T, TData> EmptyBehavior = new EmptyBehavior<T, TData>();
            internal static readonly Behavior<T, TData> ExceptionBehavior = new ExceptionBehavior<T, TData>();
        }
    }


    /// <summary>
    /// A behavior is a chain of activities invoked by a state
    /// </summary>
    /// <typeparam name="TInstance">The state type</typeparam>
    public interface Behavior<in TInstance> :
        Visitable
    {
        /// <summary>
        /// Execute the activity with the given behavior context
        /// </summary>
        /// <param name="context">The behavior context</param>
        /// <returns>An awaitable task</returns>
        Task Execute(BehaviorContext<TInstance> context);

        /// <summary>
        /// Execute the activity with the given behavior context
        /// </summary>
        /// <param name="context">The behavior context</param>
        /// <returns>An awaitable task</returns>
        Task Execute<T>(BehaviorContext<TInstance, T> context);

        /// <summary>
        /// The exception path through the behavior allows activities to catch and handle exceptions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context)
            where TException : Exception;

        /// <summary>
        /// The exception path through the behavior allows activities to catch and handle exceptions
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context)
            where TException : Exception;
    }


    /// <summary>
    /// A behavior is a chain of activities invoked by a state
    /// </summary>
    /// <typeparam name="TInstance">The state type</typeparam>
    /// <typeparam name="TData">The data type of the behavior</typeparam>
    public interface Behavior<in TInstance, in TData> :
        Visitable
    {
        /// <summary>
        /// Execute the activity with the given behavior context
        /// </summary>
        /// <param name="context">The behavior context</param>
        /// <returns>An awaitable task</returns>
        Task Execute(BehaviorContext<TInstance, TData> context);

        /// <summary>
        /// The exception path through the behavior allows activities to catch and handle exceptions
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context)
            where TException : Exception;
    }
}