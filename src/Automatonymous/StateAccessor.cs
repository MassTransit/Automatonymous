namespace Automatonymous
{
    using System.Threading.Tasks;
    using GreenPipes;


    public interface StateAccessor<TInstance> :
        IProbeSite
    {
        Task<State<TInstance>> Get(InstanceContext<TInstance> context);

        Task Set(InstanceContext<TInstance> context, State<TInstance> state);
    }
}
