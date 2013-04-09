// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
    using Activities;
    using NUnit.Framework;
    using Taskell;


    [TestFixture]
    public class Having_a_dependency_available
    {
        [Test]
        public void Should_capture_the_value()
        {
            Assert.AreEqual("79", _claim.Value);
        }

        ClaimAdjustmentInstance _claim;
        InstanceStateMachine _machine;

        [TestFixtureSetUp]
        public void Specifying_an_event_activity()
        {
            _claim = new ClaimAdjustmentInstance();
            _machine = new InstanceStateMachine();

            var data = new CreateClaim
                {
                    X = 56,
                    Y = 23,
                };

            _machine.RaiseEvent(_claim, _machine.Create, data);
        }


        class ClaimAdjustmentInstance :
            ClaimAdjustment
        {
            public State CurrentState { get; set; }
            public string Value { get; set; }
        }


        class CalculateValueActivity :
            Activity<ClaimAdjustment, CreateClaim>
        {
            readonly CalculatorService _calculator;

            public CalculateValueActivity(CalculatorService calculator)
            {
                _calculator = calculator;
            }

            public void Accept(StateMachineInspector inspector)
            {
                inspector.Inspect(this, x => { });
            }

            public void Execute(Composer composer, ClaimAdjustment instance, CreateClaim value)
            {
                composer.Execute(() => { instance.Value = _calculator.Add(value.X, value.Y); });
            }
        }


        interface ClaimAdjustment :
            ClaimModel
        {
            State CurrentState { get; set; }
        }


        interface ClaimModel
        {
            string Value { get; set; }
        }


        class CreateClaim
        {
            public int X { get; set; }
            public int Y { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<ClaimAdjustment>
        {
            public InstanceStateMachine()
            {
                InstanceState(x => x.CurrentState);

                State(() => Running);

                Event(() => Create);

                During(Initial,
                    When(Create)
                        .Then(() => new CalculateValueActivity(new LocalCalculator()))
                        .Then(() => new ActionActivity<ClaimAdjustment>(x => { }))
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }

            public Event<CreateClaim> Create { get; private set; }
        }


        interface CalculatorService
        {
            string Add(int x, int y);
        }


        class LocalCalculator :
            CalculatorService
        {
            public string Add(int x, int y)
            {
                return (x + y).ToString();
            }
        }
    }
}