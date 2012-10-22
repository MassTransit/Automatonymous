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
    /// <summary>
    /// Gets and sets the actual state.
    /// </summary>
    /// <typeparam name="TInstance">The state instance type</typeparam>
    public interface StateAccessor<TInstance>
    {
        /// <summary>
        /// Gets the state that the accessor is pointing to.
        /// </summary>
        /// <param name="instance">The state instance</param>
        /// <returns>The state that the machine is in</returns>
        State<TInstance> Get(TInstance instance);

        void Set(TInstance instance, State<TInstance> state);
    }
}