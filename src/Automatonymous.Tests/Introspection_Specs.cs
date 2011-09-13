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
            Assert.AreEqual(3, events.Count());
        }


        [Test]
        public void The_machine_should_expose_all_events()
        {
            Assert.AreEqual(4, _machine.Events.Count());
            Assert.Contains(_machine.Ignored, _machine.Events.ToList());
            Assert.Contains(_machine.Handshake, _machine.Events.ToList());
            Assert.Contains(_machine.Hello, _machine.Events.ToList());
            Assert.Contains(_machine.YelledAt, _machine.Events.ToList());
        }

        [Test]
        public void The_machine_should_expose_all_states()
        {
            Assert.AreEqual(5, _machine.States.Count());
            Assert.Contains(_machine.Initial, _machine.States.ToList());
            Assert.Contains(_machine.Completed, _machine.States.ToList());
            Assert.Contains(_machine.Greeted, _machine.States.ToList());
            Assert.Contains(_machine.Loved, _machine.States.ToList());
            Assert.Contains(_machine.Pissed, _machine.States.ToList());
        }

        [Test]
        public void The_machine_shoud_report_its_instance_type()
        {
            Assert.AreEqual(typeof(Instance), _machine.InstanceType);
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
            StateMachine<Instance>
        {
            public TestStateMachine()
            {
                Event(() => Hello);
                Event(() => YelledAt);
                Event(() => Handshake);
                Event(() => Ignored);

                State(() => Greeted);
                State(() => Loved);
                State(() => Pissed);

                Initially(
                    When(Hello)
                        .TransitionTo(Greeted));

                During(Greeted,
                       When(Handshake)
                           .TransitionTo(Loved),
                       When(Ignored)
                           .TransitionTo(Pissed));

                Anytime(When(YelledAt).TransitionTo(Completed));
            }

            public State Greeted { get; set; }
            public State Pissed { get; set; }
            public State Loved { get; set; }

            public Event Hello { get; private set; }
            public Event YelledAt { get; private set; }
            public Event<A> Handshake { get; private set; }
            public Event<B> Ignored { get; private set; }
        }
    }
}