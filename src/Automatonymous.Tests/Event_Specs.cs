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
    using Impl;
    using Xunit;


    
    public class When_an_event_is_declared
    {
        [Fact]
        public void It_should_capture_a_simple_event_name()
        {
            Assert.Equal("Hello", _machine.Hello.Name);
        }

        [Fact]
        public void It_should_capture_the_data_event_name()
        {
            Assert.Equal("EventA", _machine.EventA.Name);
        }

        [Fact]
        public void It_should_create_the_proper_event_type_for_data_events()
        {
            Assert.IsType<DataEvent<A>>(_machine.EventA);
        }

        [Fact]
        public void It_should_create_the_proper_event_type_for_simple_events()
        {
            Assert.IsType<SimpleEvent>(_machine.Hello);
        }

        [Fact]
        public void It_should_create_the_event_for_the_value_type()
        {
            Assert.IsType<DataEvent<int>>(_machine.EventInt);
        }

        TestStateMachine _machine;
        
        class Instance
        {
            public State CurrentState { get; set; }
        }

        public When_an_event_is_declared()
        {
            _machine = new TestStateMachine();
        }


        class A
        {
        }


        class TestStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Event(() => Hello);
                Event(() => EventA);
                Event(() => EventInt);
            }

            public Event Hello { get; private set; }
            public Event<A> EventA { get; private set; }
            public Event<int> EventInt { get; private set; }
        }
    }
}