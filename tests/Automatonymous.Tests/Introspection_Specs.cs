namespace Automatonymous.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Introspection;
    using NUnit.Framework;


    [TestFixture]
    public class Introspection_Specs
    {
        [Test]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = _machine.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        [Test]
        public void The_machine_shoud_report_its_instance_type()
        {
            Assert.AreEqual(typeof(Instance), ((StateMachine<Instance>)_machine).InstanceType);
        }

        [Test]
        public void The_machine_should_expose_all_events()
        {
            var events = _machine.Events.ToList();

            Assert.AreEqual(4, events.Count);
            Assert.Contains(_machine.Ignored, events);
            Assert.Contains(_machine.Handshake, events);
            Assert.Contains(_machine.Hello, events);
            Assert.Contains(_machine.YelledAt, events);
        }

        [Test]
        public void The_machine_should_expose_all_states()
        {
            Assert.AreEqual(5, ((StateMachine)_machine).States.Count());
            Assert.Contains(_machine.Initial, _machine.States.ToList());
            Assert.Contains(_machine.Final, _machine.States.ToList());
            Assert.Contains(_machine.Greeted, _machine.States.ToList());
            Assert.Contains(_machine.Loved, _machine.States.ToList());
            Assert.Contains(_machine.Pissed, _machine.States.ToList());
        }

        [Test]
        public async Task The_next_events_should_be_known()
        {
            List<Event> events = (await _machine.NextEvents(_instance)).ToList();
            Assert.AreEqual(3, events.Count);
        }

        Instance _instance;
        TestStateMachine _machine;

        [OneTimeSetUp]
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


        class Instance
        {
            public State CurrentState { get; set; }
        }


        class TestStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Initially(
                    When(Hello)
                        .TransitionTo(Greeted));

                During(Greeted,
                    When(Handshake)
                        .TransitionTo(Loved),
                    When(Ignored)
                        .TransitionTo(Pissed));

                WhenEnter(Greeted, x => x.Then(context => { }));

                DuringAny(When(YelledAt).TransitionTo(Final));
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
