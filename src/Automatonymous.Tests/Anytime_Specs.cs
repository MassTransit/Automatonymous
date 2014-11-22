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
        public void Should_not_be_handled_on_initial()
        {
            var instance = new Instance();

            _machine.RaiseEvent(instance, x => x.Hello);

            Assert.IsFalse(instance.HelloCalled);
            Assert.AreEqual(_machine.Initial, instance.CurrentState);
        }

        [Test]
        public void Should_be_called_regardless_of_state()
        {
            var instance = new Instance();

            _machine.RaiseEvent(instance, x => x.Init);
            _machine.RaiseEvent(instance, x => x.Hello);

            Assert.IsTrue(instance.HelloCalled);
            Assert.AreEqual(_machine.Final, instance.CurrentState);
        }

        [Test]
        public void Should_have_value_of_event_data()
        {
            var instance = new Instance();

            _machine.RaiseEvent(instance, x => x.Init);
            _machine.RaiseEvent(instance, x => x.EventA, new A
            {
                Value = "Test"
            });

            Assert.AreEqual("Test", instance.AValue);
            Assert.AreEqual(_machine.Final, instance.CurrentState);
        }

        TestStateMachine _machine;

        [TestFixtureSetUp]
        public void A_state_is_declared()
        {
            _machine = new TestStateMachine();
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
                Initially(
                    When(Init)
                    .TransitionTo(Ready));

                DuringAny(
                    When(Hello)
                        .Then(instance => instance.HelloCalled = true)
                        .Finalize(),
                    When(EventA)
                        .Then((instance, a) => instance.AValue = a.Value)
                        .Finalize());
            }

            public State Ready { get; private set; }

            public Event Init { get; private set; }
            public Event Hello { get; private set; }
            public Event<A> EventA { get; private set; }
        }
    }
}