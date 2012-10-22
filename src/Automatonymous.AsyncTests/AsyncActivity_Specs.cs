// Copyright 2012 Henrik Feldt
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
namespace Automatonymous.Tests
{
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class When_returning_and_awaiting_Task
    {
        [Fact]
        public async Task AssertIsTrue()
        {
            Assert.Equal(0.0, await Do());
        }

        [Fact]
        public async Task smoke_exception()
        {
            try
            {
                await throw_exception();
            }
            catch (InvalidOperationException)
            {
            }
        }

        static Task<double> Do()
        {
            return Task.FromResult(0.0);
        }

        static Task throw_exception()
        {
            throw new InvalidOperationException("oh noes");
        }
    }
    
    public class When_calling_raise_async
    {
        Instance _instance;
        InstanceStateMachine _machine;

        public When_calling_raise_async()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
        }

        [Fact]
        public async Task Should_transition_to_the_proper_state()
        {
            await _machine.RaiseEvent(_instance, _machine.Initialized);
            Assert.Equal(_machine.Running, _instance.CurrentState);
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


    public class When_specifying_an_async_activity
    {
        readonly Instance _instance;
        readonly InstanceStateMachine _machine;
        readonly Task _because;

        public When_specifying_an_async_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
            _because = _machine.RaiseEvent(_instance, _machine.Initialized);
        }

        [Fact]
        public async Task Should_transition_to_the_proper_state()
        {
            await _because;
            Assert.Equal(_machine.Running, _instance.CurrentState);
        }

        [Fact]
        public async Task Should_have_called_service()
        {
            await _because;
            Assert.True(_machine.CalledAsyncServiceMethod);
        }


        class Instance
        {
            public State CurrentState { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            bool _wasCalled;

            public InstanceStateMachine()
            {
                InstanceState(x => x.CurrentState);

                State(() => Running);

                Event(() => Initialized);

                During(Initial,
                    When(Initialized)
                        .ThenAsync(CallMeAsync)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }

            public bool CalledAsyncServiceMethod
            {
                get { return _wasCalled; }
            }

            public async Task CallMeAsync(Instance instance)
            {
                await Task.Delay(100);
                _wasCalled = true;
            }
        }
    }


    public class When_hooking_the_initial_enter_state_event_async
    {
        [Fact]
        public void Should_call_the_activity()
        {
            Assert.Equal(_machine.Running, _instance.CurrentState);
        }

        [Fact]
        public void Should_have_triggered_the_before_enter_event()
        {
            Assert.Equal(_machine.Initial, _instance.EnteredState);
        }

        [Fact]
        public void Should_have_triggered_the_after_leave_event()
        {
            Assert.Equal(_machine.Initializing, _instance.LeftState);
        }

        readonly Instance _instance;
        readonly InstanceStateMachine _machine;
        readonly Task _because;

        public When_hooking_the_initial_enter_state_event_async()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _because = _machine.RaiseEvent(_instance, _machine.Initialized);
        }


        class Instance
        {
            public State CurrentState { get; set; }
            public State EnteredState { get; set; }
            public State LeftState { get; set; }
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
                        .ThenAsync(async (instance, state) =>
                            {
                                instance.LeftState = state;
                                await Task.Delay(10);
                            }),
                    When(Initializing.BeforeEnter)
                        .Then((instance, state) => instance.EnteredState = state));
            }

            public State Running { get; private set; }
            public State Initializing { get; private set; }

            public Event Initialized { get; private set; }
        }
    }

}