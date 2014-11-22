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
namespace NHibernate.AutomatonymousTests
{
    using System;
    using System.Linq;
    using Automatonymous;
    using Automatonymous.NHibernateIntegration;
    using Automatonymous.RepositoryBuilders;
    using MassTransit;
    using MassTransit.NHibernateIntegration;
    using MassTransit.NHibernateIntegration.Saga;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;


    [TestFixture, Explicit]
    public class When_using_NHibernateRepository
    {
        [Test]
        public void Should_have_a_saga()
        {
            ShoppingChore shoppingChore = _test.Saga.Created.Contains(_correlationId);
            Assert.IsNotNull(shoppingChore);
        }


        [Test]
        public void Should_have_a_saga_in_the_proper_state()
        {
            ShoppingChore shoppingChore = _test.Saga.ContainsInState(_correlationId, _machine.Final, _machine);

            foreach (ShoppingChore result in _repository.Select(x => x))
                Console.WriteLine("{0} - {1} ({2})", result.CorrelationId, result.CurrentState, result.Screwed);

            Assert.IsNotNull(shoppingChore);
        }

        [Test]
        public void Should_have_heard_girlfriend_yelling()
        {
            Assert.IsTrue(_test.Received.Any<GirlfriendYelling>());
        }

        [Test]
        public void Should_have_heard_her_yelling_to_the_end_of_the_world()
        {
            bool shoppingChore = _test.Saga.Created.Any(x => x.CorrelationId == _correlationId && x.Screwed);
            Assert.IsNotNull(shoppingChore);
        }

        [Test]
        public void Should_have_heard_the_impact()
        {
            Assert.IsTrue(_test.Received.Any<GotHitByACar>());
        }

        SuperShopper _machine;
        SagaTest<BusTestScenario, ShoppingChore> _test;
        ISessionFactory _sessionFactory;
        ISagaRepository<ShoppingChore> _repository;
        ISagaRepository<ShoppingChore> _stateMachineRepository;
        Guid _correlationId;

        [TestFixtureSetUp]
        public void Setup()
        {
            _machine = new SuperShopper();
            AutomatonymousStateUserType<SuperShopper>.SaveAsString(_machine);

            _sessionFactory = new SqlLiteSessionFactoryProvider(typeof(ShoppingChoreMap)).GetSessionFactory();
            _repository = new NHibernateSagaRepository<ShoppingChore>(_sessionFactory);
            _stateMachineRepository = new AutomatonymousStateMachineSagaRepository<ShoppingChore>(_repository,
                x => false, Enumerable.Empty<StateMachineEventCorrelation<ShoppingChore>>());
            _correlationId = NewId.NextGuid();

            _test = TestFactory.ForSaga<ShoppingChore>().New(x =>
                {
                    x.UseStateMachineBuilder(_machine);

                    x.UseSagaRepository(_stateMachineRepository);

                    x.Publish(new GirlfriendYelling
                        {
                            CorrelationId = _correlationId
                        });

                    x.Publish(new GotHitByACar
                        {
                            CorrelationId = _correlationId
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

                this.CompositeEventProperty(x => x.Everything);

                Property(x => x.Screwed);
            }
        }


        /// <summary>
        ///     Why to exit the door to go shopping
        /// </summary>
        class GirlfriendYelling :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class GotHitByACar :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class ShoppingChore :
            SagaStateMachineInstance
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
            public CompositeEventStatus Everything { get; set; }
            public bool Screwed { get; set; }

            public Guid CorrelationId { get; set; }
            public IServiceBus Bus { get; set; }
        }


        class SuperShopper :
            AutomatonymousStateMachine<ShoppingChore>
        {
            public SuperShopper()
            {
                InstanceState(x => x.CurrentState);

                ComposeEvent(EndOfTheWorld, x => x.Everything, ExitFrontDoor, GotHitByCar);

                Initially(
                    When(ExitFrontDoor)
                        .Then(state => Console.Write("Leaving!"))
                        .TransitionTo(OnTheWayToTheStore));

                During(OnTheWayToTheStore,
                    When(GotHitByCar)
                        .Then(state => Console.WriteLine("Ouch!!"))
                        .Finalize());

                DuringAny(
                    When(EndOfTheWorld)
                        .Then(state => Console.WriteLine("Screwed!!"))
                        .Then(state => state.Screwed = true));
            }

            public Event<GirlfriendYelling> ExitFrontDoor { get; private set; }
            public Event<GotHitByACar> GotHitByCar { get; private set; }
            public Event EndOfTheWorld { get; private set; }

            public State OnTheWayToTheStore { get; private set; }
        }
    }
}