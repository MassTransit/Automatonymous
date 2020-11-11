namespace Automatonymous.Tests.DynamicModify
{
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class When_using_an_event_raiser
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
            _machine = AutomatonymousStateMachine<Instance>.Create(builder => builder
                .State("Running", out Running)
                .Event("Initialized", out Initialized)
                .During(builder.Initial)
                    .When(Initialized, b => b.TransitionTo(Running))
            );

            EventLift<Instance> eventLift = _machine.CreateEventLift(Initialized);

            eventLift.Raise(_instance).Wait();
        }


        class Instance
        {
            public State CurrentState { get; set; }
        }
    }


    [TestFixture(Category = "Dynamic Modify")]
    public class When_using_an_event_raiser_with_data
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
                    .InstanceState(b => b.CurrentState)
                    .During(builder.Initial)
                        .When(Initialized, b => b.TransitionTo(Running))
                );

            EventLift<Instance, Init> eventLift = _machine.CreateEventLift(Initialized);

            eventLift.Raise(_instance, new Init()).Wait();
        }

        class Init { }

        class Instance
        {
            public State CurrentState { get; set; }
        }
    }
}
