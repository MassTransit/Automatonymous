namespace Automatonymous.Tests
{
    using NUnit.Framework;


    [TestFixture]
    public class When_using_an_instance_lift
    {
        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(_machine.Running, _instance.CurrentState);
        }

        Instance _instance;
        InstanceStateMachine _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            InstanceLift<InstanceStateMachine> instanceLift = _machine.CreateInstanceLift(_instance);

            instanceLift.Raise(_machine.Initialized)
                .Wait();
        }


        class Instance
        {
            public State CurrentState { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }
        }
    }


    [TestFixture]
    public class When_using_an_instance_lift_with_data
    {
        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(_machine.Running, _instance.CurrentState);
        }

        Instance _instance;
        InstanceStateMachine _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            InstanceLift<InstanceStateMachine> instanceLift = _machine.CreateInstanceLift(_instance);

            instanceLift.Raise(_machine.Initialized, new Init())
                .Wait();
        }


        class Init
        {
        }


        class Instance
        {
            public State CurrentState { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }

            public Event<Init> Initialized { get; private set; }
        }
    }
}
