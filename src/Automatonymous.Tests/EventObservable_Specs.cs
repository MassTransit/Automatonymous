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
    using System.Collections.Generic;
    using Xunit;


    
    public class When_an_event_is_raised_on_an_instance
    {
        [Fact]
        public void Should_have_raised_the_initialized_event()
        {
            Assert.Equal(_machine.Initialized, _observer.Events[0].Event);
        }

        [Fact]
        public void Should_raise_the_event()
        {
            Assert.Equal(1, _observer.Events.Count);
        }

        Instance _instance;
        InstanceStateMachine _machine;
        ChangeObserver _observer;

        public When_an_event_is_raised_on_an_instance()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
            _observer = new ChangeObserver();

            using (IDisposable subscription = _machine.EventRaised(_machine.Initialized).Subscribe(_observer))
                _machine.RaiseEvent(_instance, x => x.Initialized);
        }


        class ChangeObserver :
            IObserver<EventRaised<Instance>>
        {
            public ChangeObserver()
            {
                Events = new List<EventRaised<Instance>>();
            }

            public IList<EventRaised<Instance>> Events { get; private set; }

            public void OnNext(EventRaised<Instance> value)
            {
                Events.Add(value);
            }

            public void OnError(Exception error)
            {
            }

            public void OnCompleted()
            {
            }
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
}