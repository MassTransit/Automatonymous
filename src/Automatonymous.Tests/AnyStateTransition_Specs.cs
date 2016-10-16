// Copyright 2011-2016 Chris Patterson, Dru Sellers
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
    public class When_any_state_transition_occurs
    {
        [Test]
        public void Should_be_running()
        {
            Assert.AreEqual(_machine.Running, _instance.CurrentState);
        }

        [Test]
        public void Should_have_entered_running()
        {
            Assert.AreEqual(_machine.Running, _instance.LastEntered);
        }

        [Test]
        public void Should_have_left_initial()
        {
            Assert.AreEqual(_machine.Initial, _instance.LastLeft);
        }

        Instance _instance;
        InstanceStateMachine _machine;

        [OneTimeSetUp]
        public void Setup()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _machine.RaiseEvent(_instance, x => x.Initialized)
                .Wait();
        }


        class Instance
        {
            public State CurrentState { get; set; }

            public State LastEntered { get; set; }
            public State LastLeft { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                InstanceState(instance => instance.CurrentState);

                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));

                During(Running,
                    When(Finish)
                        .Finalize());

                BeforeEnterAny(x => x.Then(context => context.Instance.LastEntered = context.Data));
                AfterLeaveAny(x => x.Then(context => context.Instance.LastLeft = context.Data));
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }
            public Event Finish { get; private set; }
        }
    }
}