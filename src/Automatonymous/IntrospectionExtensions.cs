namespace Automatonymous
{
    using System.Collections.Generic;
    using System.Linq;
    using Graphing;

    public static class IntrospectionExtensions
    {
       public static IEnumerable<Event> NextEvents<TInstance>(this StateMachine<TInstance> machine, TInstance instance) where TInstance : StateMachineInstance
       {
           var graph = machine.GetGraph();

           var current = instance.CurrentState;

           var eventNames = graph.Edges
               .Where(e => e.From.TargetType == typeof (State) && e.From.Title == current.Name)
               .Select(e=>e.Title).ToList();

           var allEvents = machine.AllEvents;

           return allEvents.Where(e => eventNames.Contains(e.Name));
       }

    }
}