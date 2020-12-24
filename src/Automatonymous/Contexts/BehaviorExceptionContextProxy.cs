namespace Automatonymous.Contexts
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class BehaviorExceptionContextProxy<TInstance, TException> :
        ProxyPipeContext,
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

        public Task Raise(Event @event)
        {
            return _context.Raise(@event);
        }

        public Task Raise<TData>(Event<TData> @event, TData data)
        {
            return _context.Raise(@event, data);
        }

        public Event Event => _context.Event;

        public BehaviorContext<TInstance> GetProxy(Event @event)
        {
            return _context.GetProxy(@event);
        }

        BehaviorExceptionContext<TInstance, T, TException> BehaviorExceptionContext<TInstance, TException>.GetProxy<T>(Event<T> @event,
            T data)
        {
            var contextProxy = _context.GetProxy(@event, data);

            return new BehaviorExceptionContextProxy<TInstance, T, TException>(contextProxy, _exception);
        }

        public TException Exception => _exception;

        public BehaviorContext<TInstance, T> GetProxy<T>(Event<T> @event, T data)
        {
            return _context.GetProxy(@event, data);
        }
    }


    public class BehaviorExceptionContextProxy<TInstance, TData, TException> :
        ProxyPipeContext,
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

        public Task Raise(Event @event)
        {
            return _context.Raise(@event);
        }

        public Task Raise<TData1>(Event<TData1> @event, TData1 data)
        {
            return _context.Raise(@event, data);
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
            var contextProxy = _context.GetProxy(@event, data);

            return new BehaviorExceptionContextProxy<TInstance, T, TException>(contextProxy, _exception);
        }

        BehaviorExceptionContext<TInstance, T, TException> BehaviorExceptionContext<TInstance, TData, TException>.GetProxy<T>(
            Event<T> @event, T data)
        {
            var contextProxy = _context.GetProxy(@event, data);

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
