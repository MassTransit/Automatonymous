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
    using Impl;
    using Impl.Activities;


    public static class TransitionExtensions
    {
        public static EventActivityBinder<TInstance> TransitionTo<TInstance>(
            this EventActivityBinder<TInstance> source, State toState)
            where TInstance : StateMachineInstance
        {
            State<TInstance> state = toState.For<TInstance>();

            var activity = new TransitionActivity<TInstance>(state);

            return source.Add(activity);
        }

        public static EventActivityBinder<TInstance, TData> TransitionTo<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, State toState)
            where TInstance : StateMachineInstance
            where TData : class
        {
            State<TInstance> state = toState.For<TInstance>();

            var activity = new TransitionActivity<TInstance>(state);

            return source.Add(activity);
        }
    }
}