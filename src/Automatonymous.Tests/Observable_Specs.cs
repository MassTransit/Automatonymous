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
    using System.Collections.Generic;
    using NUnit.Framework;


    [TestFixture]
    public class Observing_state_machine_instance_state_changes
    {
        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(2, _observer.Events.Count);
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

        Instance _instance;
        InstanceStateMachine _machine;
        ChangeObserver _observer;

        [TestFixtureSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
            _observer = new ChangeObserver();

            using (var subscription = _machine.StateChanges.Subscribe(_observer))
            {
                _machine.RaiseEvent(_instance, x => x.Initialized);
            }
        }


        class ChangeObserver :
            IObserver<StateChange<Instance>>
        {
            public ChangeObserver()
            {
                Events = new List<StateChange<Instance>>();
            }

            public void OnNext(StateChange<Instance> value)
            {
                Events.Add(value);
            }

            public void OnError(Exception error)
            {
            }

            public void OnCompleted()
            {
            }

            public IList<StateChange<Instance>> Events { get; private set; }
        }


        class Instance :
            StateMachineInstance
        {
            public State CurrentState { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
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
}