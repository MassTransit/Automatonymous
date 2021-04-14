namespace Automatonymous.Tests
{
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;


    [TestFixture]
    public class Using_an_asynchronous_activity
    {
        [Test]
        public async Task Should_capture_the_value()
        {
            var claim = new TestInstance();
            var machine = new TestStateMachine();

            await machine.RaiseEvent(claim, machine.Create, new CreateInstance());

            Assert.AreEqual("ExecuteAsync", claim.Value);
        }


        class TestInstance
        {
            public State CurrentState { get; set; }
            public string Value { get; set; }
        }


        class SetValueAsyncActivity :
            Activity<TestInstance, CreateInstance>
        {
            async Task Activity<TestInstance, CreateInstance>.Execute(BehaviorContext<TestInstance, CreateInstance> context,
                Behavior<TestInstance, CreateInstance> next)
            {
                context.Instance.Value = "ExecuteAsync";
            }

            Task Activity<TestInstance, CreateInstance>.Faulted<TException>(
                BehaviorExceptionContext<TestInstance, CreateInstance, TException> context,
                Behavior<TestInstance, CreateInstance> next)
            {
                return next.Faulted(context);
            }

            void Visitable.Accept(StateMachineVisitor visitor)
            {
                visitor.Visit(this);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        class CreateInstance
        {
            public int X { get; set; }
            public int Y { get; set; }
        }


        class TestStateMachine :
            AutomatonymousStateMachine<TestInstance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                During(Initial,
                    When(Create)
                        .Execute(context => new SetValueAsyncActivity())
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }

            public Event<CreateInstance> Create { get; private set; }
        }
    }
}
