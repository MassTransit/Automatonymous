namespace Automatonymous
{
    using System;


    public static class ObserverExtensions
    {
        public static IDisposable ConnectStateObserver<T>(this StateMachine<T> machine, StateObserver<T> observer)
            where T : class
        {
            return machine.ConnectStateObserver(observer);
        }

        public static IDisposable ConnectEventObserver<T>(this StateMachine<T> machine, EventObserver<T> observer)
            where T : class
        {
            return machine.ConnectEventObserver(observer);
        }

        public static IDisposable ConnectEventObserver<T>(this StateMachine<T> machine, Event @event, EventObserver<T> observer)
            where T : class
        {
            return machine.ConnectEventObserver(@event, observer);
        }
    }
}
