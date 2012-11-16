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
namespace NHibernate.AutomatonymousTests
{
    using System;
    using Automatonymous;
    using Automatonymous.RepositoryBuilders;
    using MassTransit;
    using MassTransit.NHibernateIntegration;
    using MassTransit.NHibernateIntegration.Saga;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;


    [TestFixture]
    public class When_using_NHibernateRepository
    {
        [Test]
        public void Should_have_heard_girlfriend_yelling()
        {
            Assert.IsTrue(_test.Received.Any<GirlfriendYelling>());
        }

        SuperShopper _machine;
        SagaTest<BusTestScenario, ShoppingChore> _test;
        ISessionFactory _sessionFactory;
        ISagaRepository<ShoppingChore> _repository;
        ISagaRepository<ShoppingChore> _stateMachineRepository;

        [TestFixtureSetUp]
        public void Setup()
        {
            _machine = new SuperShopper();
            _sessionFactory = new SqlLiteSessionFactoryProvider(typeof(ShoppingChoreMap)).GetSessionFactory();
            _repository = new NHibernateSagaRepository<ShoppingChore>(_sessionFactory);
            _stateMachineRepository = new AutomatonymousStateMachineSagaRepository<ShoppingChore>(_repository,
                x => x.CurrentState == _machine.Final, new StateMachineEventCorrelation<ShoppingChore>[] {});

            _test = TestFactory.ForSaga<ShoppingChore>().New(x =>
                {
                    x.UseStateMachineBuilder(_machine);

                    x.UseSagaRepository(_stateMachineRepository);

                    x.Publish(new GirlfriendYelling
                                  {
                                      CorrelationId = NewId.NextGuid()
                                  });
                });

            _test.Execute();
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            _test.Dispose();
            _sessionFactory.Dispose();
        }


        class ShoppingChoreMap :
            SagaClassMapping<ShoppingChore>
        {
            public ShoppingChoreMap()
            {
                Lazy(false);
                Table("ShoppingChore");

                this.StateProperty<ShoppingChore, SuperShopper>(x => x.CurrentState);

                //this.CompositeEventProperty(x => x.);
            }
        }


        /// <summary>
        ///     Why to exit the door to go shopping
        /// </summary>
        class GirlfriendYelling
            : CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class GotHitByACar
            : CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class ShoppingChore
            : SagaStateMachineInstance
        {
            [Obsolete("for serialization")]
            protected ShoppingChore()
            {
            }

            public ShoppingChore(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public State CurrentState { get; set; }

            public Guid CorrelationId { get; set; }
            public IServiceBus Bus { get; set; }
        }


        class SuperShopper
            : AutomatonymousStateMachine<ShoppingChore>
        {
            public SuperShopper()
            {
                InstanceState(x => x.CurrentState);

                State(() => OnTheWayToTheStore);

                Event(() => ExitFrontDoor);
                Event(() => GotHitByCar);

                Initially(
                    When(ExitFrontDoor)
                        .TransitionTo(OnTheWayToTheStore));

                During(OnTheWayToTheStore,
                    When(GotHitByCar)
                        .Finalize());
            }

            public Event<GirlfriendYelling> ExitFrontDoor { get; private set; }
            public Event<GotHitByACar> GotHitByCar { get; private set; }

            public State OnTheWayToTheStore { get; private set; }
        }
    }
}