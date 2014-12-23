// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


        static class Cached<T>
        {
            internal static readonly Behavior<T> EmptyBehavior = new EmptyBehavior<T>();
        }


        static class Cached<T, TData>
        {
            internal static readonly Behavior<T, TData> EmptyBehavior = new EmptyBehavior<T, TData>();
        }
    }


    /// <summary>
    /// A behavior is a chain of activities invoked by a state
    /// </summary>
    /// <typeparam name="TInstance">The state type</typeparam>
    public interface Behavior<in TInstance> :
        AcceptStateMachineInspector
    {
        Task Execute(BehaviorContext<TInstance> context);

        Task Execute<T>(BehaviorContext<TInstance, T> context);
    }


    /// <summary>
    /// A behavior is a chain of activities invoked by a state
    /// </summary>
    /// <typeparam name="TInstance">The state type</typeparam>
    /// <typeparam name="TData">The data type of the behavior</typeparam>
    public interface Behavior<in TInstance, in TData> :
        AcceptStateMachineInspector
    {
        Task Execute(BehaviorContext<TInstance, TData> context);
    }
}