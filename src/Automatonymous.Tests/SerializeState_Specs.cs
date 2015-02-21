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
    public class Serializing_a_state_instance
    {
        [Test]
        public async void Should_properly_handle_the_state_property()
        {
            var instance = new Instance();
            var machine = new InstanceStateMachine();

            await machine.RaiseEvent(instance, machine.Thing, new Data
            {
                Condition = true
            });
            Assert.AreEqual(machine.True, instance.CurrentState);

            var serializer = new JsonStateSerializer<InstanceStateMachine, Instance>(machine);

            string body = serializer.Serialize(instance);

            Console.WriteLine("Body: {0}", body);
            var reInstance = serializer.Deserialize<Instance>(body);

            Assert.AreEqual(machine.True, reInstance.CurrentState);
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

                State(() => True);
                State(() => False);

                Event(() => Thing);

                During(Initial,
                    When(Thing, context => context.Data.Condition)
                        .TransitionTo(True),
                    When(Thing, context => !context.Data.Condition)
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