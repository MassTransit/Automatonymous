namespace Automatonymous.Behaviors
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class WidenBehavior<TInstance, TData> :
        Behavior<TInstance>
    {
        readonly TData _data;
        readonly Event<TData> _event;
        readonly Behavior<TInstance, TData> _next;

        public WidenBehavior(Behavior<TInstance, TData> next, EventContext<TInstance, TData> context)
        {
            _next = next;
            _data = context.Data;
            _event = context.Event;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            _next.Probe(context);
        }

        Task Behavior<TInstance>.Execute(BehaviorContext<TInstance> context)
        {
            var nextContext = context.GetProxy(_event, _data);

            return _next.Execute(nextContext);
        }

        Task Behavior<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context)
        {
            var nextContext = context as BehaviorContext<TInstance, TData> ?? context.GetProxy(_event, _data);

            return _next.Execute(nextContext);
        }

        Task Behavior<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context)
        {
            var nextContext =
                context as BehaviorExceptionContext<TInstance, TData, TException> ?? context.GetProxy(_event, _data);

            return _next.Faulted(nextContext);
        }

        Task Behavior<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context)
        {
            var nextContext = context.GetProxy(_event, _data);

            return _next.Faulted(nextContext);
        }
    }
}
