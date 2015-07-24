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


    public interface StateObserver<in TInstance>
    {
        /// <summary>
        /// Invoked prior to changing the state of the state machine
        /// </summary>
        /// <param name="context">The instance context of the state machine</param>
        /// <param name="currentState">The current state (after the change)</param>
        /// <param name="previousState">The previous state (before the change)</param>
        /// <returns></returns>
        Task StateChanged(InstanceContext<TInstance> context, State currentState, State previousState);
    }
}