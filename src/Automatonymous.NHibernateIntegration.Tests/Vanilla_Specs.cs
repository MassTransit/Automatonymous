// Copyright 2011-2014 Chris Patterson, Dru Sellers
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
namespace Automatonymous.NHibernateIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NHibernate;
    using NHibernate.Mapping.ByCode;
    using NHibernate.Mapping.ByCode.Conformist;
    using NUnit.Framework;
    using UserTypes;


    [TestFixture, Explicit]
    public class When_using_NHibernateRepository
    {
        SuperShopper _machine;
        ISessionFactory _sessionFactory;

        [TestFixtureSetUp]
        public void Setup()
        {
            _machine = new SuperShopper();
            AutomatonymousStateUserType<SuperShopper>.SaveAsString(_machine);

            _sessionFactory = new SQLiteSessionFactoryProvider(typeof(ShoppingChoreMap))
                .GetSessionFactory();
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            _sessionFactory.Dispose();
        }

        async Task RaiseEvent<T>(Guid id, Event<T> @event, T data)
        {
            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var instance = session.Get<ShoppingChore>(id, LockMode.Upgrade);
                if (instance == null)
                {
                    instance = new ShoppingChore(id);
                }

                await _machine.RaiseEvent(instance, @event, data);

                session.SaveOrUpdate(instance);

                transaction.Commit();
            }
        }

        async Task<ShoppingChore> GetSaga(Guid id)
        {
            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var result = session.QueryOver<ShoppingChore>()
                    .Where(x => x.CorrelationId == id)
                    .SingleOrDefault<ShoppingChore>();

                transaction.Commit();

                return result;
            }
        }


        class ShoppingChoreMap :
            ClassMapping<ShoppingChore>
        {
            public ShoppingChoreMap()
            {
                Lazy(false);
                Table("ShoppingChore");

                Id(x => x.CorrelationId, x => x.Generator(Generators.Assigned));

                this.StateProperty<ShoppingChore, SuperShopper>(x => x.CurrentState);

                this.CompositeEventProperty(x => x.Everything);

                Property(x => x.Screwed);
            }
        }


        /// <summary>
        ///     Why to exit the door to go shopping
        /// </summary>
        class GirlfriendYelling
        {
            public Guid CorrelationId { get; set; }
        }


        class GotHitByACar
        {
            public Guid CorrelationId { get; set; }
        }


        class ShoppingChore
        {
            protected ShoppingChore()
            {
            }

            public ShoppingChore(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
            public CompositeEventStatus Everything { get; set; }
            public bool Screwed { get; set; }
        }


        class SuperShopper :
            AutomatonymousStateMachine<ShoppingChore>
        {
            public SuperShopper()
            {
                InstanceState(x => x.CurrentState);

                State(() => OnTheWayToTheStore);

                Event(() => ExitFrontDoor);
                Event(() => GotHitByCar);

                Event(() => EndOfTheWorld, x => x.Everything, CompositeEventOptions.IncludeInitial, ExitFrontDoor, GotHitByCar);

                Initially(
                    When(ExitFrontDoor)
                        .Then(context => Console.Write("Leaving!"))
                        .TransitionTo(OnTheWayToTheStore));

                During(OnTheWayToTheStore,
                    When(GotHitByCar)
                        .Then(context => Console.WriteLine("Ouch!!"))
                        .Finalize());

                DuringAny(
                    When(EndOfTheWorld)
                        .Then(context => Console.WriteLine("Screwed!!"))
                        .Then(context => context.Instance.Screwed = true));
            }

            public Event<GirlfriendYelling> ExitFrontDoor { get; private set; }
            public Event<GotHitByACar> GotHitByCar { get; private set; }
            public Event EndOfTheWorld { get; private set; }

            public State OnTheWayToTheStore { get; private set; }
        }


        [Test]
        public async void Should_have_the_state_machine()
        {
            Guid correlationId = Guid.NewGuid();

            await RaiseEvent(correlationId, _machine.ExitFrontDoor, new GirlfriendYelling
            {
                CorrelationId = correlationId
            });

            await RaiseEvent(correlationId, _machine.GotHitByCar, new GotHitByACar
            {
                CorrelationId = correlationId
            });

            ShoppingChore instance = await GetSaga(correlationId);

            Assert.IsTrue(instance.Screwed);
        }
    }
}