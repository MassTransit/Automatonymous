namespace Automatonymous.Tests.DynamicModify
{
    using System;
    using System.Threading.Tasks;
    using Activities;
    using GreenPipes;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class Having_an_activity_with_faulted_handler
    {
        [Test]
        public void Should_capture_the_value()
        {
            var data = new CreateClaim
            {
                X = 56,
                Y = 23,
            };

            Assert.That(async () => await _machine.RaiseEvent(_claim, Create, data), Throws.TypeOf<EventExecutionException>());

            Assert.AreEqual(default, _claim.Value);
        }

        Event<CreateClaim> Create;
        ClaimAdjustmentInstance _claim;
        StateMachine<ClaimAdjustmentInstance> _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            State Running = null;

            _claim = new ClaimAdjustmentInstance();
            _machine = AutomatonymousStateMachine<ClaimAdjustmentInstance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Create", out Create)
                    .InstanceState(b => b.CurrentState)
                    .During(builder.Initial)
                        .When(Create, b => b
                            .Execute(context => new CalculateValueActivity(new LocalCalculator()))
                            .Execute(context => new ActionActivity<ClaimAdjustmentInstance>(x => throw new Exception()))
                            .TransitionTo(Running))
                );
        }


        class ClaimAdjustmentInstance :
            ClaimAdjustment
        {
            public State CurrentState { get; set; }
            public string Value { get; set; }
        }


        class CalculateValueActivity :
            Activity<ClaimAdjustmentInstance, CreateClaim>
        {
            readonly CalculatorService _calculator;

            public CalculateValueActivity(CalculatorService calculator)
            {
                _calculator = calculator;
            }

            public async Task Execute(BehaviorContext<ClaimAdjustmentInstance, CreateClaim> context,
                Behavior<ClaimAdjustmentInstance, CreateClaim> next)
            {
                var originalValue = context.Instance.Value;
                try
                {
                    context.Instance.Value = _calculator.Add(context.Data.X, context.Data.Y);

                    await next.Execute(context);
                }
                catch (Exception)
                {
                    context.Instance.Value = originalValue;

                    throw;
                }
            }

            public Task Faulted<TException>(BehaviorExceptionContext<ClaimAdjustmentInstance, CreateClaim, TException> context,
                Behavior<ClaimAdjustmentInstance, CreateClaim> next)
                where TException : Exception
            {
                return next.Faulted(context);
            }

            public void Accept(StateMachineVisitor visitor)
            {
                visitor.Visit(this);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        interface ClaimAdjustment :
            ClaimModel
        {
            State CurrentState { get; set; }
        }


        interface ClaimModel
        {
            string Value { get; set; }
        }


        class CreateClaim
        {
            public int X { get; set; }
            public int Y { get; set; }
        }


        interface CalculatorService
        {
            string Add(int x, int y);
        }


        class LocalCalculator :
            CalculatorService
        {
            public string Add(int x, int y)
            {
                return (x + y).ToString();
            }
        }
    }
}
