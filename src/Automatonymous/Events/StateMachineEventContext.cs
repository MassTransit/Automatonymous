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
namespace Automatonymous.Events
{
    using System;
    using System.Threading;


    public class StateMachineEventContext<TInstance> :
        EventContext<TInstance>
        where TInstance : class
    {
        readonly CancellationToken _cancellationToken;
        readonly Event _event;
        readonly TInstance _instance;
        readonly StateMachine<TInstance> _machine;
        readonly PayloadCache _payloadCache;

        public StateMachineEventContext(StateMachine<TInstance> machine, TInstance instance, Event @event,
            CancellationToken cancellationToken)
        {
            _machine = machine;
            _instance = instance;
            _event = @event;
            _cancellationToken = cancellationToken;

            _payloadCache = new PayloadCache();
        }

        public bool HasPayloadType(Type contextType)
        {
            return _payloadCache.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload context) where TPayload : class
        {
            return _payloadCache.TryGetPayload(out context);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory) where TPayload : class
        {
            return _payloadCache.GetOrAddPayload(payloadFactory);
        }

        CancellationToken InstanceContext<TInstance>.CancellationToken
        {
            get { return _cancellationToken; }
        }

        Event EventContext<TInstance>.Event
        {
            get { return _event; }
        }

        TInstance InstanceContext<TInstance>.Instance
        {
            get { return _instance; }
        }
    }


    public class StateMachineEventContext<TInstance, TData> :
        StateMachineEventContext<TInstance>,
        EventContext<TInstance, TData>
        where TInstance : class
    {
        readonly TData _data;
        readonly Event<TData> _event;

        public StateMachineEventContext(StateMachine<TInstance> machine, TInstance instance, Event<TData> @event, TData data,
            CancellationToken cancellationToken)
            : base(machine, instance, @event, cancellationToken)
        {
            _data = data;
            _event = @event;
        }

        TData EventContext<TInstance, TData>.Data
        {
            get { return _data; }
        }

        Event<TData> EventContext<TInstance, TData>.Event
        {
            get { return _event; }
        }
    }
}