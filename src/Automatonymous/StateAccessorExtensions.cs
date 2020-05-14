namespace Automatonymous
{
    using System.Threading.Tasks;
    using Contexts;


    public static class StateAccessorExtensions
    {
        public static Task<State<TInstance>> GetState<TInstance>(this StateAccessor<TInstance> accessor, TInstance instance)
            where TInstance : class
        {
            var context = new StateMachineInstanceContext<TInstance>(instance);

            return accessor.Get(context);
        }

        public static Task<State<TInstance>> GetState<TInstance>(this StateMachine<TInstance> accessor, TInstance instance)
            where TInstance : class
        {
            var context = new StateMachineInstanceContext<TInstance>(instance);

            return accessor.Accessor.Get(context);
        }
    }
}
