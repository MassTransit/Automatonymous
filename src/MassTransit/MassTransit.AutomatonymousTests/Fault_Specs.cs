// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
    using System.Diagnostics;
    using Automatonymous;
    using Magnum.Extensions;
    using NUnit.Framework;
    using Saga;
    using SubscriptionConfigurators;


    [TestFixture]
    public class When_an_activity_throws_an_exception :
        MassTransitTestFixture
    {
        [Test]
        public void Should_be_received_as_a_fault_message()
        {
            var faultReceived = new FutureMessage<Fault<Start>>();

            Bus.SubscribeHandler<Fault<Start>>(faultReceived.Set);

            var message = new Start();
            Bus.Publish(message);

            Assert.IsTrue(faultReceived.IsAvailable(8.Seconds()));
            Assert.AreEqual(message.CorrelationId, faultReceived.Message.FailedMessage.CorrelationId);
        }

        [Test]
        public void Should_be_received_as_a_fault_message_as_well()
        {
            var faultReceived = new FutureMessage<Fault<Create>>();

            Bus.SubscribeHandler<Fault<Create>>(faultReceived.Set);

            var message = new Create();
            Bus.Publish(message);

            Assert.IsTrue(faultReceived.IsAvailable(Debugger.IsAttached ? 5.Minutes() : 8.Seconds()));
            Assert.AreEqual(message.CorrelationId, faultReceived.Message.FailedMessage.CorrelationId);
        }

        protected override void ConfigureSubscriptions(SubscriptionBusServiceConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository);
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
        }


        class TestStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                State(() => Running);
                Event(() => Started);
                Event(() => Created);

                Initially(
                    When(Started)
                        .Then(instance => { throw new NotSupportedException("This is expected, but nonetheless exceptional"); })
                        .TransitionTo(Running),
                    When(Created)
                        .Then((instance,msg) => { throw new NotSupportedException("This is expected, but nonetheless exceptional"); })
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<Create> Created { get; private set; }
        }


        class Start :
            CorrelatedBy<Guid>
        {
            public Start()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Guid CorrelationId { get; set; }
        }

        class Create :
            CorrelatedBy<Guid>
        {
            public Create()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Guid CorrelationId { get; set; }
        }


        class StartupComplete
        {
            public Guid TransactionId { get; set; }
        }
    }
}