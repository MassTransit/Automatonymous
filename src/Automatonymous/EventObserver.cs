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


    public interface EventObserver<in TInstance>
    {
        /// <summary>
        /// Called before the event context is delivered to the activities
        /// </summary>
        /// <param name="context">The event context</param>
        /// <returns></returns>
        Task PreExecute(EventContext<TInstance> context);

        /// <summary>
        /// Called before the event context is delivered to the activities
        /// </summary>
        /// <typeparam name="T">The event data type</typeparam>
        /// <param name="context">The event context</param>
        /// <returns></returns>
        Task PreExecute<T>(EventContext<TInstance, T> context);

        /// <summary>
        /// Called when the event has been processed by the activities
        /// </summary>
        /// <param name="context">The event context</param>
        /// <returns></returns>
        Task PostExecute(EventContext<TInstance> context);

        /// <summary>
        /// Called when the event has been processed by the activities
        /// </summary>
        /// <typeparam name="T">The event data type</typeparam>
        /// <param name="context">The event context</param>
        /// <returns></returns>
        Task PostExecute<T>(EventContext<TInstance, T> context);

        /// <summary>
        /// Called when the activity execution faults and is not handled by the activities
        /// </summary>
        /// <param name="context">The event context</param>
        /// <param name="exception">The exception that was thrown</param>
        /// <returns></returns>
        Task ExecuteFault(EventContext<TInstance> context, Exception exception);

        /// <summary>
        /// Called when the activity execution faults and is not handled by the activities
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The event context</param>
        /// <param name="exception">The exception that was thrown</param>
        /// <returns></returns>
        Task ExecuteFault<T>(EventContext<TInstance, T> context, Exception exception);
    }
}