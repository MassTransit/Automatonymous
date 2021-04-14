namespace Automatonymous.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    class EventRaisedObserver<TInstance> :
        EventObserver<TInstance>
    {
        public EventRaisedObserver()
        {
            Events = new List<EventContext<TInstance>>();
        }

        public IList<EventContext<TInstance>> Events { get; }

        public async Task PreExecute(EventContext<TInstance> context)
        {
        }

        public async Task PreExecute<T>(EventContext<TInstance, T> context)
        {
        }

        public async Task PostExecute(EventContext<TInstance> context)
        {
            Events.Add(context);
        }

        public async Task PostExecute<T>(EventContext<TInstance, T> context)
        {
            Events.Add(context);
        }

        public async Task ExecuteFault(EventContext<TInstance> context, Exception exception)
        {
        }

        public async Task ExecuteFault<T>(EventContext<TInstance, T> context, Exception exception)
        {
        }
    }
}
