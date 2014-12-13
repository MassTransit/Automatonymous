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
    /// <summary>
    /// A behavior context is an event context delivered to a behavior, including the state instance
    /// </summary>
    /// <typeparam name="TInstance">The state instance type</typeparam>
    public interface BehaviorContext<out TInstance> :
        EventContext<TInstance>
    {
        /// <summary>
        /// Return a proxy of the current behavior context with the specified event
        /// </summary>
        /// <param name="event">The event for the new context</param>
        /// <returns></returns>
        BehaviorContext<TInstance> GetProxy(Event @event);

        /// <summary>
        /// Return a proxy of the current behavior context with the specified event and data
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="event">The event for the new context</param>
        /// <param name="data">The data for the event</param>
        /// <returns></returns>
        BehaviorContext<TInstance, T> GetProxy<T>(Event<T> @event, T data);
    }


    /// <summary>
    /// A behavior context include an event context, along with the behavior for a state instance.
    /// </summary>
    /// <typeparam name="TInstance">The instance type</typeparam>
    /// <typeparam name="TData">The event type</typeparam>
    public interface BehaviorContext<out TInstance, out TData> :
        BehaviorContext<TInstance>,
        EventContext<TInstance, TData>
    {
    }
}