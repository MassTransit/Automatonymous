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

    public class When_calling_async_methods_smoke
    {
        [Fact]
        public async Task smoke()
        {
            await do_something();
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

        static Task<int> do_something()
        {
            return Task.FromResult(11);
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
            await _machine.RaiseEventAsync(_instance, _machine.Initialized);
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


    class When_specifying_an_async_activity
    {
        Instance _instance;
        InstanceStateMachine _machine;

        public When_specifying_an_async_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
        }

        [Fact]
        public async Task Should_transition_to_the_proper_state()
        {
            await _machine.RaiseEventAsync(_instance, _machine.Initialized);
            Assert.Equal(_machine.Running, _instance.CurrentState);
        }

        [Fact]
        public async Task Should_have_called_service()
        {
            await _machine.RaiseEventAsync(_instance, _machine.Initialized);

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

            public bool CalledAsyncMethod
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
}