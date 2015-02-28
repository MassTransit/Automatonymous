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


    public class ExceptionBehaviorContextProxy<TInstance, TException> :
        BehaviorExceptionContext<TInstance, TException>
        where TException : Exception
    {
        readonly BehaviorContext<TInstance> _context;
        readonly TException _exception;

        public ExceptionBehaviorContextProxy(BehaviorContext<TInstance> context, TException exception)
        {
            _context = context;
            _exception = exception;
        }

        public CancellationToken CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        public TInstance Instance
        {
            get { return _context.Instance; }
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

        public Event Event
        {
            get { return _context.Event; }
        }

        public BehaviorContext<TInstance> GetProxy(Event @event)
        {
            return _context.GetProxy(@event);
        }

        BehaviorExceptionContext<TInstance, T, TException> BehaviorExceptionContext<TInstance, TException>.GetProxy<T>(Event<T> @event,
            T data)
        {
            BehaviorContext<TInstance, T> contextProxy = _context.GetProxy(@event, data);

            return new ExceptionBehaviorContextProxy<TInstance, T, TException>(contextProxy, _exception);
        }

        public TException Exception
        {
            get { return _exception; }
        }

        public BehaviorContext<TInstance, T> GetProxy<T>(Event<T> @event, T data)
        {
            return _context.GetProxy(@event, data);
        }
    }


    public class ExceptionBehaviorContextProxy<TInstance, TData, TException> :
        BehaviorExceptionContext<TInstance, TData, TException>
        where TException : Exception
    {
        readonly BehaviorContext<TInstance, TData> _context;
        readonly TException _exception;

        public ExceptionBehaviorContextProxy(BehaviorContext<TInstance, TData> context, TException exception)
        {
            _context = context;
            _exception = exception;
        }

        public CancellationToken CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        public TInstance Instance
        {
            get { return _context.Instance; }
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

        public Event Event
        {
            get { return _context.Event; }
        }

        public TData Data
        {
            get { return _context.Data; }
        }

        public BehaviorContext<TInstance> GetProxy(Event @event)
        {
            return _context.GetProxy(@event);
        }

        BehaviorExceptionContext<TInstance, T, TException> BehaviorExceptionContext<TInstance, TData, TException>.GetProxy<T>(
            Event<T> @event, T data)
        {
            BehaviorContext<TInstance, T> contextProxy = _context.GetProxy(@event, data);

            return new ExceptionBehaviorContextProxy<TInstance, T, TException>(contextProxy, _exception);
        }

        public TException Exception
        {
            get { return _exception; }
        }

        public BehaviorContext<TInstance, T> GetProxy<T>(Event<T> @event, T data)
        {
            return _context.GetProxy(@event, data);
        }

        Event<TData> EventContext<TInstance, TData>.Event
        {
            get { return _context.Event; }
        }
    }
}