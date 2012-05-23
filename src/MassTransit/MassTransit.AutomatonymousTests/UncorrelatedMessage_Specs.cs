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
namespace MassTransit.AutomatonymousTests
{
    using System;
    using Automatonymous;
    using Magnum.Extensions;
    using NUnit.Framework;
    using Saga;
    using SubscriptionConfigurators;


    [TestFixture]
    public class When_a_message_is_not_correlated :
        MassTransitTestFixture
    {
        [Test]
        public void Should_create_the_saga_with_any_id()
        {
            var responseReceived = new FutureMessage<StartupComplete>();

            Bus.PublishRequest(new Start("A"), x =>
                {
                    x.Handle<StartupComplete>(responseReceived.Set);
                    x.HandleTimeout(8.Seconds(), () => { });
                });

            Assert.IsTrue(responseReceived.IsAvailable(0.Seconds()));
            Assert.AreEqual("A", responseReceived.Message.ServiceName);

            Assert.IsNotNull(_repository.ShouldContainSaga(responseReceived.Message.ServiceId, 1.Seconds()));
        }

        protected override void ConfigureSubscriptions(SubscriptionBusServiceConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository, x =>
                {
                    x.Correlate(_machine.Started, (i, d) => i.ServiceName == d.ServiceName);
                });
        }

        TestStateMachine _machine;
        InMemorySagaRepository<Instance> _repository;


        class Instance :
            SagaStateMachineInstance
        {
            public Instance(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            protected Instance()
            {
            }

            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
            public IServiceBus Bus { get; set; }

            public string ServiceName { get; set; }
        }


        class TestStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public TestStateMachine()
			{
				InstanceStatePropertyAccessor(x => x.CurrentState);

                State(() => Running);
                Event(() => Started);

                Initially(
                    When(Started)
                        .Then((i,d) => i.ServiceName = d.ServiceName)
                        .Respond((i,d) => new StartupComplete{ServiceId = i.CorrelationId, ServiceName = i.ServiceName})
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
        }


        class Start
        {
            public Start(string serviceName)
            {
                ServiceName = serviceName;
            }

            public Start()
            {
            }

            public string ServiceName { get; set; }
        }


        class StartupComplete
        {
            public Guid ServiceId { get; set; }
            public string ServiceName { get; set; }
        }
    }
}