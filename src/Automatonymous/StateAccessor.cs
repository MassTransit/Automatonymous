namespace Automatonymous
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using GreenPipes;


    public interface StateAccessor<TInstance> :
        IProbeSite
    {
        Task<State<TInstance>> Get(InstanceContext<TInstance> context);

        Task Set(InstanceContext<TInstance> context, State<TInstance> state);

        /// <summary>
        /// Converts a state expression to the instance current state property type.
        /// </summary>
        /// <param name="states"></param>
        /// <returns></returns>
        Expression<Func<TInstance, bool>> GetStateExpression(params State[] states);
    }
}
