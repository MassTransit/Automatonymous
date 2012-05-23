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
namespace Automatonymous.Impl
{
    using System;
    using Internals.Caching;


    class EventRaisingObserver<TInstance> :
        IObserver<EventRaising<TInstance>>
        where TInstance : class
    {
        readonly Cache<string, StateMachineEvent<TInstance>> _eventCache;

        public EventRaisingObserver(Cache<string, StateMachineEvent<TInstance>> eventCache)
        {
            _eventCache = eventCache;
        }

        public void OnNext(EventRaising<TInstance> value)
        {
            _eventCache.WithValue(value.Event.Name, x => x.EventRaising.OnNext(value));
        }

        public void OnError(Exception error)
        {
            _eventCache.Each(x => x.EventRaising.OnError(error));
        }

        public void OnCompleted()
        {
            _eventCache.Each(x => x.EventRaising.OnCompleted());
        }
    }
}