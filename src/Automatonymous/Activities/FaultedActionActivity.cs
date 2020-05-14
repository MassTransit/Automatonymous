namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class FaultedActionActivity<TInstance, TException> :
        Activity<TInstance>
        where TException : Exception
    {
        readonly Action<BehaviorExceptionContext<TInstance, TException>> _action;

        public FaultedActionActivity(Action<BehaviorExceptionContext<TInstance, TException>> action)
        {
            _action = action;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("then-faulted");
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance>.Execute<TData>(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance>.Faulted<T>(BehaviorExceptionContext<TInstance, T> context, Behavior<TInstance> next)
        {
            var exceptionContext = context as BehaviorExceptionContext<TInstance, TException>;
            if (exceptionContext != null)
                _action(exceptionContext);

            return next.Faulted(context);
        }

        Task Activity<TInstance>.Faulted<TData, T>(BehaviorExceptionContext<TInstance, TData, T> context,
            Behavior<TInstance, TData> next)
        {
            var exceptionContext = context as BehaviorExceptionContext<TInstance, TData, TException>;
            if (exceptionContext != null)
                _action(exceptionContext);

            return next.Faulted(context);
        }
    }


    public class FaultedActionActivity<TInstance, TData, TException> :
        Activity<TInstance, TData>
        where TInstance : class
        where TException : Exception
    {
        readonly Action<BehaviorExceptionContext<TInstance, TData, TException>> _action;

        public FaultedActionActivity(Action<BehaviorExceptionContext<TInstance, TData, TException>> action)
        {
            _action = action;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("then-faulted");
        }

        Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance, TData>.Faulted<T>(BehaviorExceptionContext<TInstance, TData, T> context,
            Behavior<TInstance, TData> next)
        {
            var exceptionContext = context as BehaviorExceptionContext<TInstance, TData, TException>;
            if (exceptionContext != null)
                _action(exceptionContext);

            return next.Faulted(context);
        }
    }
}
