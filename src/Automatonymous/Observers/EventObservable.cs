namespace Automatonymous.Observers
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Util;


    class EventObservable<TInstance> :
        Connectable<EventObserver<TInstance>>,
        EventObserver<TInstance>
    {
        Task EventObserver<TInstance>.PreExecute(EventContext<TInstance> context)
        {
            return ForEachAsync(x => x.PreExecute(context));
        }

        Task EventObserver<TInstance>.PreExecute<T>(EventContext<TInstance, T> context)
        {
            return ForEachAsync(x => x.PreExecute(context));
        }

        Task EventObserver<TInstance>.PostExecute(EventContext<TInstance> context)
        {
            return ForEachAsync(x => x.PostExecute(context));
        }

        Task EventObserver<TInstance>.PostExecute<T>(EventContext<TInstance, T> context)
        {
            return ForEachAsync(x => x.PostExecute(context));
        }

        Task EventObserver<TInstance>.ExecuteFault(EventContext<TInstance> context, Exception exception)
        {
            return ForEachAsync(x => x.ExecuteFault(context, exception));
        }

        Task EventObserver<TInstance>.ExecuteFault<T>(EventContext<TInstance, T> context, Exception exception)
        {
            return ForEachAsync(x => x.ExecuteFault(context, exception));
        }
    }
}
