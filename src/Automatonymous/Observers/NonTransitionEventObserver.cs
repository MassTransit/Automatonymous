namespace Automatonymous.Observers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Events;
    using GreenPipes.Util;


    class NonTransitionEventObserver<TInstance> :
        EventObserver<TInstance>
    {
        readonly IReadOnlyDictionary<string, StateMachineEvent> _eventCache;
        readonly EventObserver<TInstance> _observer;

        public NonTransitionEventObserver(IReadOnlyDictionary<string, StateMachineEvent> eventCache,
            EventObserver<TInstance> observer)
        {
            _eventCache = eventCache;
            _observer = observer;
        }

        public Task PreExecute(EventContext<TInstance> context)
        {
            if (_eventCache.TryGetValue(context.Event.Name, out var stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.PreExecute(context);

            return TaskUtil.Completed;
        }

        public Task PreExecute<T>(EventContext<TInstance, T> context)
        {
            if (_eventCache.TryGetValue(context.Event.Name, out var stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.PreExecute(context);

            return TaskUtil.Completed;
        }

        public Task PostExecute(EventContext<TInstance> context)
        {
            if (_eventCache.TryGetValue(context.Event.Name, out var stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.PostExecute(context);

            return TaskUtil.Completed;
        }

        public Task PostExecute<T>(EventContext<TInstance, T> context)
        {
            if (_eventCache.TryGetValue(context.Event.Name, out var stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.PostExecute(context);

            return TaskUtil.Completed;
        }

        public Task ExecuteFault(EventContext<TInstance> context, Exception exception)
        {
            if (_eventCache.TryGetValue(context.Event.Name, out var stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.ExecuteFault(context, exception);

            return TaskUtil.Completed;
        }

        public Task ExecuteFault<T>(EventContext<TInstance, T> context, Exception exception)
        {
            if (_eventCache.TryGetValue(context.Event.Name, out var stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.ExecuteFault(context, exception);

            return TaskUtil.Completed;
        }
    }
}
