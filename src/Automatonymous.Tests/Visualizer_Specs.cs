// Copyright 2011-2015 Chris Patterson, Dru Sellers
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Automatonymous.Tests
{
    using System;
    using Graphing;
    using NUnit.Framework;
    using Visualizer;


    [TestFixture]
    public class When_visualizing_a_state_machine
    {
        InstanceStateMachine _machine;
        StateMachineGraph _graph;

        [TestFixtureSetUp]
        public void Setup()
        {
            _machine = new InstanceStateMachine();

            _graph = _machine.GetGraph();
        }

        const string Expected = @"digraph G {
0 [shape=ellipse, label=""Initial""];
1 [shape=ellipse, label=""Running""];
2 [shape=ellipse, label=""Failed""];
3 [shape=ellipse, label=""Final""];
4 [shape=ellipse, label=""Suspended""];
5 [shape=rectangle, label=""Initialized""];
6 [shape=rectangle, label=""Exception""];
7 [shape=rectangle, label=""Finished""];
8 [shape=rectangle, label=""Suspend""];
9 [shape=rectangle, label=""Resume""];
10 [shape=rectangle, label=""Restart<RestartData>""];
0 -> 5 [ ];
1 -> 7 [ ];
1 -> 8 [ ];
2 -> 10 [ ];
4 -> 9 [ ];
5 -> 1 [ ];
5 -> 6 [ ];
6 -> 2 [ ];
7 -> 3 [ ];
8 -> 4 [ ];
9 -> 1 [ ];
10 -> 1 [ ];
}
";


        class Instance
        {
            public State CurrentState { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                InstanceState(x => x.CurrentState);

                State(() => Running);
                State(() => Suspended);
                State(() => Failed);

                Event(() => Initialized);
                Event(() => Finished);
                Event(() => Suspend);
                Event(() => Resume);
                Event(() => Restart);

                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running)
                        .Catch<Exception>(h => h.TransitionTo(Failed)));

                During(Running,
                    When(Finished)
                        .TransitionTo(Final),
                    When(Suspend)
                        .TransitionTo(Suspended));

                During(Suspended,
                    When(Resume)
                        .TransitionTo(Running));

                During(Failed,
                    When(Restart, context => context.Data.Name != null)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }
            public State Suspended { get; private set; }
            public State Failed { get; private set; }

            public Event Initialized { get; private set; }
            public Event Suspend { get; private set; }
            public Event Resume { get; private set; }
            public Event Finished { get; private set; }

            public Event<RestartData> Restart { get; private set; }
        }


        class RestartData
        {
            public string Name { get; set; }
        }


        [Test]
        public void Should_parse_the_graph()
        {
            Assert.IsNotNull(_graph);
        }

        [Test]
        public void Should_show_the_goods()
        {
            var generator = new StateMachineGraphvizGenerator(_graph);

            string dots = generator.CreateDotFile();

            Console.WriteLine(dots);

            Assert.AreEqual(Expected, dots);
        }
    }
}