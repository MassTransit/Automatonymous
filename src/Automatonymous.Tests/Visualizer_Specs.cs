﻿// Copyright 2011 Chris Patterson, Dru Sellers
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
    using Xunit;
    //using Visualizer;


    
    public class When_visualizing_a_state_machine
    {
        InstanceStateMachine _machine;
        StateMachineGraph _graph;

        public When_visualizing_a_state_machine()
        {
            _machine = new InstanceStateMachine();

            _graph = _machine.GetGraph();
        }

        [Fact]
        public void Should_parse_the_graph()
        {
            Assert.NotNull(_graph);
        }

        // oh noes, the visualizzzer doesn't work in FULL SCREEN METRO STYLEZ
        //[Fact]
        //[Explicit]
        //public void Should_show_the_goods()
        //{
        //    StateMachineDebugVisualizer.TestShowVisualizer(_graph);
        //}


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
                        .Try(x => x.TransitionTo(Running),
                            c => c.Handle<Exception>(h => h.TransitionTo(Failed))));

                During(Running,
                    When(Finished)
                        .TransitionTo(Final),
                    When(Suspend)
                        .TransitionTo(Suspended));

                During(Suspended,
                    When(Resume)
                        .TransitionTo(Running));

                During(Failed,
                    When(Restart, x => x.Name != null)
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
    }
}