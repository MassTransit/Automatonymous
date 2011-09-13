namespace Automatonymous.Tests
{
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class Introspection_Specs
    {
        
        [Test]
        public void The_next_events_should_be_known()
        {
            var events = _machine.NextEvents(_instance);
            Assert.AreEqual(2, events.Count());
        }


        Instance _instance;
        TestStateMachine _machine;

        [TestFixtureSetUp]
        public void A_state_is_declared()
        {
            _instance = new Instance();
            _machine = new TestStateMachine();

            _machine.RaiseEvent(_instance, _machine.Hello);
        }

        class A
        {
        }
        class B
        {
        }

        class Instance :
            StateMachineInstance
        {
            public State CurrentState { get; set; }
        }

        class TestStateMachine :
            StateMachine<StateMachineInstance>
        {
            public TestStateMachine()
            {
                Event(() => Hello);
                Event(() => Handshake);
                Event(() => Finger);

                State(() => Greeted);
                State(() => Loved);
                State(() => Pissed);

                Initially(
                    When(Hello)
                        .TransitionTo(Greeted));

                During(Greeted,
                       When(Handshake)
                           .TransitionTo(Loved),
                       When(Finger)
                           .TransitionTo(Pissed));
            }

            public State Greeted { get; set; }
            public State Pissed { get; set; }
            public State Loved { get; set; }

            public Event Hello { get; private set; }
            public Event<A> Handshake { get; private set; }
            public Event<B> Finger { get; private set; }
        }
    }
}