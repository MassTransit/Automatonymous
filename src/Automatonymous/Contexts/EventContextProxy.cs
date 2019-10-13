// Copyright 2011-2016 Chris Patterson, Dru Sellers
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
    using System.Threading.Tasks;
    using GreenPipes;


    public class EventContextProxy<TInstance> :
        ProxyPipeContext,
        EventContext<TInstance>
    {
        readonly EventContext<TInstance> _context;

        public EventContextProxy(EventContext<TInstance> context, Event @event)
            : base(context)
        {
            _context = context;
            Event = @event;
        }

        public Task Raise(Event @event)
        {
            return _context.Raise(@event);
        }

        public Task Raise<T>(Event<T> @event, T data)
        {
            return _context.Raise(@event, data);
        }

        public Event Event { get; }
        public TInstance Instance => _context.Instance;
    }


    public class EventContextProxy<TInstance, TData> :
        ProxyPipeContext,
        EventContext<TInstance, TData>
    {
        readonly EventContext<TInstance> _context;
        readonly Event<TData> _event;

        public EventContextProxy(EventContext<TInstance> context, Event<TData> @event, TData data)
            : base(context)
        {
            _context = context;
            _event = @event;
            Data = data;
        }

        public Task Raise(Event @event)
        {
            return _context.Raise(@event);
        }

        public Task Raise<T>(Event<T> @event, T data)
        {
            return _context.Raise(@event, data);
        }

        public TData Data { get; }
        Event EventContext<TInstance>.Event => _event;
        Event<TData> EventContext<TInstance, TData>.Event => _event;
        public TInstance Instance => _context.Instance;
    }
}
