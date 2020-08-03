namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class AsyncFaultedActionActivity<TInstance, TException> :
        Activity<TInstance>
        where TException : Exception
    {
        readonly Func<BehaviorExceptionContext<TInstance, TException>, Task> _asyncAction;

        public AsyncFaultedActionActivity(Func<BehaviorExceptionContext<TInstance, TException>, Task> asyncAction)
        {
            _asyncAction = asyncAction;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("then-async-faulted");
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance>.Execute<TData>(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance>.Faulted<T>(BehaviorExceptionContext<TInstance, T> context, Behavior<TInstance> next)
        {
            var exceptionContext = context as BehaviorExceptionContext<TInstance, TException>;
            if (exceptionContext != null)
                await _asyncAction(exceptionContext);

            await next.Faulted(context);
        }

        async Task Activity<TInstance>.Faulted<TData, T>(BehaviorExceptionContext<TInstance, TData, T> context,
            Behavior<TInstance, TData> next)
        {
            var exceptionContext = context as BehaviorExceptionContext<TInstance, TData, TException>;
            if (exceptionContext != null)
                await _asyncAction(exceptionContext);

            await next.Faulted(context);
        }
    }


    public class AsyncFaultedActionActivity<TInstance, TData, TException> :
        Activity<TInstance, TData>
        where TInstance : class
        where TException : Exception
    {
        readonly Func<BehaviorExceptionContext<TInstance, TData, TException>, Task> _asyncAction;

        public AsyncFaultedActionActivity(Func<BehaviorExceptionContext<TInstance, TData, TException>, Task> asyncAction)
        {
            _asyncAction = asyncAction;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("then-async-faulted");
        }

        Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance, TData>.Faulted<T>(BehaviorExceptionContext<TInstance, TData, T> context,
            Behavior<TInstance, TData> next)
        {
            var exceptionContext = context as BehaviorExceptionContext<TInstance, TData, TException>;
            if (exceptionContext != null)
                await _asyncAction(exceptionContext);

            await next.Faulted(context);
        }
    }
}
