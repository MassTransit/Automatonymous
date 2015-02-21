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
    using NUnit.Framework;


    [TestFixture]
    public class Anytime_events
    {
        [Test]
        public async void Should_be_called_regardless_of_state()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, x => x.Init);
            await _machine.RaiseEvent(instance, x => x.Hello);

            Assert.IsTrue(instance.HelloCalled);
            Assert.AreEqual(_machine.Final, instance.CurrentState);
        }

        [Test]
        public async void Should_have_value_of_event_data()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, x => x.Init);
            await _machine.RaiseEvent(instance, x => x.EventA, new A
            {
                Value = "Test"
            });

            Assert.AreEqual("Test", instance.AValue);
            Assert.AreEqual(_machine.Final, instance.CurrentState);
        }

        [Test]
        public void Should_not_be_handled_on_initial()
        {
            var instance = new Instance();

            Assert.Throws<InvalidEventInStateException>(async () => await _machine.RaiseEvent(instance, x => x.Hello));

            Assert.IsFalse(instance.HelloCalled);
            Assert.AreEqual(_machine.Initial, instance.CurrentState);
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
                State(() => Ready);

                Event(() => Init);
                Event(() => Hello);
                Event(() => EventA);

                Initially(
                    When(Init)
                        .TransitionTo(Ready));

                DuringAny(
                    When(Hello)
                        .Then(context => context.Instance.HelloCalled = true)
                        .Finalize(),
                    When(EventA)
                        .Then(context => context.Instance.AValue = context.Data.Value)
                        .Finalize());
            }

            public State Ready { get; private set; }

            public Event Init { get; private set; }
            public Event Hello { get; private set; }
            public Event<A> EventA { get; private set; }
        }
    }
}