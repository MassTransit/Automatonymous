namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class ExceptionBehavior<TInstance, TException> :
        Behavior<TInstance>
        where TException : Exception
    {
        readonly BehaviorExceptionContext<TInstance, TException> _context;
        readonly Behavior<TInstance> _next;

        public ExceptionBehavior(Behavior<TInstance> next, BehaviorExceptionContext<TInstance, TException> context)
        {
            _next = next;
            _context = context;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            _next.Accept(visitor);
        }

        public void Probe(ProbeContext context)
        {
            _next.Probe(context);
        }

        Task Behavior<TInstance>.Execute(BehaviorContext<TInstance> context)
        {
            return _next.Faulted(_context);
        }

        Task Behavior<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context)
        {
            return _next.Faulted(_context);
        }

        Task Behavior<TInstance>.Faulted<TData, T>(BehaviorExceptionContext<TInstance, TData, T> context)
        {
            throw new AutomatonymousException("This should not ever be called.");
        }

        Task Behavior<TInstance>.Faulted<T>(BehaviorExceptionContext<TInstance, T> context)
        {
            throw new AutomatonymousException("This should not ever be called.");
        }
    }


    public class ExceptionBehavior<TInstance, TData, TException> :
        Behavior<TInstance>
        where TException : Exception
    {
        readonly BehaviorExceptionContext<TInstance, TData, TException> _context;
        readonly Behavior<TInstance, TData> _next;

        public ExceptionBehavior(Behavior<TInstance, TData> next, BehaviorExceptionContext<TInstance, TData, TException> context)
        {
            _next = next;
            _context = context;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            _next.Accept(visitor);
        }

        public void Probe(ProbeContext context)
        {
            _next.Probe(context);
        }

        Task Behavior<TInstance>.Execute(BehaviorContext<TInstance> context)
        {
            return _next.Faulted(_context);
        }

        Task Behavior<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context)
        {
            return _next.Faulted(_context);
        }

        Task Behavior<TInstance>.Faulted<TD, T>(BehaviorExceptionContext<TInstance, TD, T> context)
        {
            throw new AutomatonymousException("This should not ever be called.");
        }

        Task Behavior<TInstance>.Faulted<T>(BehaviorExceptionContext<TInstance, T> context)
        {
            throw new AutomatonymousException("This should not ever be called.");
        }
    }
}
