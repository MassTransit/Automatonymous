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
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class Raising_an_event_within_an_event
    {
        [Test]
        public async Task Should_include_payload()
        {
            var instance = new Instance();
            var machine = new InstanceStateMachine();

            await machine.RaiseEvent(instance, machine.Thing, new Data
            {
                Condition = true
            });
            Assert.AreEqual(machine.True, instance.CurrentState);
            Assert.IsTrue(instance.Initialized.HasValue);
        }


        class Instance
        {
            public State CurrentState { get; set; }
            public DateTime? Initialized { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                During(Initial,
                    When(Thing, context => context.Data.Condition)
                        .TransitionTo(True)
                        .Then(context => context.Raise(Initialize)),
                    When(Thing, context => !context.Data.Condition)
                        .TransitionTo(False));

                DuringAny(
                    When(Initialize)
                        .Then(context => context.Instance.Initialized = DateTime.Now));
            }

            public State True { get; private set; }
            public State False { get; private set; }

            public Event<Data> Thing { get; private set; }
            public Event Initialize { get; private set; }
        }


        class Data
        {
            public bool Condition { get; set; }
        }
    }
}