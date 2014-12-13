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
    using System.Threading.Tasks;


    public interface Activity :
        AcceptStateMachineInspector
    {
    }


    /// <summary>
    /// An activity is part of a behavior that is executed in order
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public interface Activity<TState> :
        Activity
    {
        /// <summary>
        /// Execute the activity with the given behavior context
        /// </summary>
        /// <param name="context">The behavior context</param>
        /// <param name="next">The behavior that follows this activity</param>
        /// <returns>An awaitable task</returns>
        Task Execute(BehaviorContext<TState> context, Behavior<TState> next);

        /// <summary>
        /// Execute the activity with the given behavior context
        /// </summary>
        /// <param name="context">The behavior context</param>
        /// <param name="next">The behavior that follows this activity</param>
        /// <returns>An awaitable task</returns>
        Task Execute<T>(BehaviorContext<TState, T> context, Behavior<TState, T> next);
    }


    public interface Activity<TState, TData> :
        Activity
    {
        /// <summary>
        /// Execute the activity with the given behavior context
        /// </summary>
        /// <param name="context">The behavior context</param>
        /// <param name="next">The behavior that follows this activity</param>
        /// <returns>An awaitable task</returns>
        Task Execute(BehaviorContext<TState, TData> context, Behavior<TState, TData> next);
    }
}