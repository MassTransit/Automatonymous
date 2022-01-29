namespace Automatonymous.Tests
{
    using System;
    using Graphing;
    using NUnit.Framework;
    using Visualizer;


    [TestFixture]
    public class Compare_difference_in_compositeevent_assignment
    {
        [Test]
        public void Should_show_the_differences()
        {
            var dotsNotAssigned = new StateMachineGraphvizGenerator(new TestStateMachine(false).GetGraph()).CreateDotFile();
            var dotsAssigned = new StateMachineGraphvizGenerator(new TestStateMachine(true).GetGraph()).CreateDotFile();
            Console.WriteLine(dotsNotAssigned);
            Console.WriteLine(dotsAssigned);
            var expectedNotAssigned = ExpectedNotAssigned.Replace("\r", "").Replace("\n", Environment.NewLine);
            var expectedAssigned = ExpectedAssigned.Replace("\r", "").Replace("\n", Environment.NewLine);
            Assert.AreNotEqual(dotsNotAssigned, dotsAssigned);
            Assert.AreEqual(expectedNotAssigned, dotsNotAssigned);
            Assert.AreEqual(expectedAssigned, dotsAssigned);
        }

        const string ExpectedNotAssigned = @"digraph G {
0 [shape=ellipse, label=""Initial""];
1 [shape=ellipse, label=""Waiting""];
2 [shape=ellipse, label=""Final""];
3 [shape=rectangle, label=""Start""];
4 [shape=rectangle, label=""First""];
5 [shape=rectangle, label=""Third""];
6 [shape=rectangle, label=""Second""];
0 -> 3;
1 -> 4;
1 -> 6;
2 -> 4;
2 -> 6;
3 -> 1;
4 -> 5;
5 -> 2;
6 -> 5;
}";

        const string ExpectedAssigned = @"digraph G {
0 [shape=ellipse, label=""Initial""];
1 [shape=ellipse, label=""Waiting""];
2 [shape=ellipse, label=""Final""];
3 [shape=rectangle, label=""Start""];
4 [shape=rectangle, label=""First""];
5 [shape=rectangle, label=""Third""];
6 [shape=rectangle, label=""Second""];
0 -> 3;
1 -> 4;
1 -> 6;
3 -> 1;
4 -> 5;
5 -> 2;
6 -> 5;
}";


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
                        }),
                    When(Third, context => context.Instance.SecondFirst)
                        .Then(context =>
                        {
                            context.Instance.Called = true;
                            context.Instance.CalledAfterAll = true;
                        })
                        .Finalize()
                    );

                if (specificallyAssignedToWaiting)
                    CompositeEvent(() => Third, x => Equals(x, Waiting), x => x.CompositeStatus, First, Second);
                else
                    CompositeEvent(() => Third, x => x.CompositeStatus, First, Second);
            }

            public State Waiting { get; private set; }

            public Event Start { get; private set; }

            public Event First { get; private set; }
            public Event Second { get; private set; }
            public Event Third { get; private set; }
        }
    }
}
