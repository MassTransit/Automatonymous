namespace Automatonymous.Tests.DynamicModify
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class Raising_an_event_within_an_event
    {
        [Test]
        public async Task Should_include_payload()
        {
            Event<Data> Thing = null;
            State True = null;

            var instance = new Instance();
            var machine = AutomatonymousStateMachine<Instance>
                .New(builder => builder
                    .State("True", out True)
                    .State("False", out State False)
                    .Event("Thing", out Thing)
                    .Event("Initialize", out Event Initialize)
                    .During(builder.Initial)
                        .When(Thing, context => context.Data.Condition, b => b
                            .TransitionTo(True)
                            .Then(context => context.Raise(Initialize)))
                        .When(Thing, context => !context.Data.Condition, b => b
                            .TransitionTo(False))
                    .DuringAny()
                        .When(Initialize, b => b
                            .Then(context => context.Instance.Initialized = DateTime.Now))
                );

            await machine.RaiseEvent(instance, Thing, new Data
            {
                Condition = true
            });
            Assert.AreEqual(True, instance.CurrentState);
            Assert.IsTrue(instance.Initialized.HasValue);
        }


        class Instance
        {
            public State CurrentState { get; set; }
            public DateTime? Initialized { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                During(Initial,
                    When(Thing, context => context.Data.Condition)
                        .TransitionTo(True)
                        .Then(context => context.Raise(Initialize)),
                    When(Thing, context => !context.Data.Condition)
                        .TransitionTo(False));

                DuringAny(
                    When(Initialize)
                        .Then(context => context.Instance.Initialized = DateTime.Now));
            }

            public State True { get; private set; }
            public State False { get; private set; }

            public Event<Data> Thing { get; private set; }
            public Event Initialize { get; private set; }
        }


        class Data
        {
            public bool Condition { get; set; }
        }
    }
}
