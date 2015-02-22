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
    /// Callback for an unhandled event in the state machine
    /// </summary>
    /// <typeparam name="TInstance">The state machine instance type</typeparam>
    /// <param name="context">The event context</param>
    /// <returns></returns>
    public delegate Task UnhandledEventCallback<in TInstance>(UnhandledEventContext<TInstance> context);
}