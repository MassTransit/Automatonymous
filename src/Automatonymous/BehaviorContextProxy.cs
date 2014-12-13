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


    public class BehaviorContextProxy<TInstance> :
        BehaviorContext<TInstance>
    {
        readonly BehaviorContext<TInstance> _context;
        readonly Event _event;

        public BehaviorContextProxy(BehaviorContext<TInstance> context, Event @event)
        {
            _context = context;
            _event = @event;
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
        readonly TData _data;
        readonly Event<TData> _event;

        public BehaviorContextProxy(BehaviorContext<TInstance> context, Event<TData> @event, TData data)
        {
            _context = context;
            _event = @event;
            _data = data;
        }

        public TData Data
        {
            get { return _data; }
        }

        public CancellationToken CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        Event EventContext<TInstance>.Event
        {
            get { return _event; }
        }

        Event<TData> EventContext<TInstance, TData>.Event
        {
            get { return _event; }
        }

        public TInstance Instance
        {
            get { return _context.Instance; }
        }

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