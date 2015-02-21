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
    public class When_specifying_an_event_activity
    {
        [Test]
        public void Should_transition_to_the_proper_state()
        {
            Assert.AreEqual(_machine.Running, _instance.CurrentState);
        }

        Instance _instance;
        InstanceStateMachine _machine;

        [TestFixtureSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _machine.RaiseEvent(_instance, _machine.Initialized)
                .Wait();
        }


        class Instance
        {
            public State CurrentState { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                InstanceState(x => x.CurrentState);

                State(() => Running);

                Event(() => Initialized);

                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }
        }
    }


    [TestFixture]
    public class When_specifying_an_event_activity_using_initially
    {
        [Test]
        public void Should_transition_to_the_proper_state()
        {
            Assert.AreEqual(_machine.Running, _instance.CurrentState);
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


        class Instance
        {
            public State CurrentState { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                InstanceState(x => x.CurrentState);

                State(() => Running);

                Event(() => Initialized);

                Initially(
                    When(Initialized)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }
        }
    }


    [TestFixture]
    public class When_specifying_an_event_activity_using_finally
    {
        [Test]
        public void Should_have_called_the_finally_activity()
        {
            Assert.AreEqual(InstanceStateMachine.Finalized, _instance.Value);
        }

        [Test]
        public void Should_transition_to_the_proper_state()
        {
            Assert.AreEqual(_machine.Final, _instance.CurrentState);
        }

        Instance _instance;
        InstanceStateMachine _machine;

        [TestFixtureSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _machine.RaiseEvent(_instance, _machine.Initialized)
                .Wait();
        }


        class Instance
        {
            public string Value { get; set; }
            public State CurrentState { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public const string Finalized = "Finalized";

            public InstanceStateMachine()
            {
                State(() => Running);

                Event(() => Initialized);

                Initially(
                    When(Initialized)
                        .Finalize());

                Finally(x => x.Then(context => context.Instance.Value = Finalized));
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }
        }
    }


    [TestFixture]
    public class When_hooking_the_initial_enter_state_event
    {
        [Test]
        public void Should_call_the_activity()
        {
            Assert.AreEqual(_machine.Final, _instance.CurrentState);
        }

        [Test]
        public void Should_have_trigger_the_final_before_enter_event()
        {
            Assert.AreEqual(_machine.Running, _instance.FinalState);
        }

        [Test]
        public void Should_have_triggered_the_after_leave_event()
        {
            Assert.AreEqual(_machine.Initializing, _instance.LeftState);
        }

        [Test]
        public void Should_have_triggered_the_before_enter_event()
        {
            Assert.AreEqual(_machine.Initial, _instance.EnteredState);
        }

        Instance _instance;
        InstanceStateMachine _machine;

        [TestFixtureSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _machine.RaiseEvent(_instance, _machine.Initialized)
                .Wait();
        }


        class Instance
        {
            public State CurrentState { get; set; }
            public State EnteredState { get; set; }
            public State LeftState { get; set; }
            public State FinalState { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                InstanceState(x => x.CurrentState);

                State(() => Initializing);
                State(() => Running);

                Event(() => Initialized);

                During(Initializing,
                    When(Initialized)
                        .TransitionTo(Running));

                DuringAny(
                    When(Initial.Enter)
                        .TransitionTo(Initializing),
                    When(Initial.AfterLeave)
                        .Then(context => context.Instance.LeftState = context.Data),
                    When(Initializing.BeforeEnter)
                        .Then(context => context.Instance.EnteredState = context.Data),
                    When(Running.Enter)
                        .Finalize(),
                    When(Final.BeforeEnter)
                        .Then(context => context.Instance.FinalState = context.Data));
            }

            public State Running { get; private set; }
            public State Initializing { get; private set; }

            public Event Initialized { get; private set; }
        }
    }
}