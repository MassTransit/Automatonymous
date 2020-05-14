namespace Automatonymous.Tests
{
    using System;
    using Events;
    using GreenPipes;
    using GreenPipes.Introspection;
    using NUnit.Framework;


    [TestFixture]
    public class When_an_event_is_declared
    {
        [Test]
        public void It_should_capture_a_simple_event_name()
        {
            Assert.AreEqual("Hello", _machine.Hello.Name);
        }

        [Test]
        public void It_should_capture_the_data_event_name()
        {
            Assert.AreEqual("EventA", _machine.EventA.Name);
        }

        [Test]
        public void It_should_create_the_event_for_the_value_type()
        {
            Assert.IsInstanceOf<DataEvent<int>>(_machine.EventInt);
        }

        [Test]
        public void It_should_create_the_proper_event_type_for_data_events()
        {
            Assert.IsInstanceOf<DataEvent<A>>(_machine.EventA);
        }

        [Test]
        public void It_should_create_the_proper_event_type_for_simple_events()
        {
            Assert.IsInstanceOf<TriggerEvent>(_machine.Hello);
        }

        [Test]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = _machine.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        TestStateMachine _machine;


        class Instance
        {
            public State CurrentState { get; set; }
        }


        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = new TestStateMachine();
        }


        class A
        {
        }


        class TestStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public Event Hello { get; private set; }
            public Event<A> EventA { get; private set; }
            public Event<int> EventInt { get; private set; }
        }
    }
}
