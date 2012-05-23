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
    public class Anytime_events
    {
        [Test]
        public void Should_be_called_regardless_of_state()
        {
            Assert.IsTrue(_instance.HelloCalled);
        }

        [Test]
        public void Should_have_value_of_event_data()
        {
            Assert.AreEqual("Test", _instance.AValue);
        }

        TestStateMachine _machine;
        Instance _instance;

        [TestFixtureSetUp]
        public void A_state_is_declared()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();

            _machine.RaiseEvent(_instance, x => x.Hello);
            _machine.RaiseEvent(_instance, x => x.EventA, new A
                {
                    Value = "Test"
                });
        }


        class A
        {
            public string Value { get; set; }
        }


        class Instance
        {
            public bool HelloCalled { get; set; }
            public string AValue { get; set; }
            public State CurrentState { get; set; }
        }


        class TestStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public TestStateMachine()
			{
				InstanceStatePropertyAccessor(x => x.CurrentState);

                Event(() => Hello);
                Event(() => EventA);

                DuringAny(
                    When(Hello)
                        .Then(instance => instance.HelloCalled = true)
                        .Finalize(),
                    When(EventA)
                        .Then((instance, a) => instance.AValue = a.Value)
                        .Finalize());
            }

            public Event Hello { get; private set; }
            public Event<A> EventA { get; private set; }
        }
    }
}