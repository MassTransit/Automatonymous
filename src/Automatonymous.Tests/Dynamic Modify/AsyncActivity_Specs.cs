namespace Automatonymous.Tests.DynamicModify
{
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class Using_an_asynchronous_activity
    {
        [Test]
        public async Task Should_capture_the_value()
        {
            Event<CreateInstance> Create = null;

            var claim = new TestInstance();
            var machine = AutomatonymousStateMachine<TestInstance>
                .Create(builder => builder
                    .State("Running", out State Running)
                    .Event("Create", out Create)
                    .InstanceState(b => b.CurrentState)
                    .During(builder.Initial)
                        .When(Create, b => b
                            .Execute(context => new SetValueAsyncActivity())
                            .TransitionTo(Running)
                        )
                );

            await machine.RaiseEvent(claim, Create, new CreateInstance());

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
    }
}
