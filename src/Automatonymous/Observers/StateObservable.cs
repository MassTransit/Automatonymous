namespace Automatonymous.Observers
{
    using System.Threading.Tasks;
    using GreenPipes.Util;


    class StateObservable<TInstance> :
        Connectable<StateObserver<TInstance>>,
        StateObserver<TInstance>
    {
        public Task StateChanged(InstanceContext<TInstance> context, State currentState, State previousState)
        {
            return ForEachAsync(x => x.StateChanged(context, currentState, previousState));
        }
    }
}
