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
    using NUnit.Framework;


    [TestFixture]
    public class When_combining_events_into_a_single_event
    {
        [Test]
        public void Should_have_called_combined_event()
        {
            Assert.IsTrue(_instance.Called);
        }

        TestStateMachine _machine;
        Instance _instance;

        [TestFixtureSetUp]
        public void A_state_is_declared()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();

            _machine.RaiseEvent(_instance, _machine.First);
            _machine.RaiseEvent(_instance, _machine.Second);
        }


        class Instance :
            StateMachineInstance
        {
            public CompositeEventStatus CompositeStatus { get; set; }
            public bool Called { get; set; }
            public State CurrentState { get; set; }
        }


        class TestStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Event(() => First);
                Event(() => Second);

                Event(() => Third, x => x.CompositeStatus, First.And(Second));
                // support OR as well, let the language due deal with order of operations and grouping

                Initially(
                          When(Third)
                              .Then(instance => instance.Called = true));
            }

            public Event First { get; private set; }
            public Event Second { get; private set; }
            public Event Third { get; private set; }
        }
    }
}