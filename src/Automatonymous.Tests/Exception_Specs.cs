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
    using NUnit.Framework;


    [TestFixture]
    public class When_an_action_throws_an_exception
    {
        [Test]
        public void Should_have_called_the_exception_handler()
        {
            Assert.AreEqual(_machine.Failed, _instance.CurrentState);
        }

        [Test]
        public void Should_have_called_the_first_action()
        {
            Assert.IsTrue(_instance.Called);
        }

        [Test]
        public void Should_not_have_called_the_second_action()
        {
            Assert.IsTrue(_instance.NotCalled);
        }

        Instance _instance;
        InstanceStateMachine _machine;

        [TestFixtureSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _machine.RaiseEvent(_instance, _machine.Initialized);
        }


        class Instance :
            StateMachineInstance
        {
            public Instance()
            {
                NotCalled = true;
            }

            public bool Called { get; set; }
            public bool NotCalled { get; set; }
            public State CurrentState { get; set; }
        }


        class InstanceStateMachine :
            StateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                State(() => Failed);

                Event(() => Initialized);

                During(Initial,
                    When(Initialized)
                        .Try(x => x.Then(instance => instance.Called = true)
                                      .Then(_ => { throw new ApplicationException("Boom!"); })
                                      .Then(instance => instance.NotCalled = false),
                            x => x.Handle<ApplicationException>(ex => ex.TransitionTo(Failed))));
            }

            public State Failed { get; private set; }

            public Event Initialized { get; private set; }
        }
    }
}