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
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class Explicitly_transitioning_to_a_state
    {
        Instance _instance;
        InstanceStateMachine _machine;
        StateChangeObserver<Instance> _observer;

        [TestFixtureSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
            _observer = new StateChangeObserver<Instance>();

            using (IDisposable subscription = _machine.StateChanged.Subscribe(_observer))
            {
                _machine.TransitionToState(_instance, _machine.Running)
                    .Wait();
            }
        }


        class Instance
        {
            public State CurrentState { get; set; }
            public bool EnterCalled { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                State(() => Running);

                Event(() => Initialized);
                Event(() => Finish);

                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));

                During(Running,
                    When(Finish)
                        .Finalize());

                WhenEnter(Running, x => x.Then(context => context.Instance.EnterCalled = true));
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }
            public Event Finish { get; private set; }
        }


        [Test]
        public void Should_call_the_enter_event()
        {
            Assert.IsTrue(_instance.EnterCalled);
        }

        [Test]
        public void Should_have_first_moved_to_initial()
        {
            Assert.AreEqual(null, _observer.Events[0].Previous);
            Assert.AreEqual(_machine.Initial, _observer.Events[0].Current);
        }

        [Test]
        public void Should_have_second_moved_to_running()
        {
            Assert.AreEqual(_machine.Initial, _observer.Events[1].Previous);
            Assert.AreEqual(_machine.Running, _observer.Events[1].Current);
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(2, _observer.Events.Count);
        }
    }


    [TestFixture]
    public class Transitioning_to_a_state_from_a_state
    {
        Instance _instance;
        InstanceStateMachine _machine;
        StateChangeObserver<Instance> _observer;

        [TestFixtureSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
            _observer = new StateChangeObserver<Instance>();

            using (IDisposable subscription = _machine.StateChanged.Subscribe(_observer))
            {
                _machine.RaiseEvent(_instance, x => x.Initialized)
                    .Wait();
                _machine.RaiseEvent(_instance, x => x.Finish);
            }
        }


        class Instance
        {
            public State CurrentState { get; set; }
            public bool EnterCalled { get; set; }

            public bool FinalEntered { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                State(() => Running);

                Event(() => Initialized);
                Event(() => Finish);

                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));

                During(Running,
                    When(Finish)
                        .Finalize());

                BeforeEnter(Final, x => x.Then(context => context.Instance.FinalEntered = true));
                WhenEnter(Running, x => x.Then(context => context.Instance.EnterCalled = true));
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }
            public Event Finish { get; private set; }
        }


        [Test]
        public void Should_call_the_enter_event()
        {
            Assert.IsTrue(_instance.EnterCalled);
        }

        [Test]
        public void Should_have_first_moved_to_initial()
        {
            Assert.AreEqual(null, _observer.Events[0].Previous);
            Assert.AreEqual(_machine.Initial, _observer.Events[0].Current);
        }

        [Test]
        public void Should_have_invoked_final_entered()
        {
            Assert.IsTrue(_instance.FinalEntered);
        }

        [Test]
        public void Should_have_second_moved_to_running()
        {
            Assert.AreEqual(_machine.Initial, _observer.Events[1].Previous);
            Assert.AreEqual(_machine.Running, _observer.Events[1].Current);
        }

        [Test]
        public void Should_have_third_moved_to_final()
        {
            Assert.AreEqual(_machine.Running, _observer.Events[2].Previous);
            Assert.AreEqual(_machine.Final, _observer.Events[2].Current);
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(3, _observer.Events.Count);
        }
    }
}