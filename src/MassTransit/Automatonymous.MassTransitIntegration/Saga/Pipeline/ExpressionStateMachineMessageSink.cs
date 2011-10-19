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
namespace Automatonymous.Saga.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Magnum.Extensions;
    using MassTransit;
    using MassTransit.Saga;
    using MassTransit.Saga.Pipeline;


    public class ExpressionStateMachineMessageSink<TInstance, TMessage> :
        SagaMessageSinkBase<TInstance, TMessage>
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        public ExpressionStateMachineMessageSink(StateMachine<TInstance> stateMachine,
                                                 ISagaRepository<TInstance> repository,
                                                 ISagaPolicy<TInstance, TMessage> policy,
                                                 Event<TMessage> @event,
                                                 Expression<Func<TInstance, TMessage, bool>> selector)
            : base(repository, policy, new PropertySagaLocator<TInstance, TMessage>(repository, policy, selector),
                (s, c) => GetHandlers(stateMachine, s, c, @event))
        {
        }

        static IEnumerable<Action<IConsumeContext<TMessage>>> GetHandlers(StateMachine<TInstance> stateMachine,
                                                                          TInstance instance,
                                                                          IConsumeContext<TMessage> context,
                                                                          Event<TMessage> @event)
        {
            yield return x =>
                {
                    instance.Bus = context.Bus;

                    context.BaseContext.NotifyConsume(context, typeof(TInstance).ToShortTypeName(),
                        instance.CorrelationId.ToString());

                    using (x.CreateScope())
                        stateMachine.RaiseEvent(instance, @event, x.Message);
                };
        }
    }
}