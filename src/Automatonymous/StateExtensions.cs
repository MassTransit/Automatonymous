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
    using System;


    public static class StateExtensions
    {
        public static State<TInstance> For<TInstance>(this State state)
            where TInstance : class, StateMachineInstance
        {
            if (state == null)
                throw new ArgumentNullException("state");

            var result = state as State<TInstance>;
            if (result == null)
                throw new ArgumentException("The state is invalid: " + state.Name);

            return result;
        }

        public static void WithState<TInstance>(this State state, Action<State<TInstance>> callback)
            where TInstance : class, StateMachineInstance
        {
            if (state == null)
                return;

            var result = state as State<TInstance>;
            if (result == null)
                throw new ArgumentException("The state is invalid: " + state.Name);

            callback(result);
        }
    }
}