namespace Automatonymous.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_specifying_a_condition_on_a_composite_event
    {
        [Test]
        public async Task Should_call_when_met()
        {
            _machine = new TestStateMachine(false);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.Second);
            await _machine.RaiseEvent(_instance, _machine.First);

            Assert.IsTrue(_instance.Called);
            Assert.IsTrue(_instance.SecondFirst);
        }

        [Test]
        public async Task Should_skip_when_not_met()
        {
            _machine = new TestStateMachine(false);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);
            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsFalse(_instance.Called);
            Assert.IsFalse(_instance.SecondFirst);
        }

        [Test]
        public async Task Assigned_to_specific_state_should_call_when_met()
        {
            _machine = new TestStateMachine(true);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.Second);
            await _machine.RaiseEvent(_instance, _machine.First);

            Assert.IsTrue(_instance.Called);
            Assert.IsTrue(_instance.SecondFirst);
        }

        [Test]
        public async Task Assigned_to_specific_state_should_skip_when_not_met()
        {
            _machine = new TestStateMachine(true);
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);
            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsFalse(_instance.Called);
            Assert.IsFalse(_instance.SecondFirst);
        }


        TestStateMachine _machine;
        Instance _instance;


        class Instance
        {
            public CompositeEventStatus CompositeStatus { get; set; }
            public bool Called { get; set; }
            public bool CalledAfterAll { get; set; }
            public State CurrentState { get; set; }
            public bool SecondFirst { get; set; }
            public bool First { get; set; }
            public bool Second { get; set; }
        }


        sealed class TestStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public TestStateMachine(bool specificallyAssignedToWaiting)
            {
                Initially(
                    When(Start)
                        .TransitionTo(Waiting));

                During(Waiting,
                    When(First)
                        .Then(context =>
                        {
                            context.Instance.First = true;
                            context.Instance.CalledAfterAll = false;
                        }),
                    When(Second)
                        .Then(context =>
                        {
                            context.Instance.SecondFirst = !context.Instance.First;
                            context.Instance.Second = true;
                            context.Instance.CalledAfterAll = false;
                        })
                    );

                if (specificallyAssignedToWaiting)
                    CompositeEvent(() => Third, x => Equals(x, Waiting), x => x.CompositeStatus, First, Second);
                else
                    CompositeEvent(() => Third, x => x.CompositeStatus, First, Second);

                During(Waiting,
                    When(Third, context => context.Instance.SecondFirst)
                        .Then(context =>
                        {
                            context.Instance.Called = true;
                            context.Instance.CalledAfterAll = true;
                        })
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
