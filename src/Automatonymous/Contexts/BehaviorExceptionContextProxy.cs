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
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public class BehaviorExceptionContextProxy<TInstance, TException> :
        BasePipeContext,
        BehaviorExceptionContext<TInstance, TException>
        where TException : Exception
    {
        readonly BehaviorContext<TInstance> _context;
        readonly TException _exception;

        public BehaviorExceptionContextProxy(BehaviorContext<TInstance> context, TException exception)
            : base(context)
        {
            _context = context;
            _exception = exception;
        }

        public TInstance Instance => _context.Instance;

        public Task Raise(Event @event, CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.Raise(@event, cancellationToken);
        }

        public Task Raise<TData>(Event<TData> @event, TData data, CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.Raise(@event, data, cancellationToken);
        }

        public Event Event => _context.Event;

        public BehaviorContext<TInstance> GetProxy(Event @event)
        {
            return _context.GetProxy(@event);
        }

        BehaviorExceptionContext<TInstance, T, TException> BehaviorExceptionContext<TInstance, TException>.GetProxy<T>(Event<T> @event,
            T data)
        {
            BehaviorContext<TInstance, T> contextProxy = _context.GetProxy(@event, data);

            return new BehaviorExceptionContextProxy<TInstance, T, TException>(contextProxy, _exception);
        }

        public TException Exception => _exception;

        public BehaviorContext<TInstance, T> GetProxy<T>(Event<T> @event, T data)
        {
            return _context.GetProxy(@event, data);
        }
    }


    public class BehaviorExceptionContextProxy<TInstance, TData, TException> :
        BasePipeContext,
        BehaviorExceptionContext<TInstance, TData, TException>
        where TException : Exception
    {
        readonly BehaviorContext<TInstance, TData> _context;
        readonly TException _exception;

        public BehaviorExceptionContextProxy(BehaviorContext<TInstance, TData> context, TException exception)
            : base(context)
        {
            _context = context;
            _exception = exception;
        }

        public TInstance Instance => _context.Instance;

        public Task Raise(Event @event, CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.Raise(@event, cancellationToken);
        }

        public Task Raise<TData1>(Event<TData1> @event, TData1 data, CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.Raise(@event, data, cancellationToken);
        }

        public Event Event => _context.Event;
        public TData Data => _context.Data;

        public BehaviorContext<TInstance> GetProxy(Event @event)
        {
            return _context.GetProxy(@event);
        }

        BehaviorExceptionContext<TInstance, T, TException> BehaviorExceptionContext<TInstance, TException>.GetProxy<T>(Event<T> @event,
            T data)
        {
            BehaviorContext<TInstance, T> contextProxy = _context.GetProxy(@event, data);

            return new BehaviorExceptionContextProxy<TInstance, T, TException>(contextProxy, _exception);
        }

        BehaviorExceptionContext<TInstance, T, TException> BehaviorExceptionContext<TInstance, TData, TException>.GetProxy<T>(
            Event<T> @event, T data)
        {
            BehaviorContext<TInstance, T> contextProxy = _context.GetProxy(@event, data);

            return new BehaviorExceptionContextProxy<TInstance, T, TException>(contextProxy, _exception);
        }

        public TException Exception => _exception;

        public BehaviorContext<TInstance, T> GetProxy<T>(Event<T> @event, T data)
        {
            return _context.GetProxy(@event, data);
        }

        Event<TData> EventContext<TInstance, TData>.Event => _context.Event;
    }
}