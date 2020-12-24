namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class ConditionExceptionActivity<TInstance, TConditionException> :
        Activity<TInstance>
        where TInstance : class
        where TConditionException : Exception
    {
        readonly StateMachineAsyncExceptionCondition<TInstance, TConditionException> _condition;
        readonly Behavior<TInstance> _elseBehavior;
        readonly Behavior<TInstance> _thenBehavior;

        public ConditionExceptionActivity(StateMachineAsyncExceptionCondition<TInstance, TConditionException> condition,
            Behavior<TInstance> thenBehavior, Behavior<TInstance> elseBehavior)
        {
            _condition = condition;
            _thenBehavior = thenBehavior;
            _elseBehavior = elseBehavior;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("condition");

            _thenBehavior.Probe(scope);
            _elseBehavior.Probe(scope);
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x => _thenBehavior.Accept(visitor));
            visitor.Visit(this, x => _elseBehavior.Accept(visitor));
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            var behaviorContext = context as BehaviorExceptionContext<TInstance, TConditionException>;
            if (behaviorContext != null)
            {
                if (await _condition(behaviorContext).ConfigureAwait(false))
                    await _thenBehavior.Faulted(context).ConfigureAwait(false);
                else
                    await _elseBehavior.Faulted(context).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context,
            Behavior<TInstance, T> next)
        {
            var behaviorContext = context as BehaviorExceptionContext<TInstance, T, TConditionException>;
            if (behaviorContext != null)
            {
                if (await _condition(behaviorContext).ConfigureAwait(false))
                    await _thenBehavior.Faulted(context).ConfigureAwait(false);
                else
                    await _elseBehavior.Faulted(context).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }


    public class ConditionExceptionActivity<TInstance, TData, TConditionException> :
        Activity<TInstance>
        where TInstance : class
        where TConditionException : Exception
    {
        readonly StateMachineAsyncExceptionCondition<TInstance, TData, TConditionException> _condition;
        readonly Behavior<TInstance> _elseBehavior;
        readonly Behavior<TInstance> _thenBehavior;

        public ConditionExceptionActivity(StateMachineAsyncExceptionCondition<TInstance, TData, TConditionException> condition,
            Behavior<TInstance> thenBehavior, Behavior<TInstance> elseBehavior)
        {
            _condition = condition;
            _thenBehavior = thenBehavior;
            _elseBehavior = elseBehavior;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("condition");

            _thenBehavior.Probe(scope);
            _elseBehavior.Probe(scope);
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x => _thenBehavior.Accept(visitor));
            visitor.Visit(this, x => _elseBehavior.Accept(visitor));
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            throw new AutomatonymousException("This activity requires a body with the event, but no body was specified.");
        }

        Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            throw new AutomatonymousException("This activity requires a body with the event, but no body was specified.");
        }

        async Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context,
            Behavior<TInstance, T> next)
        {
            var behaviorContext = context as BehaviorExceptionContext<TInstance, TData, TConditionException>;
            if (behaviorContext != null)
            {
                if (await _condition(behaviorContext).ConfigureAwait(false))
                    await _thenBehavior.Faulted(context).ConfigureAwait(false);
                else
                    await _elseBehavior.Faulted(context).ConfigureAwait(false);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }
}
