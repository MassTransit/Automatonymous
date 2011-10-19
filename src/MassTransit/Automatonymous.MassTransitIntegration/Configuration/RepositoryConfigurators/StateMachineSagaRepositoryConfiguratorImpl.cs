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
namespace Automatonymous.RepositoryConfigurators
{
    using System;
    using System.Linq.Expressions;
    using BuilderConfigurators;
    using MassTransit.Saga;
    using RepositoryBuilders;
    using Util.Caching;


    public class StateMachineSagaRepositoryConfiguratorImpl<TInstance> :
        StateMachineSagaRepositoryConfigurator<TInstance>,
        StateMachineSagaRepositoryBuilderConfigurator<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly ISagaRepository<TInstance> _repository;
        readonly StateMachine<TInstance> _stateMachine;
        readonly Cache<Event, StateMachineEventCorrelation<TInstance>> _correlations;

        public StateMachineSagaRepositoryConfiguratorImpl(StateMachine<TInstance> stateMachine,
                                                          ISagaRepository<TInstance> repository)
        {
            _stateMachine = stateMachine;
            _repository = repository;
            _correlations = new DictionaryCache<Event, StateMachineEventCorrelation<TInstance>>();
        }

        public StateMachineSagaRepository<TInstance> Configure()
        {
            var builder = new StateMachineSagaRepositoryBuilderImpl<TInstance>(_stateMachine, _repository, _correlations);

            return builder.Build();
        }

        public void Correlate<TData>(Event<TData> @event, Expression<Func<TInstance, TData, bool>> correlationExpression) 
            where TData : class
        {
            var builder = new StateMachineEventCorrelationImpl<TInstance, TData>(@event, correlationExpression);

            _correlations[@event] = builder;
        }
    }
}