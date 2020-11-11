namespace Automatonymous.Tests.DynamicModify
{
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class When_using_an_instance_lift
    {
        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(Running, _instance.CurrentState);
        }

        State Running;
        Event Initialized;
        Instance _instance;
        StateMachine<Instance> _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = AutomatonymousStateMachine<Instance>
                .Create(builder => builder
                    .State("Running", out Running)
                    .Event("Initialized", out Initialized)
                    .During(builder.Initial)
                        .When(Initialized, b => b.TransitionTo(Running))
                );

            InstanceLift<StateMachine<Instance>> instanceLift = _machine.CreateInstanceLift(_instance);

            instanceLift.Raise(Initialized)
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


    [TestFixture(Category = "Dynamic Modify")]
    public class When_using_an_instance_lift_with_data
    {
        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(Running, _instance.CurrentState);
        }

        State Running;
        Event<Init> Initialized;
        Instance _instance;
        StateMachine<Instance> _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = AutomatonymousStateMachine<Instance>
                .Create(builder => builder
                    .State("Running", out Running)
                    .Event("Initialized", out Initialized)
                    .During(builder.Initial)
                        .When(Initialized, b => b.TransitionTo(Running))
                );

            InstanceLift<StateMachine<Instance>> instanceLift = _machine.CreateInstanceLift(_instance);

            instanceLift.Raise(Initialized, new Init())
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
