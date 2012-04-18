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
    public class When_a_state_is_double_pumped
    {
        [Test]
        public void Should_handle_both_ends()
        {
            Assert.AreEqual("Hello", _instance.Top.Value);
            Assert.AreEqual("Goodbye", _instance.Bottom.OtherValue);
        }

        MyState _instance;
        BottomInstanceStateMachine _bottom;
        TopInstanceStateMachine _top;

        [TestFixtureSetUp]
        public void Specifying_an_event_activity_with_data()
        {
            _instance = new MyState();

            _top = new TopInstanceStateMachine();
            _bottom = new BottomInstanceStateMachine();

            _top.RaiseEvent(_instance.Top, _top.Initialized, new Init
                {
                    Value = "Hello"
                });

            _bottom.RaiseEvent(_instance.Bottom, _bottom.Initialized, new Init
                {
                    Value = "Goodbye"
                });
        }


        class TopInstance :
            StateMachineInstance
        {
            public string Value { get; set; }
            public State CurrentState { get; set; }
        }


        class BottomInstance :
            StateMachineInstance
        {
            public string OtherValue { get; set; }
            public State CurrentState { get; set; }
        }


        class MyState
        {
            public MyState()
            {
                Top = new TopInstance();
                Bottom = new BottomInstance();
            }

            public TopInstance Top { get; private set; }
            public BottomInstance Bottom { get; private set; }
        }


        class Init
        {
            public string Value { get; set; }
        }


        class TopInstanceStateMachine :
            AutomatonymousStateMachine<TopInstance>
        {
            public TopInstanceStateMachine()
            {
                State(() => Running);

                Event(() => Initialized);

                During(Initial,
                    When(Initialized)
                        .Then((instance, data) => instance.Value = data.Value)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }

            public Event<Init> Initialized { get; private set; }
        }


        class BottomInstanceStateMachine :
            AutomatonymousStateMachine<BottomInstance>
        {
            public BottomInstanceStateMachine()
            {
                State(() => Running);

                Event(() => Initialized);

                During(Initial,
                    When(Initialized)
                        .Then((instance, data) => instance.OtherValue = data.Value)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }

            public Event<Init> Initialized { get; private set; }
        }
    }
}