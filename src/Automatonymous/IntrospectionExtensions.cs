namespace Automatonymous
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contexts;


    public static class IntrospectionExtensions
    {
        public static async Task<IEnumerable<Event>> NextEvents<T, TInstance>(this T machine, TInstance instance)
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
        {
            if (machine == null)
                throw new ArgumentNullException(nameof(machine));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var context = new StateMachineEventContext<TInstance>(machine, instance, machine.Initial.Enter, default);

            return machine.NextEvents(await machine.Accessor.Get(context).ConfigureAwait(false));
        }
    }
}
