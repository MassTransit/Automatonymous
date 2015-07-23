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


    public class BehaviorContextProxy<TInstance> :
        BehaviorContext<TInstance>
    {
        readonly BehaviorContext<TInstance> _context;

        public BehaviorContextProxy(BehaviorContext<TInstance> context, Event @event)
        {
            _context = context;
            Event = @event;
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
            return _context.GetProxy(@event);
        }

        public BehaviorContext<TInstance, T> GetProxy<T>(Event<T> @event, T data)
        {
            return _context.GetProxy(@event, data);
        }
    }


    public class BehaviorContextProxy<TInstance, TData> :
        BehaviorContext<TInstance, TData>
    {
        readonly BehaviorContext<TInstance> _context;
        readonly Event<TData> _event;

        public BehaviorContextProxy(BehaviorContext<TInstance> context, Event<TData> @event, TData data)
        {
            _context = context;
            _event = @event;
            Data = data;
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

        public TData Data { get; }
        public CancellationToken CancellationToken => _context.CancellationToken;
        Event EventContext<TInstance>.Event => _event;
        Event<TData> EventContext<TInstance, TData>.Event => _event;
        public TInstance Instance => _context.Instance;

        public BehaviorContext<TInstance> GetProxy(Event @event)
        {
            return _context.GetProxy(@event);
        }

        public BehaviorContext<TInstance, T> GetProxy<T>(Event<T> @event, T data)
        {
            return _context.GetProxy(@event, data);
        }
    }
}