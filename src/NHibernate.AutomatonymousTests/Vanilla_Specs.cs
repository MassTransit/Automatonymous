using System;

namespace NHibernate.AutomatonymousTests
{
    using Automatonymous;
    using Cfg;
    using Dialect;
    using Mapping.ByCode;
    using MassTransit;
    using MassTransit.NHibernateIntegration.Saga;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using Automatonymous.NHibernateIntegration;
    using NUnit.Framework;


    class NHibernateTestFactory
    {
        public static ISagaRepository<When_using_NHibernateRepository.ShoppingChore> CreateRepositoryFor(StateMachine<When_using_NHibernateRepository.ShoppingChore> machine)
        {
            var cfg = new Configuration();
            cfg.DataBaseIntegration(c =>
                {
                    c.Dialect<SQLiteDialect>();
                    c.ConnectionString = "Data Source=:memory:;Version=3;New=True;Pooling=True;Max Pool Size=1;";
                    c.SchemaAction = SchemaAutoAction.Create;
                });
            var mapper = new ModelMapper();
            mapper.Class<When_using_NHibernateRepository.ShoppingChore>(c =>
                {
                    c.StateProperty<When_using_NHibernateRepository.ShoppingChore, When_using_NHibernateRepository.SuperShopper>(
                        x => x.CurrentState);

                    // Id
                    // CompositeStateProperty

                    // download SQLite!
                });

            cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

            return new NHibernateSagaRepository<When_using_NHibernateRepository.ShoppingChore>(cfg.BuildSessionFactory());
        }
    }

    public class When_using_NHibernateRepository
    {
        SuperShopper machine;
        SagaTest<BusTestScenario, ShoppingChore> test;

        [Test]
        public void Should_receive_ExitFrontDoor()
        {
            Assert.That(test.Received.Any<GirlfriendYelling>(),
                Is.True);
        }

        public When_using_NHibernateRepository()
        {
            machine = new SuperShopper();
            test = TestFactory.ForSaga<ShoppingChore>().New(x =>
                {
                    x.UseStateMachineBuilder(machine);

                    x.UseSagaRepository(NHibernateTestFactory.CreateRepositoryFor(machine));

                    x.Publish(new GirlfriendYelling
                        {
                            CorrelationId = "GET GOING ALREADY! I'm not having this discussion again!"
                        });
                });

            test.Execute();
        }

        internal class SuperShopper
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


        internal class ShoppingChore
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

            public Guid CorrelationId { get; set; }
            public IServiceBus Bus { get; set; }
            public State CurrentState { get; set; }
        }

        /// <summary>
        /// Why to exit the door to go shopping
        /// </summary>
        internal class GirlfriendYelling
            : CorrelatedBy<string>
        {
            public string CorrelationId { get; set; }
        }


        internal class GotHitByACar
            : CorrelatedBy<string>
        {
            public string CorrelationId { get; set; }
        }
    }


}
