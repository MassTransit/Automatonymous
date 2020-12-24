namespace Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using Activities;
    using Binders;


    public static class ThenExtensions
    {
        /// <summary>
        /// Adds a synchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The synchronous delegate</param>
        public static EventActivityBinder<TInstance> Then<TInstance>(this EventActivityBinder<TInstance> binder,
            Action<BehaviorContext<TInstance>> action)
            where TInstance : class
        {
            return binder.Add(new ActionActivity<TInstance>(action));
        }

        /// <summary>
        /// Adds a synchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The synchronous delegate</param>
        public static ExceptionActivityBinder<TInstance, TException> Then<TInstance, TException>(
            this ExceptionActivityBinder<TInstance, TException> binder, Action<BehaviorExceptionContext<TInstance, TException>> action)
            where TInstance : class
            where TException : Exception
        {
            return binder.Add(new FaultedActionActivity<TInstance, TException>(action));
        }

        /// <summary>
        /// Adds a asynchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="asyncAction">The asynchronous delegate</param>
        public static ExceptionActivityBinder<TInstance, TException> ThenAsync<TInstance, TException>(
            this ExceptionActivityBinder<TInstance, TException> binder,
            Func<BehaviorExceptionContext<TInstance, TException>, Task> asyncAction)
            where TInstance : class
            where TException : Exception
        {
            return binder.Add(new AsyncFaultedActionActivity<TInstance, TException>(asyncAction));
        }

        /// <summary>
        /// Adds an asynchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The asynchronous delegate</param>
        public static EventActivityBinder<TInstance> ThenAsync<TInstance>(this EventActivityBinder<TInstance> binder,
            Func<BehaviorContext<TInstance>, Task> action)
            where TInstance : class
        {
            return binder.Add(new AsyncActivity<TInstance>(action));
        }

        /// <summary>
        /// Adds a synchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The synchronous delegate</param>
        public static EventActivityBinder<TInstance, TData> Then<TInstance, TData>(this EventActivityBinder<TInstance, TData> binder,
            Action<BehaviorContext<TInstance, TData>> action)
            where TInstance : class
        {
            return binder.Add(new ActionActivity<TInstance, TData>(action));
        }

        /// <summary>
        /// Adds a synchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The synchronous delegate</param>
        public static ExceptionActivityBinder<TInstance, TData, TException> Then<TInstance, TData, TException>(
            this ExceptionActivityBinder<TInstance, TData, TException> binder,
            Action<BehaviorExceptionContext<TInstance, TData, TException>> action)
            where TInstance : class
            where TException : Exception
        {
            return binder.Add(new FaultedActionActivity<TInstance, TData, TException>(action));
        }

        /// <summary>
        /// Adds a asynchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="asyncAction">The asynchronous delegate</param>
        public static ExceptionActivityBinder<TInstance, TData, TException> ThenAsync<TInstance, TData, TException>(
            this ExceptionActivityBinder<TInstance, TData, TException> binder,
            Func<BehaviorExceptionContext<TInstance, TData, TException>, Task> asyncAction)
            where TInstance : class
            where TException : Exception
        {
            return binder.Add(new AsyncFaultedActionActivity<TInstance, TData, TException>(asyncAction));
        }

        /// <summary>
        /// Adds an asynchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The asynchronous delegate</param>
        public static EventActivityBinder<TInstance, TData> ThenAsync<TInstance, TData>(this EventActivityBinder<TInstance, TData> binder,
            Func<BehaviorContext<TInstance, TData>, Task> action)
            where TInstance : class
        {
            return binder.Add(new AsyncActivity<TInstance, TData>(action));
        }

        /// <summary>
        /// Add an activity execution to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="activityFactory">The factory method which returns the activity to execute</param>
        public static EventActivityBinder<TInstance> Execute<TInstance>(this EventActivityBinder<TInstance> binder,
            Func<BehaviorContext<TInstance>, Activity<TInstance>> activityFactory)
            where TInstance : class
        {
            var activity = new FactoryActivity<TInstance>(activityFactory);
            return binder.Add(activity);
        }

        /// <summary>
        /// Add an activity execution to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="activity">An existing activity</param>
        public static EventActivityBinder<TInstance> Execute<TInstance>(this EventActivityBinder<TInstance> binder,
            Activity<TInstance> activity)
            where TInstance : class
        {
            return binder.Add(activity);
        }

        /// <summary>
        /// Add an activity execution to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="activityFactory">The factory method which returns the activity to execute</param>
        public static EventActivityBinder<TInstance> ExecuteAsync<TInstance>(this EventActivityBinder<TInstance> binder,
            Func<BehaviorContext<TInstance>, Task<Activity<TInstance>>> activityFactory)
            where TInstance : class
        {
            var activity = new AsyncFactoryActivity<TInstance>(activityFactory);
            return binder.Add(activity);
        }

        /// <summary>
        /// Add an activity execution to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="activityFactory">The factory method which returns the activity to execute</param>
        public static EventActivityBinder<TInstance, TData> Execute<TInstance, TData>(this EventActivityBinder<TInstance, TData> binder,
            Func<BehaviorContext<TInstance, TData>, Activity<TInstance, TData>> activityFactory)
            where TInstance : class
        {
            var activity = new FactoryActivity<TInstance, TData>(activityFactory);
            return binder.Add(activity);
        }

        /// <summary>
        /// Add an activity execution to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="activityFactory">The factory method which returns the activity to execute</param>
        public static EventActivityBinder<TInstance, TData> ExecuteAsync<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> binder,
            Func<BehaviorContext<TInstance, TData>, Task<Activity<TInstance, TData>>> activityFactory)
            where TInstance : class
        {
            var activity = new AsyncFactoryActivity<TInstance, TData>(activityFactory);
            return binder.Add(activity);
        }

        /// <summary>
        /// Add an activity execution to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="activityFactory">The factory method which returns the activity to execute</param>
        public static EventActivityBinder<TInstance, TData> Execute<TInstance, TData>(this EventActivityBinder<TInstance, TData> binder,
            Func<BehaviorContext<TInstance, TData>, Activity<TInstance>> activityFactory)
            where TInstance : class
        {
            var activity = new FactoryActivity<TInstance, TData>(context =>
            {
                var newActivity = activityFactory(context);

                return new SlimActivity<TInstance, TData>(newActivity);
            });

            return binder.Add(activity);
        }

        /// <summary>
        /// Add an activity execution to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="activityFactory">The factory method which returns the activity to execute</param>
        public static EventActivityBinder<TInstance, TData> ExecuteAsync<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> binder,
            Func<BehaviorContext<TInstance, TData>, Task<Activity<TInstance>>> activityFactory)
            where TInstance : class
        {
            var activity = new AsyncFactoryActivity<TInstance, TData>(async context =>
            {
                var newActivity = await activityFactory(context).ConfigureAwait(false);

                return new SlimActivity<TInstance, TData>(newActivity);
            });

            return binder.Add(activity);
        }
    }
}
