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
    namespace Telephone_Sample
    {
        using System.Diagnostics;
        using System.Threading.Tasks;
        using NUnit.Framework;


        [TestFixture]
        public class A_simple_phone_call
        {
            [Test]
            public async void Should_be_short_and_sweet()
            {
                var phone = new PrincessModelTelephone();
                await _machine.RaiseEvent(phone, _machine.ServiceEstablished, new PhoneServiceEstablished {Digits = "555-1212"});

                await _machine.RaiseEvent(phone, x => x.CallDialed);
                await _machine.RaiseEvent(phone, x => x.CallConnected);

                await Task.Delay(10);

                await _machine.RaiseEvent(phone, x => x.HungUp);

                Assert.AreEqual(_machine.OffHook.Name, phone.CurrentState);
                Assert.GreaterOrEqual(phone.CallTimer.ElapsedMilliseconds, 10);
            }

            PhoneStateMachine _machine;

            [TestFixtureSetUp]
            public void Setup()
            {
                _machine = new PhoneStateMachine();
            }
        }

        [TestFixture]
        public class A_short_time_on_hold
        {
            [Test]
            public async void Should_be_short_and_sweet()
            {
                var phone = new PrincessModelTelephone();
                await _machine.RaiseEvent(phone, _machine.ServiceEstablished, new PhoneServiceEstablished {Digits = "555-1212"});

                await _machine.RaiseEvent(phone, x => x.CallDialed);
                await _machine.RaiseEvent(phone, x => x.CallConnected);

                await Task.Delay(10);

                await _machine.RaiseEvent(phone, x => x.PlacedOnHold);
                await _machine.RaiseEvent(phone, x => x.TakenOffHold);
                await _machine.RaiseEvent(phone, x => x.HungUp);

                Assert.AreEqual(_machine.OffHook.Name, phone.CurrentState);
                Assert.GreaterOrEqual(phone.CallTimer.ElapsedMilliseconds, 10);
            }

            PhoneStateMachine _machine;

            [TestFixtureSetUp]
            public void Setup()
            {
                _machine = new PhoneStateMachine();
            }
        }

        [TestFixture]
        public class An_extended_time_on_hold
        {
            [Test]
            public async void Should_end__badly()
            {
                var phone = new PrincessModelTelephone();
                await _machine.RaiseEvent(phone, _machine.ServiceEstablished, new PhoneServiceEstablished {Digits = "555-1212"});

                await _machine.RaiseEvent(phone, x => x.CallDialed);
                await _machine.RaiseEvent(phone, x => x.CallConnected);
                await _machine.RaiseEvent(phone, x => x.PlacedOnHold);

                await Task.Delay(10);

                await _machine.RaiseEvent(phone, x => x.HungUp);

                Assert.AreEqual(_machine.OffHook.Name, phone.CurrentState);
                Assert.GreaterOrEqual(phone.CallTimer.ElapsedMilliseconds, 10);
            }

            PhoneStateMachine _machine;

            [TestFixtureSetUp]
            public void Setup()
            {
                _machine = new PhoneStateMachine();
            }
        }


        class PrincessModelTelephone
        {
            public PrincessModelTelephone()
            {
                CallTimer = new Stopwatch();
            }

            public string CurrentState { get; set; }

            public Stopwatch CallTimer { get; private set; }

            public string Number { get; set; }
        }


        class PhoneServiceEstablished
        {
            public string Digits { get; set; }
        }


        class PhoneStateMachine :
            AutomatonymousStateMachine<PrincessModelTelephone>
        {
            public PhoneStateMachine()
            {
                InstanceState(x => x.CurrentState);

                State(() => OffHook);
                State(() => Ringing);
                State(() => Connected);
                State(() => OnHold, Connected);
                State(() => PhoneDestroyed);

                Event(() => ServiceEstablished);
                Event(() => CallDialed);
                Event(() => HungUp);
                Event(() => CallConnected);
                Event(() => LeftMessage);
                Event(() => PlacedOnHold);
                Event(() => TakenOffHold);
                Event(() => PhoneHurledAgainstWall);

                Initially(
                    When(ServiceEstablished)
                        .Then(context => context.Instance.Number = context.Data.Digits)
                        .TransitionTo(OffHook));

                During(OffHook,
                    When(CallDialed)
                        .TransitionTo(Ringing));

                During(Ringing,
                    When(HungUp)
                        .TransitionTo(OffHook),
                    When(CallConnected)
                        .TransitionTo(Connected));

                During(Connected,
                    When(LeftMessage).TransitionTo(OffHook),
                    When(HungUp).TransitionTo(OffHook),
                    When(PlacedOnHold).TransitionTo(OnHold));

                During(OnHold,
                    When(TakenOffHold).TransitionTo(Connected),
                    When(PhoneHurledAgainstWall).TransitionTo(PhoneDestroyed));

                DuringAny(
                    When(Connected.Enter)
                        .Then(context => StartCallTimer(context.Instance)),
                    When(Connected.Leave)
                        .Then(context => StopCallTimer(context.Instance)));
            }


            public State OffHook { get; set; }
            public State Ringing { get; set; }
            public State Connected { get; set; }
            public State OnHold { get; set; }
            public State PhoneDestroyed { get; set; }

            public Event<PhoneServiceEstablished> ServiceEstablished { get; set; }
            public Event CallDialed { get; set; }
            public Event HungUp { get; set; }
            public Event CallConnected { get; set; }
            public Event LeftMessage { get; set; }
            public Event PlacedOnHold { get; set; }
            public Event TakenOffHold { get; set; }
            public Event PhoneHurledAgainstWall { get; set; }

            void StopCallTimer(PrincessModelTelephone instance)
            {
                instance.CallTimer.Stop();
            }

            void StartCallTimer(PrincessModelTelephone instance)
            {
                instance.CallTimer.Start();
            }
        }
    }
}