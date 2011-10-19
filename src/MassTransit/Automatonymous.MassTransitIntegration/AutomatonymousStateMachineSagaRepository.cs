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
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using MassTransit;
    using MassTransit.Pipeline;
    using MassTransit.Saga;
    using RepositoryBuilders;
    using Util.Caching;


    public class AutomatonymousStateMachineSagaRepository<TInstance> :
        StateMachineSagaRepository<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        Expression<Func<TInstance, bool>> _completedExpression;
        Cache<Event, StateMachineEventCorrelation<TInstance>> _correlations;
        ISagaRepository<TInstance> _repository;

        public AutomatonymousStateMachineSagaRepository(ISagaRepository<TInstance> repository,
                                                        Expression<Func<TInstance, bool>> completedExpression,
                                                        IEnumerable<StateMachineEventCorrelation<TInstance>>
                                                            correlations)
        {
            _repository = repository;
            _completedExpression = completedExpression;

            _correlations = new DictionaryCache<Event, StateMachineEventCorrelation<TInstance>>(x => x.Event);
            _correlations.Fill(correlations);
        }

        public IEnumerable<Action<IConsumeContext<TMessage>>> GetSaga<TMessage>(IConsumeContext<TMessage> context,
                                                                                Guid sagaId,
                                                                                InstanceHandlerSelector
                                                                                    <TInstance, TMessage> selector,
                                                                                ISagaPolicy<TInstance, TMessage> policy)
            where TMessage : class
        {
            return _repository.GetSaga(context, sagaId, selector, policy);
        }

        public IEnumerable<Guid> Find(ISagaFilter<TInstance> filter)
        {
            return _repository.Find(filter);
        }

        public IEnumerable<TInstance> Where(ISagaFilter<TInstance> filter)
        {
            return _repository.Where(filter);
        }

        public IEnumerable<TResult> Where<TResult>(ISagaFilter<TInstance> filter, Func<TInstance, TResult> transformer)
        {
            return _repository.Where(filter, transformer);
        }

        public IEnumerable<TResult> Select<TResult>(Func<TInstance, TResult> transformer)
        {
            return _repository.Select(transformer);
        }

        public bool TryGetCorrelationExpressionForEvent<TData>(Event<TData> @event,
                                                               out Expression<Func<TInstance, TData, bool>> expression)
            where TData : class
        {
            expression = _correlations.WithValue(@event, correlation => correlation.GetCorrelationExpression<TData>(),
                null);

            return expression != null;
        }

        public Expression<Func<TInstance, bool>> GetCompletedExpression()
        {
            return _completedExpression;
        }
    }
}