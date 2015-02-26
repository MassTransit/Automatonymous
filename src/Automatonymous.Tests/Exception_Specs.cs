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
    public class When_an_action_throws_an_exception
    {
        Instance _instance;
        InstanceStateMachine _machine;

        [TestFixtureSetUp]
        public void Specifying_an_event_activity()
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
                        .Try(t => t
                            .Then(context => context.Instance.Called = true)
                            .Then(_ => { throw new ApplicationException("Boom!"); })
                            .Then(context => context.Instance.NotCalled = false),
                            h => h
                                .Handle<Exception>(ex => ex
                                    .Then(context =>
                                    {
                                        context.Instance.ExceptionMessage = context.Data.Message;
                                        context.Instance.ExceptionType = context.Data.GetType();
                                    })
                                    .TransitionTo(Failed))));
            }

            public State Failed { get; private set; }

            public Event Initialized { get; private set; }
        }


        [Test]
        public void Should_capture_the_exception_message()
        {
            Assert.AreEqual("Boom!", _instance.ExceptionMessage);
        }

        [Test]
        public void Should_capture_the_exception_type()
        {
            Assert.AreEqual(typeof(ApplicationException), _instance.ExceptionType);
        }

        [Test]
        public void Should_have_called_the_exception_handler()
        {
            Assert.AreEqual(_machine.Failed, _instance.CurrentState);
        }

        [Test]
        public void Should_have_called_the_first_action()
        {
            Assert.IsTrue(_instance.Called);
        }

        [Test]
        public void Should_not_have_called_the_second_action()
        {
            Assert.IsTrue(_instance.NotCalled);
        }
    }


    [TestFixture]
    public class When_an_action_throws_an_exception_on_data_events
    {
        Instance _instance;
        InstanceStateMachine _machine;

        [TestFixtureSetUp]
        public void Specifying_an_event_activity()
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
                        .Try(x => x.Then(context => context.Instance.Called = true)
                            .Then(_ => { throw new ApplicationException("Boom!"); })
                            .Then(context => context.Instance.NotCalled = false),
                            x => x.Handle<Exception>(ex =>
                            {
                                return ex
                                    .Then(context =>
                                    {
                                        context.Instance.ExceptionMessage = context.Data.Item2.Message;
                                        context.Instance.ExceptionType = context.Data.Item2.GetType();
                                    })
                                    .TransitionTo(Failed);
                            })));
            }

            public State Failed { get; private set; }

            public Event<Init> Initialized { get; private set; }
        }


        [Test]
        public void Should_capture_the_exception_message()
        {
            Assert.AreEqual("Boom!", _instance.ExceptionMessage);
        }

        [Test]
        public void Should_capture_the_exception_type()
        {
            Assert.AreEqual(typeof(ApplicationException), _instance.ExceptionType);
        }

        [Test]
        public void Should_have_called_the_exception_handler()
        {
            Assert.AreEqual(_machine.Failed, _instance.CurrentState);
        }

        [Test]
        public void Should_have_called_the_first_action()
        {
            Assert.IsTrue(_instance.Called);
        }

        [Test]
        public void Should_not_have_called_the_second_action()
        {
            Assert.IsTrue(_instance.NotCalled);
        }
    }
}