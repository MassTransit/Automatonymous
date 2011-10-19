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
namespace Automatonymous.RepositoryBuilders
{
    using System;
    using System.Linq.Expressions;
    using MassTransit.Util;


    public class StateMachineEventCorrelationImpl<TInstance, TData>
        : StateMachineEventCorrelation<TInstance>
        where TInstance : SagaStateMachineInstance
        where TData : class
    {
        readonly Expression<Func<TInstance, TData, bool>> _correlationExpression;
        Event<TData> _event;

        public StateMachineEventCorrelationImpl(Event<TData> @event, Expression<Func<TInstance, TData, bool>> correlationExpression)
        {
            _correlationExpression = correlationExpression;
            _event = @event;
        }

        public Event Event
        {
            get { return _event; }
        }

        public Expression<Func<TInstance, TMessage, bool>> GetCorrelationExpression<TMessage>()
        {
            return _correlationExpression.TranslateTo<Expression<Func<TInstance, TMessage, bool>>>();
        }
    }
}