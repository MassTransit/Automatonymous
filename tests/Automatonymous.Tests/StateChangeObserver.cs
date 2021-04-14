namespace Automatonymous.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GreenPipes.Util;


    class StateChangeObserver<T> :
        StateObserver<T>
        where T : class
    {
        public StateChangeObserver()
        {
            Events = new List<StateChange>();
        }

        public IList<StateChange> Events { get; }

        public Task StateChanged(InstanceContext<T> context, State currentState, State previousState)
        {
            Events.Add(new StateChange(context, currentState, previousState));

            return TaskUtil.Completed;
        }


        public struct StateChange
        {
            public InstanceContext<T> Context;
            public readonly State Current;
            public readonly State Previous;

            public StateChange(InstanceContext<T> context, State current, State previous)
            {
                Context = context;
                Current = current;
                Previous = previous;
            }
        }
    }
}
