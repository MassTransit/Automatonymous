namespace Automatonymous.Events
{
    class StateMachineEvent
    {
        public Event Event { get; set; }
        public bool IsTransitionEvent { get; }

        public StateMachineEvent(Event @event, bool isTransitionEvent)
        {
            Event = @event;
            IsTransitionEvent = isTransitionEvent;
        }
    }
}
