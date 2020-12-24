namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;


    /// <summary>
    /// Catches an exception of a specific type and compensates using the behavior
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    public class CatchFaultActivity<TInstance, TException> :
        Activity<TInstance>
        where TInstance : class
        where TException : Exception
    {
        readonly Behavior<TInstance> _behavior;

        public CatchFaultActivity(Behavior<TInstance> behavior)
        {
            _behavior = behavior;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x => _behavior.Accept(visitor));
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("catch");

            scope.Add("exceptionType", TypeCache<TException>.ShortName);

            _behavior.Probe(scope.CreateScope("behavior"));
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance>.Faulted<T>(BehaviorExceptionContext<TInstance, T> context, Behavior<TInstance> next)
        {
            var exceptionContext = context as BehaviorExceptionContext<TInstance, TException>;
            if (exceptionContext != null)
            {
                await _behavior.Faulted(exceptionContext).ConfigureAwait(false);

                // if the compensate returns, we should go forward normally
                await next.Execute(context).ConfigureAwait(false);
            }
            else
            {
                await next.Faulted(context).ConfigureAwait(false);
            }
        }

        async Task Activity<TInstance>.Faulted<TData, T>(BehaviorExceptionContext<TInstance, TData, T> context,
            Behavior<TInstance, TData> next)
        {
            var exceptionContext = context as BehaviorExceptionContext<TInstance, TData, TException>;
            if (exceptionContext != null)
            {
                await _behavior.Faulted(exceptionContext).ConfigureAwait(false);

                // if the compensate returns, we should go forward normally
                await next.Execute(context).ConfigureAwait(false);
            }
            else
            {
                await next.Faulted(context).ConfigureAwait(false);
            }
        }
    }
}
