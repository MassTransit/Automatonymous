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
namespace Automatonymous.Observers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Events;
    using Internals;


    class NonTransitionEventObserver<TInstance> :
        EventObserver<TInstance>
    {
        readonly IReadOnlyDictionary<string, StateMachineEvent<TInstance>> _eventCache;
        readonly EventObserver<TInstance> _observer;

        public NonTransitionEventObserver(IReadOnlyDictionary<string, StateMachineEvent<TInstance>> eventCache,
            EventObserver<TInstance> observer)
        {
            _eventCache = eventCache;
            _observer = observer;
        }

        public Task PreExecute(EventContext<TInstance> context)
        {
            StateMachineEvent<TInstance> stateMachineEvent;
            if (_eventCache.TryGetValue(context.Event.Name, out stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.PreExecute(context);

            return TaskUtil.Completed;
        }

        public Task PreExecute<T>(EventContext<TInstance, T> context)
        {
            StateMachineEvent<TInstance> stateMachineEvent;
            if (_eventCache.TryGetValue(context.Event.Name, out stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.PreExecute(context);

            return TaskUtil.Completed;
        }

        public Task PostExecute(EventContext<TInstance> context)
        {
            StateMachineEvent<TInstance> stateMachineEvent;
            if (_eventCache.TryGetValue(context.Event.Name, out stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.PostExecute(context);

            return TaskUtil.Completed;
        }

        public Task PostExecute<T>(EventContext<TInstance, T> context)
        {
            StateMachineEvent<TInstance> stateMachineEvent;
            if (_eventCache.TryGetValue(context.Event.Name, out stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.PostExecute(context);

            return TaskUtil.Completed;
        }

        public Task ExecuteFault(EventContext<TInstance> context, Exception exception)
        {
            StateMachineEvent<TInstance> stateMachineEvent;
            if (_eventCache.TryGetValue(context.Event.Name, out stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.ExecuteFault(context, exception);

            return TaskUtil.Completed;
        }

        public Task ExecuteFault<T>(EventContext<TInstance, T> context, Exception exception)
        {
            StateMachineEvent<TInstance> stateMachineEvent;
            if (_eventCache.TryGetValue(context.Event.Name, out stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.ExecuteFault(context, exception);

            return TaskUtil.Completed;
        }
    }
}