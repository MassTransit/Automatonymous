﻿// Copyright 2011 Chris Patterson, Dru Sellers
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
    using RepositoryConfigurators;


    public static class StateMachineSagaRepositoryExtensions
    {
        public static StateMachineSagaRepositoryConfigurator<TInstance> RemoveWhenFinalized<TInstance>(
            this StateMachineSagaRepositoryConfigurator<TInstance> configurator)
            where TInstance : class, SagaStateMachineInstance
        {
            return configurator.RemoveWhen(x => x.Final);
        }

        public static StateMachineSagaRepositoryConfigurator<TInstance> RemoveWhen<TInstance>(
            this StateMachineSagaRepositoryConfigurator<TInstance> configurator,
            Func<StateMachine<TInstance>, State> stateSelector)
            where TInstance : class, SagaStateMachineInstance
        {
            State state = stateSelector(configurator.StateMachine);

            configurator.RemoveWhen(x => configurator.StateMachine.CurrentStateAccessor.Get(x) == state);

            return configurator;
        }
    }
}