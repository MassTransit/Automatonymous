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
        Visitable
    {
    }


    /// <summary>
    /// An activity is part of a behavior that is executed in order
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public interface Activity<TInstance> :
        Activity
    {
        /// <summary>
        /// Execute the activity with the given behavior context
        /// </summary>
        /// <param name="context">The behavior context</param>
        /// <param name="next">The behavior that follows this activity</param>
        /// <returns>An awaitable task</returns>
        Task Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next);

        /// <summary>
        /// Execute the activity with the given behavior context
        /// </summary>
        /// <param name="context">The behavior context</param>
        /// <param name="next">The behavior that follows this activity</param>
        /// <returns>An awaitable task</returns>
        Task Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next);
    }


    public interface Activity<TInstance, TData> :
        Activity
    {
        /// <summary>
        /// Execute the activity with the given behavior context
        /// </summary>
        /// <param name="context">The behavior context</param>
        /// <param name="next">The behavior that follows this activity</param>
        /// <returns>An awaitable task</returns>
        Task Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next);
    }
}