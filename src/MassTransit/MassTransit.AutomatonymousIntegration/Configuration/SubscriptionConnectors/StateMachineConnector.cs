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
    using System.Linq;
    using Automatonymous;
    using Exceptions;
    using Magnum.Reflection;
    using MassTransit.Pipeline;
    using Saga;


    public interface StateMachineConnector
    {
        UnsubscribeAction Connect(IInboundPipelineConfigurator configurator);
    }


    public class StateMachineConnector<T> :
        StateMachineConnector
        where T : class, SagaStateMachineInstance
    {
        readonly IEnumerable<StateMachineSubscriptionConnector> _connectors;
        readonly ISagaRepository<T> _sagaRepository;
        readonly StateMachine<T> _stateMachine;

        public StateMachineConnector(StateMachine<T> stateMachine, ISagaRepository<T> sagaRepository)
        {
            _stateMachine = stateMachine;
            _sagaRepository = sagaRepository;

            try
            {
                _connectors = StateMachineEvents().ToList();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(
                    "Failed to create the state machine connector for " + typeof(T).FullName, ex);
            }
        }

        public IEnumerable<StateMachineSubscriptionConnector> Connectors
        {
            get { return _connectors; }
        }

        public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator)
        {
            return _connectors.Select(x => x.Connect(configurator))
                .Aggregate<UnsubscribeAction, UnsubscribeAction>(() => true, (seed, x) => () => seed() && x());
        }

        IEnumerable<StateMachineSubscriptionConnector> StateMachineEvents()
        {
            var policyFactory = new AutomatonymousStateMachinePolicyFactory<T>(_stateMachine);

            foreach (Event @event in _stateMachine.Events)
            {
                Type eventType = @event.GetType();

                Type dataEventInterfaceType = eventType.GetInterfaces()
                    .Where(x => x.IsGenericType)
                    .Where(x => x.GetGenericTypeDefinition() == typeof(Event<>))
                    .SingleOrDefault();
                if (dataEventInterfaceType == null)
                    continue;

                Type messageType = dataEventInterfaceType.GetGenericArguments()[0];

                IEnumerable<State> states =
                    _stateMachine.States.Where(state => _stateMachine.NextEvents(state).Contains(@event));

                var factory =
                    (StateMachineEventConnectorFactory)
                    FastActivator.Create(typeof(StateMachineEventConnectorFactory<,>),
                        new[] {typeof(T), messageType},
                        new object[] {_stateMachine, _sagaRepository, policyFactory, @event, states});

                foreach (StateMachineSubscriptionConnector connector in factory.Create())
                    yield return connector;
            }
        }
    }
}