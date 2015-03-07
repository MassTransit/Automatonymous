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
    using Activities;
    using NUnit.Framework;


    [TestFixture]
    public class Having_a_dependency_available
    {
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

            _machine.RaiseEvent(_claim, _machine.Create, data)
                .Wait();
        }


        class ClaimAdjustmentInstance :
            ClaimAdjustment
        {
            public State CurrentState { get; set; }
            public string Value { get; set; }
        }


        class CalculateValueActivity :
            Activity<ClaimAdjustmentInstance, CreateClaim>
        {
            readonly CalculatorService _calculator;

            public CalculateValueActivity(CalculatorService calculator)
            {
                _calculator = calculator;
            }

            public async Task Execute(BehaviorContext<ClaimAdjustmentInstance, CreateClaim> context,
                Behavior<ClaimAdjustmentInstance, CreateClaim> next)
            {
                context.Instance.Value = _calculator.Add(context.Data.X, context.Data.Y);
            }

            public Task Faulted<TException>(BehaviorExceptionContext<ClaimAdjustmentInstance, CreateClaim, TException> context,
                Behavior<ClaimAdjustmentInstance, CreateClaim> next)
                where TException : Exception
            {
                return next.Faulted(context);
            }

            public void Accept(StateMachineVisitor visitor)
            {
                visitor.Visit(this);
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
            AutomatonymousStateMachine<ClaimAdjustmentInstance>
        {
            public InstanceStateMachine()
            {
                InstanceState(x => x.CurrentState);

                During(Initial,
                    When(Create)
                        .Execute(context => new CalculateValueActivity(new LocalCalculator()))
                        .Execute(context => new ActionActivity<ClaimAdjustmentInstance>(x => { }))
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


        [Test]
        public void Should_capture_the_value()
        {
            Assert.AreEqual("79", _claim.Value);
        }
    }
}