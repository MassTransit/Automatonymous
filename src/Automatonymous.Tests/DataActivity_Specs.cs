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
    public class When_specifying_an_event_activity_with_data
    {
        [Test]
        public void Should_have_the_proper_value()
        {
            Assert.AreEqual("Hello", _instance.Value);
        }

        [Test]
        public void Should_transition_to_the_proper_state()
        {
            Assert.AreEqual(_machine.Running, _instance.CurrentState);
        }

        [Test]
        public void Should_capture_passed_value()
        {
            Assert.AreEqual(47, _instance.OtherValue);
        }

        Instance _instance;
        InstanceStateMachine _machine;

        [TestFixtureSetUp]
        public void Specifying_an_event_activity_with_data()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _machine.RaiseEvent(_instance, _machine.Initialized, new Init
                {
                    Value = "Hello"
                }).Wait();

            _machine.RaiseEvent(_instance, _machine.PassedValue, 47).Wait();
        }


        class Instance
        {
            public string Value { get; set; }
            public int OtherValue { get; set; }
            public State CurrentState { get; set; }
        }


        class Init
        {
            public string Value { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
			{
				InstanceState(x => x.CurrentState);

                State(() => Running);

                Event(() => Initialized);
                Event(() => PassedValue);

                During(Initial,
                       When(Initialized)
                            .Then((instance, data) => instance.Value = data.Value)
                            .TransitionTo(Running));

                During(Running,
                    When(PassedValue)
                        .Then((instance, data) => instance.OtherValue = data));
            }

            public State Running { get; private set; }

            public Event<Init> Initialized { get; private set; }
            public Event<int> PassedValue { get; private set; }
        }
    }
}