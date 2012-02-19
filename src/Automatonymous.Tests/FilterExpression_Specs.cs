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
    public class When_specifying_a_conditional_event_activity
    {
        [Test]
        public void Should_transition_to_the_proper_state()
        {
            var instance = new Instance();
            var machine = new InstanceStateMachine();

            machine.RaiseEvent(instance, machine.Thing, new Data { Condition = true });
            Assert.AreEqual(machine.True, instance.CurrentState);

            // reset
            instance.CurrentState = machine.Initial;

            machine.RaiseEvent(instance, machine.Thing, new Data { Condition = false });
            Assert.AreEqual(machine.False, instance.CurrentState);
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
                State(() => True);
                State(() => False);

                Event(() => Thing);

                During(Initial,
                    When(Thing, msg => msg.Condition)
                        .TransitionTo(True),
                    When(Thing, msg => !msg.Condition)
                        .TransitionTo(False));
            }

            public State True { get; private set; }
            public State False { get; private set; }

            public Event<Data> Thing { get; private set; }
        }

        class Data
        {
            public bool Condition { get; set; }
        }
    }
}