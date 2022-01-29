namespace Automatonymous.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_combining_events_into_a_single_event
    {
        [Test]
        public async Task Should_have_called_combined_event()
        {
            _machine = new TestStateMachine(false);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);
            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsTrue(_instance.Called);
        }

        [Test]
        public async Task Should_not_call_for_one_event()
        {
            _machine = new TestStateMachine(false);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);

            Assert.IsFalse(_instance.Called);
        }

        [Test]
        public async Task Should_not_call_for_one_other_event()
        {
            _machine = new TestStateMachine(false);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsFalse(_instance.Called);
        }

        [Test]
        public async Task Assigned_to_specific_state_should_have_called_combined_event()
        {
            _machine = new TestStateMachine(true);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);
            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsTrue(_instance.Called);
        }

        [Test]
        public async Task Assigned_to_specific_state_should_not_call_for_one_event()
        {
            _machine = new TestStateMachine(true);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);

            Assert.IsFalse(_instance.Called);
        }

        [Test]
        public async Task Assigned_to_specific_state_should_not_call_for_one_other_event()
        {
            _machine = new TestStateMachine(true);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsFalse(_instance.Called);
        }

        TestStateMachine _machine;
        Instance _instance;


        class Instance
        {
            public CompositeEventStatus CompositeStatus { get; set; }
            public bool Called { get; set; }
            public State CurrentState { get; set; }
        }


        sealed class TestStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public TestStateMachine(bool specificallyAssignedToWaiting)
            {
                if (specificallyAssignedToWaiting)
                    CompositeEvent(() => Third, x => Equals(x, Waiting), x => x.CompositeStatus, First, Second);
                else
                    CompositeEvent(() => Third, x => x.CompositeStatus, First, Second);

                Initially(
                    When(Start)
                        .TransitionTo(Waiting));

                During(Waiting,
                    When(Third)
                        .Then(context => context.Instance.Called = true)
                        .Finalize());
            }

            public State Waiting { get; private set; }

            public Event Start { get; private set; }

            public Event First { get; private set; }
            public Event Second { get; private set; }
            public Event Third { get; private set; }
        }
    }


    [TestFixture]
    public class When_combining_events_with_an_int_for_state
    {
        [Test]
        public async Task Should_have_called_combined_event()
        {
            _machine = new TestStateMachine(false);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            Assert.IsFalse(_instance.Called);

            await _machine.RaiseEvent(_instance, _machine.First);
            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsTrue(_instance.Called);

            Assert.AreEqual(2, _instance.CurrentState);
        }

        [Test]
        public async Task Should_have_initial_state_with_zero()
        {
            _machine = new TestStateMachine(false);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            Assert.AreEqual(3, _instance.CurrentState);
        }

        [Test]
        public async Task Should_not_call_for_one_event()
        {
            _machine = new TestStateMachine(false);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);

            Assert.IsFalse(_instance.Called);
        }

        [Test]
        public async Task Should_not_call_for_one_other_event()
        {
            _machine = new TestStateMachine(false);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsFalse(_instance.Called);
        }

        [Test]
        public async Task Assigned_to_specific_state_should_have_called_combined_event()
        {
            _machine = new TestStateMachine(true);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            Assert.IsFalse(_instance.Called);

            await _machine.RaiseEvent(_instance, _machine.First);
            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsTrue(_instance.Called);

            Assert.AreEqual(2, _instance.CurrentState);
        }

        [Test]
        public async Task Assigned_to_specific_state_should_have_initial_state_with_zero()
        {
            _machine = new TestStateMachine(true);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            Assert.AreEqual(3, _instance.CurrentState);
        }

        [Test]
        public async Task Assigned_to_specific_state_should_not_call_for_one_event()
        {
            _machine = new TestStateMachine(true);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);

            Assert.IsFalse(_instance.Called);
        }

        [Test]
        public async Task Assigned_to_specific_state_should_not_call_for_one_other_event()
        {
            _machine = new TestStateMachine(true);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsFalse(_instance.Called);
        }

        TestStateMachine _machine;
        Instance _instance;


        class Instance
        {
            public int CompositeStatus { get; set; }
            public bool Called { get; set; }
            public int CurrentState { get; set; }
        }


        sealed class TestStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public TestStateMachine(bool specificallyAssignedToWaiting)
            {
                InstanceState(x => x.CurrentState);

                if (specificallyAssignedToWaiting)
                    CompositeEvent(() => Third, x => Equals(x, Waiting), x => x.CompositeStatus, First, Second);
                else
                    CompositeEvent(() => Third, x => x.CompositeStatus, First, Second);

                Initially(
                    When(Start)
                        .TransitionTo(Waiting));

                During(Waiting,
                    When(Third)
                        .Then(context => context.Instance.Called = true)
                        .Finalize());
            }

            public State Waiting { get; private set; }

            public Event Start { get; private set; }

            public Event First { get; private set; }
            public Event Second { get; private set; }
            public Event Third { get; private set; }
        }
    }
}
