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
    using System.Collections.Generic;
    using NUnit.Framework;


    [TestFixture]
    public class Observing_state_machine_instance_state_changes
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
                _machine.RaiseEvent(_instance, x => x.Initialized).Wait();
                _machine.RaiseEvent(_instance, x => x.Finish).Wait();
            }
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
                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));

                During(Running,
                    When(Finish)
                        .Finalize());
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }
            public Event Finish { get; private set; }
        }


        [Test]
        public void Should_have_first_moved_to_initial()
        {
            Assert.AreEqual(null, _observer.Events[0].Previous);
            Assert.AreEqual(_machine.Initial, _observer.Events[0].Current);
        }

        [Test]
        public void Should_have_second_switched_to_running()
        {
            Assert.AreEqual(_machine.Initial, _observer.Events[1].Previous);
            Assert.AreEqual(_machine.Running, _observer.Events[1].Current);
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(3, _observer.Events.Count);
        }
    }


    [TestFixture]
    public class Observing_events_with_substates
    {
        Instance _instance;
        InstanceStateMachine _machine;
        StateChangeObserver<Instance> _observer;
        EventRaisedObserver _eventObserver;

        [TestFixtureSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
            _observer = new StateChangeObserver<Instance>();
            _eventObserver = new EventRaisedObserver();

            using (IDisposable subscription = _machine.StateChanged.Subscribe(_observer))
            using (IDisposable beforeEnterSub = _machine.EventRaised(_machine.Running.BeforeEnter).Subscribe(_eventObserver))
            using (IDisposable afterLeaveSub = _machine.EventRaised(_machine.Running.AfterLeave).Subscribe(_eventObserver))
            {
                _machine.RaiseEvent(_instance, x => x.Initialized).Wait();
                _machine.RaiseEvent(_instance, x => x.LegCramped).Wait();
                _machine.RaiseEvent(_instance, x => x.Finish).Wait();
            }
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
                SubState(() => Resting, Running);

                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));

                During(Running,
                    When(LegCramped)
                        .TransitionTo(Resting),
                    When(Finish)
                        .Finalize());

                WhenEnter(Running, x => x.Then(context => { }));
                WhenLeave(Running, x => x.Then(context => { }));
                BeforeEnter(Running, x => x.Then(context => { }));
                AfterLeave(Running, x => x.Then(context => { }));
            }

            public State Running { get; private set; }
            public State Resting { get; private set; }

            public Event Initialized { get; private set; }
            public Event LegCramped { get; private set; }
            public Event Finish { get; private set; }
        }


        class EventRaisedObserver :
            IObserver<EventRaised<Instance>>
        {
            public EventRaisedObserver()
            {
                Events = new List<EventRaised<Instance>>();
            }

            public IList<EventRaised<Instance>> Events { get; private set; }

            public void OnNext(EventRaised<Instance> value)
            {
                Events.Add(value);
            }

            public void OnError(Exception error)
            {
            }

            public void OnCompleted()
            {
            }
        }


        [Test]
        public void Should_have_all_events()
        {
            Assert.AreEqual(2, _eventObserver.Events.Count);
        }

        [Test]
        public void Should_have_first_moved_to_initial()
        {
            Assert.AreEqual(null, _observer.Events[0].Previous);
            Assert.AreEqual(_machine.Initial, _observer.Events[0].Current);
        }

        [Test]
        public void Should_have_fourth_switched_to_finished()
        {
            Assert.AreEqual(_machine.Resting, _observer.Events[3].Previous);
            Assert.AreEqual(_machine.Final, _observer.Events[3].Current);
        }

        [Test]
        public void Should_have_second_switched_to_running()
        {
            Assert.AreEqual(_machine.Initial, _observer.Events[1].Previous);
            Assert.AreEqual(_machine.Running, _observer.Events[1].Current);
        }

        [Test]
        public void Should_have_third_switched_to_resting()
        {
            Assert.AreEqual(_machine.Running, _observer.Events[2].Previous);
            Assert.AreEqual(_machine.Resting, _observer.Events[2].Current);
        }

        [Test]
        public void Should_have_transition_1()
        {
            Assert.AreEqual("Running.BeforeEnter", _eventObserver.Events[0].Event.Name);
        }

        [Test]
        public void Should_have_transition_2()
        {
            Assert.AreEqual("Running.AfterLeave", _eventObserver.Events[1].Event.Name);
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(4, _observer.Events.Count);
        }
    }


    [TestFixture]
    public class Observing_events_with_substates_part_deux
    {
        Instance _instance;
        InstanceStateMachine _machine;
        StateChangeObserver<Instance> _observer;
        EventRaisedObserver _eventObserver;

        [TestFixtureSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
            _observer = new StateChangeObserver<Instance>();
            _eventObserver = new EventRaisedObserver();

            using (IDisposable subscription = _machine.StateChanged.Subscribe(_observer))
            using (IDisposable beforeEnterSub = _machine.EventRaised(_machine.Running.BeforeEnter).Subscribe(_eventObserver))
            using (IDisposable afterLeaveSub = _machine.EventRaised(_machine.Running.AfterLeave).Subscribe(_eventObserver))
            {
                _machine.RaiseEvent(_instance, x => x.Initialized).Wait();
                _machine.RaiseEvent(_instance, x => x.LegCramped).Wait();
                _machine.RaiseEvent(_instance, x => x.Recovered).Wait();
                _machine.RaiseEvent(_instance, x => x.Finish).Wait();
            }
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
                SubState(() => Resting, Running);

                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));

                During(Running,
                    When(LegCramped)
                        .TransitionTo(Resting),
                    When(Finish)
                        .Finalize());

                During(Resting,
                    When(Recovered)
                        .TransitionTo(Running));

                WhenEnter(Running, x => x.Then(context => { }));
                WhenLeave(Running, x => x.Then(context => { }));
                BeforeEnter(Running, x => x.Then(context => { }));
                AfterLeave(Running, x => x.Then(context => { }));
            }

            public State Running { get; private set; }
            public State Resting { get; private set; }

            public Event Initialized { get; private set; }
            public Event LegCramped { get; private set; }
            public Event Recovered { get; private set; }
            public Event Finish { get; private set; }
        }


        class EventRaisedObserver :
            IObserver<EventRaised<Instance>>
        {
            public EventRaisedObserver()
            {
                Events = new List<EventRaised<Instance>>();
            }

            public IList<EventRaised<Instance>> Events { get; private set; }

            public void OnNext(EventRaised<Instance> value)
            {
                Events.Add(value);
            }

            public void OnError(Exception error)
            {
            }

            public void OnCompleted()
            {
            }
        }


        [Test]
        public void Should_have_all_events()
        {
            Assert.AreEqual(2, _eventObserver.Events.Count);
        }

        [Test]
        public void Should_have_fifth_switched_to_finished()
        {
            Assert.AreEqual(_machine.Running, _observer.Events[4].Previous);
            Assert.AreEqual(_machine.Final, _observer.Events[4].Current);
        }

        [Test]
        public void Should_have_first_moved_to_initial()
        {
            Assert.AreEqual(null, _observer.Events[0].Previous);
            Assert.AreEqual(_machine.Initial, _observer.Events[0].Current);
        }

        [Test]
        public void Should_have_fourth_switched_to_finished()
        {
            Assert.AreEqual(_machine.Resting, _observer.Events[3].Previous);
            Assert.AreEqual(_machine.Running, _observer.Events[3].Current);
        }

        [Test]
        public void Should_have_second_switched_to_running()
        {
            Assert.AreEqual(_machine.Initial, _observer.Events[1].Previous);
            Assert.AreEqual(_machine.Running, _observer.Events[1].Current);
        }

        [Test]
        public void Should_have_third_switched_to_resting()
        {
            Assert.AreEqual(_machine.Running, _observer.Events[2].Previous);
            Assert.AreEqual(_machine.Resting, _observer.Events[2].Current);
        }

        [Test]
        public void Should_have_transition_1()
        {
            Assert.AreEqual("Running.BeforeEnter", _eventObserver.Events[0].Event.Name);
        }

        [Test]
        public void Should_have_transition_2()
        {
            Assert.AreEqual("Running.AfterLeave", _eventObserver.Events[1].Event.Name);
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(5, _observer.Events.Count);
        }
    }
}