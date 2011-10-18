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
namespace MassTransit.AutomatonymousIntegration.SubscriptionConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Automatonymous;
    using Magnum.Extensions;
    using Magnum.Reflection;
    using Saga;


    public interface StateMachineEventConnectorFactory
    {
        IEnumerable<StateMachineSubscriptionConnector> Create();
    }


    public class StateMachineEventConnectorFactory<TInstance, TMessage> :
        StateMachineEventConnectorFactory
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly Event<TMessage> _event;
        readonly StateMachinePolicyFactory<TInstance> _policyFactory;
        readonly Expression<Func<TInstance, bool>> _removeExpression;
        readonly ISagaRepository<TInstance> _sagaRepository;
        readonly StateMachine<TInstance> _stateMachine;
        readonly IEnumerable<State> _states;

        public StateMachineEventConnectorFactory(StateMachine<TInstance> stateMachine, ISagaRepository<TInstance> sagaRepository,
                                                 StateMachinePolicyFactory<TInstance> policyFactory,
                                                 Event<TMessage> @event, IEnumerable<State> states)
        {
            _stateMachine = stateMachine;
            _sagaRepository = sagaRepository;
            _policyFactory = policyFactory;
            _event = @event;
            _states = states;

            _removeExpression = x => false; // SagaStateMachine<TSaga>.GetCompletedExpression();
        }

        public IEnumerable<StateMachineSubscriptionConnector> Create()
        {
            Expression<Func<TInstance, TMessage, bool>> expression;
//            if (SagaStateMachine<TSaga>.TryGetCorrelationExpressionForEvent(_event, out expression))
//			{
//				yield return (SagaSubscriptionConnector) FastActivator.Create(typeof (PropertyStateMachineSubscriptionConnector<,>),
//					new[] {typeof (TSaga), typeof (TMessage)},
//					new object[] {_sagaRepository, _dataEvent, _states, _policyFactory, _removeExpression, expression});
//			}
//			else 
            if (typeof(TMessage).Implements<CorrelatedBy<Guid>>())
            {
                yield return
                    (StateMachineSubscriptionConnector)
                    FastActivator.Create(typeof(CorrelatedStateMachineSubscriptionConnector<,>),
                        new[] {typeof(TInstance), typeof(TMessage)},
                        new object[]
                            {_stateMachine, _sagaRepository, _event, _states, _policyFactory, _removeExpression});
            }
            else
            {
                throw new NotSupportedException("No method to connect to event was found for "
                                                + typeof(TMessage).FullName);
            }
        }
    }
}