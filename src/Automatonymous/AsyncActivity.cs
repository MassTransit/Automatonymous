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
    using System.Threading.Tasks;


    public interface AsyncActivity<TInstance> :
        Activity<TInstance>
    {
        /// <summary>
        /// Instructs the activity to execute asynchronously, at which point it should
        /// start a Task and return it
        /// </summary>
        /// <param name="instance">The state machine instance</param>
        /// <param name="data">The data of the execution</param>
        /// <returns>A started Task</returns>
        Task<TInstance> ExecuteAsync(TInstance instance);

        /// <summary>
        /// Instructs the activity to execute asynchronously, at which point it should
        /// start a Task and return it
        /// </summary>
        /// <param name="instance">The state machine instance</param>
        /// <param name="value">The data of the execution</param>
        /// <returns>A started Task</returns>
        Task<TInstance> ExecuteAsync<TData>(TInstance instance, TData value);
    }


    public interface AsyncActivity<TInstance, in TData> :
        Activity<TInstance, TData>
    {
        /// <summary>
        /// Instructs the activity to execute asynchronously, at which point it should
        /// start a Task and return it
        /// </summary>
        /// <param name="instance">The state machine instance</param>
        /// <param name="data">The data of the execution</param>
        /// <returns>A started Task</returns>
        Task<TInstance> ExecuteAsync(TInstance instance, TData data);
    }
}