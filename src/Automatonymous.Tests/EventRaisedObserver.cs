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
    using System.Collections.Generic;
    using System.Threading.Tasks;


    class EventRaisedObserver<TInstance> :
        EventObserver<TInstance>
    {
        public EventRaisedObserver()
        {
            Events = new List<EventContext<TInstance>>();
        }

        public IList<EventContext<TInstance>> Events { get; }

        public async Task PreExecute(EventContext<TInstance> context)
        {
        }

        public async Task PreExecute<T>(EventContext<TInstance, T> context)
        {
        }

        public async Task PostExecute(EventContext<TInstance> context)
        {
            Events.Add(context);
        }

        public async Task PostExecute<T>(EventContext<TInstance, T> context)
        {
            Events.Add(context);
        }

        public async Task ExecuteFault(EventContext<TInstance> context, Exception exception)
        {
        }

        public async Task ExecuteFault<T>(EventContext<TInstance, T> context, Exception exception)
        {
        }
    }
}