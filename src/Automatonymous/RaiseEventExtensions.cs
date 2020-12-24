namespace Automatonymous
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;


    [DebuggerNonUserCode]
    public static class RaiseEventExtensions
    {
        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance>(this T machine, TInstance instance, Event @event,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance>(this T machine, TInstance instance, Func<T, Event> eventSelector,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance>(this T machine, TInstance instance, Event<TData> @event, TData data,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance>(this T machine, TInstance instance, Func<T, Event<TData>> eventSelector,
            TData data,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1>(this T machine, TInstance instance, Event @event, T1 context1,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1>(this T machine, TInstance instance, Func<T, Event> eventSelector, T1 context1,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1>(this T machine, TInstance instance, Event<TData> @event, TData data,
            T1 context1,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1>(this T machine, TInstance instance, Func<T, Event<TData>> eventSelector,
            TData data, T1 context1,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2>(this T machine, TInstance instance, Event @event, T1 context1, T2 context2,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2>(this T machine, TInstance instance, Func<T, Event> eventSelector, T1 context1,
            T2 context2,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2>(this T machine, TInstance instance, Event<TData> @event, TData data,
            T1 context1, T2 context2,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2>(this T machine, TInstance instance, Func<T, Event<TData>> eventSelector,
            TData data, T1 context1, T2 context2,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3>(this T machine, TInstance instance, Event @event, T1 context1, T2 context2,
            T3 context3,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3>(this T machine, TInstance instance, Func<T, Event> eventSelector,
            T1 context1, T2 context2, T3 context3,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3>(this T machine, TInstance instance, Event<TData> @event, TData data,
            T1 context1, T2 context2, T3 context3,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3>(this T machine, TInstance instance,
            Func<T, Event<TData>> eventSelector, TData data, T1 context1, T2 context2, T3 context3,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4>(this T machine, TInstance instance, Event @event, T1 context1,
            T2 context2, T3 context3, T4 context4,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4>(this T machine, TInstance instance, Func<T, Event> eventSelector,
            T1 context1, T2 context2, T3 context3, T4 context4,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4>(this T machine, TInstance instance, Event<TData> @event,
            TData data, T1 context1, T2 context2, T3 context3, T4 context4,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4>(this T machine, TInstance instance,
            Func<T, Event<TData>> eventSelector, TData data, T1 context1, T2 context2, T3 context3, T4 context4,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5>(this T machine, TInstance instance, Event @event, T1 context1,
            T2 context2, T3 context3, T4 context4, T5 context5,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5>(this T machine, TInstance instance, Func<T, Event> eventSelector,
            T1 context1, T2 context2, T3 context3, T4 context4, T5 context5,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5>(this T machine, TInstance instance, Event<TData> @event,
            TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5>(this T machine, TInstance instance,
            Func<T, Event<TData>> eventSelector, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6>(this T machine, TInstance instance, Event @event, T1 context1,
            T2 context2, T3 context3, T4 context4, T5 context5, T6 context6,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6>(this T machine, TInstance instance, Func<T, Event> eventSelector,
            T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6>(this T machine, TInstance instance, Event<TData> @event,
            TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6>(this T machine, TInstance instance,
            Func<T, Event<TData>> eventSelector, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7>(this T machine, TInstance instance, Event @event,
            T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7>(this T machine, TInstance instance,
            Func<T, Event> eventSelector, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7>(this T machine, TInstance instance,
            Event<TData> @event, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7>(this T machine, TInstance instance,
            Func<T, Event<TData>> eventSelector, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6,
            T7 context7,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8>(this T machine, TInstance instance, Event @event,
            T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7, T8 context8,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8>(this T machine, TInstance instance,
            Func<T, Event> eventSelector, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            T8 context8,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8>(this T machine, TInstance instance,
            Event<TData> @event, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            T8 context8,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8>(this T machine, TInstance instance,
            Func<T, Event<TData>> eventSelector, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6,
            T7 context7, T8 context8,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this T machine, TInstance instance, Event @event,
            T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7, T8 context8, T9 context9,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this T machine, TInstance instance,
            Func<T, Event> eventSelector, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            T8 context8, T9 context9,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this T machine, TInstance instance,
            Event<TData> @event, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            T8 context8, T9 context9,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this T machine, TInstance instance,
            Func<T, Event<TData>> eventSelector, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6,
            T7 context7, T8 context8, T9 context9,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this T machine, TInstance instance,
            Event @event, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7, T8 context8,
            T9 context9, T10 context10,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this T machine, TInstance instance,
            Func<T, Event> eventSelector, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            T8 context8, T9 context9, T10 context10,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this T machine, TInstance instance,
            Event<TData> @event, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            T8 context8, T9 context9, T10 context10,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this T machine, TInstance instance,
            Func<T, Event<TData>> eventSelector, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6,
            T7 context7, T8 context8, T9 context9, T10 context10,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this T machine, TInstance instance,
            Event @event, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7, T8 context8,
            T9 context9, T10 context10, T11 context11,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this T machine, TInstance instance,
            Func<T, Event> eventSelector, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            T8 context8, T9 context9, T10 context10, T11 context11,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this T machine, TInstance instance,
            Event<TData> @event, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            T8 context8, T9 context9, T10 context10, T11 context11,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this T machine, TInstance instance,
            Func<T, Event<TData>> eventSelector, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6,
            T7 context7, T8 context8, T9 context9, T10 context10, T11 context11,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this T machine, TInstance instance,
            Event @event, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7, T8 context8,
            T9 context9, T10 context10, T11 context11, T12 context12,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this T machine, TInstance instance,
            Func<T, Event> eventSelector, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            T8 context8, T9 context9, T10 context10, T11 context11, T12 context12,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this T machine,
            TInstance instance, Event<TData> @event, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5,
            T6 context6, T7 context7, T8 context8, T9 context9, T10 context10, T11 context11, T12 context12,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this T machine,
            TInstance instance, Func<T, Event<TData>> eventSelector, TData data, T1 context1, T2 context2, T3 context3, T4 context4,
            T5 context5, T6 context6, T7 context7, T8 context8, T9 context9, T10 context10, T11 context11, T12 context12,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this T machine,
            TInstance instance, Event @event, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this T machine,
            TInstance instance, Func<T, Event> eventSelector, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6,
            T7 context7, T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this T machine,
            TInstance instance, Event<TData> @event, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5,
            T6 context6, T7 context7, T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this T machine,
            TInstance instance, Func<T, Event<TData>> eventSelector, TData data, T1 context1, T2 context2, T3 context3, T4 context4,
            T5 context5, T6 context6, T7 context7, T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="context14">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this T machine,
            TInstance instance, Event @event, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13, T14 context14,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
            where T14 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);
            context.GetOrAddPayload(() => context14);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="context14">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this T machine,
            TInstance instance, Func<T, Event> eventSelector, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6,
            T7 context7, T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13, T14 context14,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
            where T14 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);
            context.GetOrAddPayload(() => context14);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="context14">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this T machine,
            TInstance instance, Event<TData> @event, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5,
            T6 context6, T7 context7, T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13, T14 context14,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
            where T14 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);
            context.GetOrAddPayload(() => context14);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="context14">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this T machine,
            TInstance instance, Func<T, Event<TData>> eventSelector, TData data, T1 context1, T2 context2, T3 context3, T4 context4,
            T5 context5, T6 context6, T7 context7, T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13,
            T14 context14,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
            where T14 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);
            context.GetOrAddPayload(() => context14);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="context14">An additional context added to the event context</param>
        /// <param name="context15">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this T machine,
            TInstance instance, Event @event, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13, T14 context14, T15 context15,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
            where T14 : class
            where T15 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);
            context.GetOrAddPayload(() => context14);
            context.GetOrAddPayload(() => context15);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="context14">An additional context added to the event context</param>
        /// <param name="context15">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this T machine,
            TInstance instance, Func<T, Event> eventSelector, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6,
            T7 context7, T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13, T14 context14, T15 context15,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
            where T14 : class
            where T15 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);
            context.GetOrAddPayload(() => context14);
            context.GetOrAddPayload(() => context15);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="context14">An additional context added to the event context</param>
        /// <param name="context15">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this T machine,
            TInstance instance, Event<TData> @event, TData data, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5,
            T6 context6, T7 context7, T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13, T14 context14,
            T15 context15,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
            where T14 : class
            where T15 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);
            context.GetOrAddPayload(() => context14);
            context.GetOrAddPayload(() => context15);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="context14">An additional context added to the event context</param>
        /// <param name="context15">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this T machine,
            TInstance instance, Func<T, Event<TData>> eventSelector, TData data, T1 context1, T2 context2, T3 context3, T4 context4,
            T5 context5, T6 context6, T7 context7, T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13,
            T14 context14, T15 context15,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
            where T14 : class
            where T15 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);
            context.GetOrAddPayload(() => context14);
            context.GetOrAddPayload(() => context15);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="context14">An additional context added to the event context</param>
        /// <param name="context15">An additional context added to the event context</param>
        /// <param name="context16">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this T machine,
            TInstance instance, Event @event, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6, T7 context7,
            T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13, T14 context14, T15 context15,
            T16 context16,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
            where T14 : class
            where T15 : class
            where T16 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);
            context.GetOrAddPayload(() => context14);
            context.GetOrAddPayload(() => context15);
            context.GetOrAddPayload(() => context16);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a simple event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="context14">An additional context added to the event context</param>
        /// <param name="context15">An additional context added to the event context</param>
        /// <param name="context16">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this T machine,
            TInstance instance, Func<T, Event> eventSelector, T1 context1, T2 context2, T3 context3, T4 context4, T5 context5, T6 context6,
            T7 context7, T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13, T14 context14, T15 context15,
            T16 context16,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
            where T14 : class
            where T15 : class
            where T16 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance>(machine, instance, @event, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);
            context.GetOrAddPayload(() => context14);
            context.GetOrAddPayload(() => context15);
            context.GetOrAddPayload(() => context16);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="event">The event to raise</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="context14">An additional context added to the event context</param>
        /// <param name="context15">An additional context added to the event context</param>
        /// <param name="context16">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
            this T machine, TInstance instance, Event<TData> @event, TData data, T1 context1, T2 context2, T3 context3, T4 context4,
            T5 context5, T6 context6, T7 context7, T8 context8, T9 context9, T10 context10, T11 context11, T12 context12, T13 context13,
            T14 context14, T15 context15, T16 context16,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
            where T14 : class
            where T15 : class
            where T16 : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);
            context.GetOrAddPayload(() => context14);
            context.GetOrAddPayload(() => context15);
            context.GetOrAddPayload(() => context16);

            return machine.RaiseEvent(context);
        }

        /// <summary>
        /// Raise a data event on the state machine
        /// </summary>
        /// <typeparam name="T">The state machine type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <typeparam name="TInstance">The instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state machine instance</param>
        /// <param name="eventSelector">Selector to the event on the state machine</param>
        /// <param name="data">The event data</param>
        /// <param name="context1">An additional context added to the event context</param>
        /// <param name="context2">An additional context added to the event context</param>
        /// <param name="context3">An additional context added to the event context</param>
        /// <param name="context4">An additional context added to the event context</param>
        /// <param name="context5">An additional context added to the event context</param>
        /// <param name="context6">An additional context added to the event context</param>
        /// <param name="context7">An additional context added to the event context</param>
        /// <param name="context8">An additional context added to the event context</param>
        /// <param name="context9">An additional context added to the event context</param>
        /// <param name="context10">An additional context added to the event context</param>
        /// <param name="context11">An additional context added to the event context</param>
        /// <param name="context12">An additional context added to the event context</param>
        /// <param name="context13">An additional context added to the event context</param>
        /// <param name="context14">An additional context added to the event context</param>
        /// <param name="context15">An additional context added to the event context</param>
        /// <param name="context16">An additional context added to the event context</param>
        /// <param name="cancellationToken"></param>
        public static Task RaiseEvent<T, TData, TInstance, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
            this T machine, TInstance instance, Func<T, Event<TData>> eventSelector, TData data, T1 context1, T2 context2, T3 context3,
            T4 context4, T5 context5, T6 context6, T7 context7, T8 context8, T9 context9, T10 context10, T11 context11, T12 context12,
            T13 context13, T14 context14, T15 context15, T16 context16,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
            where T9 : class
            where T10 : class
            where T11 : class
            where T12 : class
            where T13 : class
            where T14 : class
            where T15 : class
            where T16 : class
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector),
                    "The event selector did not return a valid event from the state machine");

            var context = new StateMachineEventContext<TInstance, TData>(machine, instance, @event, data, cancellationToken);
            context.GetOrAddPayload(() => context1);
            context.GetOrAddPayload(() => context2);
            context.GetOrAddPayload(() => context3);
            context.GetOrAddPayload(() => context4);
            context.GetOrAddPayload(() => context5);
            context.GetOrAddPayload(() => context6);
            context.GetOrAddPayload(() => context7);
            context.GetOrAddPayload(() => context8);
            context.GetOrAddPayload(() => context9);
            context.GetOrAddPayload(() => context10);
            context.GetOrAddPayload(() => context11);
            context.GetOrAddPayload(() => context12);
            context.GetOrAddPayload(() => context13);
            context.GetOrAddPayload(() => context14);
            context.GetOrAddPayload(() => context15);
            context.GetOrAddPayload(() => context16);

            return machine.RaiseEvent(context);
        }
    }
}
