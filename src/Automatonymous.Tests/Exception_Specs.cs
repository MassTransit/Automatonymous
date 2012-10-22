﻿// Copyright 2011 Chris Patterson, Dru Sellers
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
    using Xunit;


    
    public class When_an_action_throws_an_exception
    {
        [Fact]
        public void Should_capture_the_exception_message()
        {
            Assert.Equal("Boom!", _instance.ExceptionMessage);
        }

        [Fact]
        public void Should_capture_the_exception_type()
        {
            Assert.Equal(typeof(InvalidOperationException), _instance.ExceptionType);
        }

        [Fact]
        public void Should_have_called_the_exception_handler()
        {
            Assert.Equal(_machine.Failed, _instance.CurrentState);
        }

        [Fact]
        public void Should_have_called_the_first_action()
        {
            Assert.True(_instance.Called);
        }

        [Fact]
        public void Should_not_have_called_the_second_action()
        {
            Assert.True(_instance.NotCalled);
        }

        Instance _instance;
        InstanceStateMachine _machine;

        public When_an_action_throws_an_exception()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _machine.RaiseEvent(_instance, _machine.Initialized).Wait();
        }


        class Instance
        {
            public Instance()
            {
                NotCalled = true;
            }

            public bool Called { get; set; }
            public bool NotCalled { get; set; }
            public Type ExceptionType { get; set; }
            public string ExceptionMessage { get; set; }
            public State CurrentState { get; set; }
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
			{
				InstanceState(x => x.CurrentState);

                State(() => Failed);

                Event(() => Initialized);

                During(Initial,
                    When(Initialized)
                        .Try(x => x.Then(instance => instance.Called = true)
                                      .Then(_ => { throw new InvalidOperationException("Boom!"); })
                                      .Then(instance => instance.NotCalled = false),
                            x => x.Handle<Exception>(ex =>
                                {
                                    return ex
                                        .Then((instance, exception) =>
                                            {
                                                instance.ExceptionMessage = exception.Message;
                                                instance.ExceptionType = exception.GetType();
                                            })
                                        .TransitionTo(Failed);
                                })));
            }

            public State Failed { get; private set; }

            public Event Initialized { get; private set; }
        }
    }


    
    public class When_an_action_throws_an_exception_on_data_events
    {
        [Fact]
        public void Should_capture_the_exception_message()
        {
            Assert.Equal("Boom!", _instance.ExceptionMessage);
        }

        [Fact]
        public void Should_capture_the_exception_type()
        {
            Assert.Equal(typeof(InvalidOperationException), _instance.ExceptionType);
        }

        [Fact]
        public void Should_have_called_the_exception_handler()
        {
            Assert.Equal(_machine.Failed, _instance.CurrentState);
        }

        [Fact]
        public void Should_have_called_the_first_action()
        {
            Assert.True(_instance.Called);
        }

        [Fact]
        public void Should_not_have_called_the_second_action()
        {
            Assert.True(_instance.NotCalled);
        }

        Instance _instance;
        InstanceStateMachine _machine;

        public When_an_action_throws_an_exception_on_data_events()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _machine.RaiseEvent(_instance, _machine.Initialized, new Init()).Wait();
        }


        class Instance
        {
            public Instance()
            {
                NotCalled = true;
            }

            public bool Called { get; set; }
            public bool NotCalled { get; set; }
            public Type ExceptionType { get; set; }
            public string ExceptionMessage { get; set; }
            public State CurrentState { get; set; }
        }


        class Init
        {
        }


        class InstanceStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public InstanceStateMachine()
			{
				InstanceState(x => x.CurrentState);

                State(() => Failed);

                Event(() => Initialized);

                During(Initial,
                    When(Initialized)
                        .Try(x => x.Then(instance => instance.Called = true)
                                      .Then(_ => { throw new InvalidOperationException("Boom!"); })
                                      .Then(instance => instance.NotCalled = false),
                            x => x.Handle<Exception>(ex =>
                                {
                                    return ex
                                        .Then((instance, exception) =>
                                            {
                                                instance.ExceptionMessage = exception.Item2.Message;
                                                instance.ExceptionType = exception.Item2.GetType();
                                            })
                                        .TransitionTo(Failed);
                                })));
            }

            public State Failed { get; private set; }

            public Event<Init> Initialized { get; private set; }
        }
    }
}