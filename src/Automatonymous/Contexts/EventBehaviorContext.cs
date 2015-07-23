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
namespace Automatonymous.Contexts
{
    using System;
    using System.Threading;


    public class EventBehaviorContext<TInstance> :
        BehaviorContext<TInstance>
    {
        readonly InstanceContext<TInstance> _context;

        public EventBehaviorContext(EventContext<TInstance> context)
        {
            _context = context;
            Event = context.Event;
        }

        public EventBehaviorContext(InstanceContext<TInstance> context)
        {
            _context = context;
        }

        public bool HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload payload) where TPayload : class
        {
            return _context.TryGetPayload(out payload);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory) where TPayload : class
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        public CancellationToken CancellationToken => _context.CancellationToken;
        public Event Event { get; }
        public TInstance Instance => _context.Instance;

        public BehaviorContext<TInstance> GetProxy(Event @event)
        {
            return new BehaviorContextProxy<TInstance>(this, @event);
        }

        public BehaviorContext<TInstance, T> GetProxy<T>(Event<T> @event, T data)
        {
            return new BehaviorContextProxy<TInstance, T>(this, @event, data);
        }
    }


    public class EventBehaviorContext<TInstance, TData> :
        EventBehaviorContext<TInstance>,
        BehaviorContext<TInstance, TData>
    {
        readonly EventContext<TInstance, TData> _context;

        public EventBehaviorContext(EventContext<TInstance, TData> context)
            : base(context)
        {
            _context = context;
        }

        Event<TData> EventContext<TInstance, TData>.Event => _context.Event;
        TData EventContext<TInstance, TData>.Data => _context.Data;
    }
}