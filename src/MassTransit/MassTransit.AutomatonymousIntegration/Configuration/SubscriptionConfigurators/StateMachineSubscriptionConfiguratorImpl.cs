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
namespace MassTransit.AutomatonymousIntegration.SubscriptionConfigurators
{
    using System.Collections.Generic;
    using Automatonymous;
    using Configurators;
    using MassTransit.SubscriptionBuilders;
    using MassTransit.SubscriptionConfigurators;
    using Saga;
    using SubscriptionBuilders;


    public class StateMachineSubscriptionConfiguratorImpl<TInstance> :
        SubscriptionConfiguratorImpl<StateMachineSubscriptionConfigurator<TInstance>>,
        StateMachineSubscriptionConfigurator<TInstance>,
        SubscriptionBuilderConfigurator
        where TInstance : class, SagaStateMachineInstance
    {
        readonly ISagaRepository<TInstance> _sagaRepository;
        readonly StateMachine<TInstance> _stateMachine;

        public StateMachineSubscriptionConfiguratorImpl(StateMachine<TInstance> stateMachine,
                                                        ISagaRepository<TInstance> sagaRepository)
        {
            _stateMachine = stateMachine;
            _sagaRepository = sagaRepository;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_stateMachine == null)
                yield return this.Failure("The state machine cannot be null.");
        }

        public SubscriptionBuilder Configure()
        {
            return new StateMachineSubscriptionBuilder<TInstance>(_stateMachine, _sagaRepository, ReferenceFactory);
        }
    }
}