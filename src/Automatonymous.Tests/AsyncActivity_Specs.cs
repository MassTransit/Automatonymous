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
    using System.Diagnostics;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class Using_an_asynchronous_activity
    {
        [Test]
        public void Should_capture_the_value()
        {
            var claim = new TestInstance();
            var machine = new TestStateMachine();

            Task<TestInstance> task = machine.RaiseEventAsync(claim, machine.Create, new CreateInstance());

            bool wait = task.Wait(Debugger.IsAttached ? TimeSpan.FromMinutes(30) : TimeSpan.FromSeconds(8));

            Assert.IsTrue(wait, "Task did not complete");

            Assert.AreEqual("ExecuteAsync", claim.Value);
        }


        class TestInstance
        {
            public State CurrentState { get; set; }
            public string Value { get; set; }
        }


        class SetValueAsyncActivity :
            AsyncActivity<TestInstance, CreateInstance>
        {
            public void Execute(TestInstance instance, CreateInstance data)
            {
                instance.Value = "Execute";
            }

            public Task<TestInstance> ExecuteAsync(TestInstance instance, CreateInstance data)
            {
                return Task<TestInstance>.Factory.StartNew(() =>
                    {
                        instance.Value = "ExecuteAsync";
                        return instance;
                    });
            }

            public void Accept(StateMachineInspector inspector)
            {
                inspector.Inspect(this, x => { });
            }
        }


        class CreateInstance
        {
            public int X { get; set; }
            public int Y { get; set; }
        }


        class TestStateMachine :
            AutomatonymousStateMachine<TestInstance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                State(() => Running);

                Event(() => Create);

                During(Initial,
                    When(Create)
                        .Then(() => new SetValueAsyncActivity())
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }

            public Event<CreateInstance> Create { get; private set; }
        }
    }
}