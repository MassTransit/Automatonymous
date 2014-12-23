// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Automatonymous.Observers
{
    using System;
    using System.Collections.Generic;
    using Events;


    class EventRaisedObserver<TInstance> :
        IObserver<EventRaised<TInstance>>
        where TInstance : class
    {
        readonly IDictionary<string, StateMachineEvent<TInstance>> _eventCache;

        public EventRaisedObserver(IDictionary<string, StateMachineEvent<TInstance>> eventCache)
        {
            _eventCache = eventCache;
        }

        public void OnNext(EventRaised<TInstance> value)
        {
            StateMachineEvent<TInstance> stateMachineEvent;
            if (_eventCache.TryGetValue(value.Event.Name, out stateMachineEvent))
                stateMachineEvent.EventRaised.OnNext(value);
        }

        public void OnError(Exception error)
        {
            foreach (var stateMachineEvent in _eventCache.Values)
                stateMachineEvent.EventRaised.OnError(error);
        }

        public void OnCompleted()
        {
            foreach (var stateMachineEvent in _eventCache.Values)
                stateMachineEvent.EventRaised.OnCompleted();
        }
    }
}