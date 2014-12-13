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
namespace Automatonymous
{
    using System.Threading;


    public class BehaviorContextImpl<TInstance> :
        BehaviorContext<TInstance>
    {
        readonly InstanceContext<TInstance> _context;
        readonly Event _event;

        public BehaviorContextImpl(EventContext<TInstance> context)
        {
            _context = context;
            _event = context.Event;
        }

        public BehaviorContextImpl(InstanceContext<TInstance> context)
        {
            _context = context;
        }

        public CancellationToken CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        public Event Event
        {
            get { return _event; }
        }

        public TInstance Instance
        {
            get { return _context.Instance; }
        }

        public BehaviorContext<TInstance> GetProxy(Event @event)
        {
            return new BehaviorContextProxy<TInstance>(this, @event);
        }

        public BehaviorContext<TInstance, T> GetProxy<T>(Event<T> @event, T data)
        {
            return new BehaviorContextProxy<TInstance, T>(this, @event, data);
        }
    }


    public class BehaviorContextImpl<TInstance, TData> :
        BehaviorContextImpl<TInstance>,
        BehaviorContext<TInstance, TData>
    {
        readonly EventContext<TInstance, TData> _context;

        public BehaviorContextImpl(EventContext<TInstance, TData> context)
            : base(context)
        {
            _context = context;
        }

        Event<TData> EventContext<TInstance, TData>.Event
        {
            get { return _context.Event; }
        }

        TData EventContext<TInstance, TData>.Data
        {
            get { return _context.Data; }
        }
    }
}