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
    using System.Threading.Tasks;


    /// <summary>
    /// The context of an unhandled event in the state machine
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public interface UnhandledEventContext<out TInstance> :
        EventContext<TInstance>
    {
        /// <summary>
        /// The current state of the state machine
        /// </summary>
        State CurrentState { get; }

        /// <summary>
        /// Returns a Task that ignores the unhandled event
        /// </summary>
        Task Ignore();

        /// <summary>
        /// Returns a thrown exception task for the unhandled event
        /// </summary>
        /// <returns></returns>
        Task Throw();
    }
}