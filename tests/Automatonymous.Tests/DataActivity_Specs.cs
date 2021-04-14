namespace Automatonymous.Tests
{
    using NUnit.Framework;


    [TestFixture]
    public class When_specifying_an_event_activity_with_data
    {
        [Test]
        public void Should_capture_passed_value()
        {
            Assert.AreEqual(47, _instance.OtherValue);
        }

        [Test]
        public void Should_have_the_proper_value()
        {
            Assert.AreEqual("Hello", _instance.Value);
        }

        [Test]
        public void Should_transition_to_the_proper_state()
        {
            Assert.AreEqual(_machine.Running, _instance.CurrentState);
        }

        Instance _instance;
        InstanceStateMachine _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity_with_data()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _machine.RaiseEvent(_instance, _machine.Initialized, new Init
            {
                Value = "Hello"
            }).Wait();

            _machine.RaiseEvent(_instance, _machine.PassedValue, 47)
                .Wait();
        }


        class Instance
        {
            public string Value { get; set; }
            public int OtherValue { get; set; }
            public State CurrentState { get; set; }
        }


        class Init
        {
            public string Value { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                During(Initial,
                    When(Initialized)
                        .Then(context => context.Instance.Value = context.Data.Value)
                        .TransitionTo(Running));

                During(Running,
                    When(PassedValue)
                        .Then(context => context.Instance.OtherValue = context.Data));
            }

            public State Running { get; private set; }

            public Event<Init> Initialized { get; private set; }
            public Event<int> PassedValue { get; private set; }
        }
    }
}
