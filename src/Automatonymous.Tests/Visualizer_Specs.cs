// Copyright 2011 Chris Patterson, Dru Sellers
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
    [Explicit]
    public class When_visualizing_a_state_machine
    {
        [Test]
        public void Should_show_the_goods()
        {
            var machine = new InstanceStateMachine();

            StateMachineGraph stateMachineGraph = machine.GetGraph();

            StateMachineDebugVisualizer.TestShowVisualizer(stateMachineGraph);
        }


        class Instance :
            StateMachineInstance
        {
            public State CurrentState { get; set; }
        }


        class InstanceStateMachine :
            StateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                State(() => Running);
                State(() => Suspended);
                State(() => Failed);

                Event(() => Initialized);
                Event(() => Finished);
                Event(() => Suspend);
                Event(() => Resume);

                During(Initial,
                    When(Initialized)
                        .Try(x => x.TransitionTo(Running),
                            c => c.Handle<Exception>(h => h.TransitionTo(Failed))));

                During(Running,
                    When(Finished)
                        .TransitionTo(Completed),
                    When(Suspend)
                        .TransitionTo(Suspended));

                During(Suspended,
                    When(Resume)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }
            public State Suspended { get; private set; }
            public State Failed { get; private set; }

            public Event Initialized { get; private set; }
            public Event Suspend { get; private set; }
            public Event Resume { get; private set; }
            public Event Finished { get; private set; }
        }
    }
}