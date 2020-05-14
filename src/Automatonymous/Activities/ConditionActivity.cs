namespace Automatonymous.Activities
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class ConditionActivity<TInstance> :
        Activity<TInstance>
        where TInstance : class
    {
        readonly StateMachineAsyncCondition<TInstance> _condition;
        readonly Behavior<TInstance> _elseBehavior;
        readonly Behavior<TInstance> _thenBehavior;

        public ConditionActivity(StateMachineAsyncCondition<TInstance> condition, Behavior<TInstance> thenBehavior,
            Behavior<TInstance> elseBehavior)
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

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            if (await _condition(context).ConfigureAwait(false))
                await _thenBehavior.Execute(context).ConfigureAwait(false);
            else
                await _elseBehavior.Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            if (await _condition(context).ConfigureAwait(false))
                await _thenBehavior.Execute(context).ConfigureAwait(false);
            else
                await _elseBehavior.Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            return next.Faulted(context);
        }

        Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context,
            Behavior<TInstance, T> next)
        {
            return next.Faulted(context);
        }
    }


    public class ConditionActivity<TInstance, TData> :
        Activity<TInstance>
        where TInstance : class
    {
        readonly StateMachineAsyncCondition<TInstance, TData> _condition;
        readonly Behavior<TInstance> _elseBehavior;
        readonly Behavior<TInstance> _thenBehavior;

        public ConditionActivity(StateMachineAsyncCondition<TInstance, TData> condition, Behavior<TInstance> thenBehavior,
            Behavior<TInstance> elseBehavior)
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

        async Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            var behaviorContext = context as BehaviorContext<TInstance, TData>;
            if (behaviorContext != null)
            {
                if (await _condition(behaviorContext).ConfigureAwait(false))
                    await _thenBehavior.Execute(behaviorContext).ConfigureAwait(false);
                else
                    await _elseBehavior.Execute(behaviorContext).ConfigureAwait(false);
            }

            await next.Execute(context).ConfigureAwait(false);
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            return next.Faulted(context);
        }

        Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context,
            Behavior<TInstance, T> next)
        {
            return next.Faulted(context);
        }
    }
}
