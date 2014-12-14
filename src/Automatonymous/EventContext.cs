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
    using System;
    using System.Threading;


    public interface InstanceContext<out TInstance>
    {
        CancellationToken CancellationToken { get; }

        TInstance Instance { get; }

        /// <summary>
        /// Checks if a payload is present in the context
        /// </summary>
        /// <param name="contextType"></param>
        /// <returns></returns>
        bool HasPayloadType(Type contextType);

        /// <summary>
        /// Retrieves a payload from the pipe context
        /// </summary>
        /// <typeparam name="TPayload">The payload type</typeparam>
        /// <param name="payload">The payload</param>
        /// <returns></returns>
        bool TryGetPayload<TPayload>(out TPayload payload)
            where TPayload : class;

        /// <summary>
        /// Returns an existing payload or creates the payload using the factory method provided
        /// </summary>
        /// <typeparam name="TPayload">The payload type</typeparam>
        /// <param name="payloadFactory">The payload factory is the payload is not present</param>
        /// <returns>The payload</returns>
        TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class;
    }


    public delegate T PayloadFactory<out T>()
        where T : class;


    public interface EventContext<out TInstance> :
        InstanceContext<TInstance>
    {
        Event Event { get; }
    }


    /// <summary>
    /// Encapsulates an event that was raised which includes data
    /// </summary>
    /// <typeparam name="TInstance">The state instance the event is targeting</typeparam>
    /// <typeparam name="TData">The event data type</typeparam>
    public interface EventContext<out TInstance, out TData> :
        EventContext<TInstance>
    {
        new Event<TData> Event { get; }

        /// <summary>
        /// The data from the event
        /// </summary>
        TData Data { get; }
    }
}