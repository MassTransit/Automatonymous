// Copyright 2011-2015 Chris Patterson, Dru Sellers
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
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
        public static EventActivityBinder<TInstance> Then<TInstance>(
            this EventActivityBinder<TInstance> binder, Action<BehaviorContext<TInstance>> action)
            where TInstance : class
        {
            return binder.Add(new ActionActivity<TInstance>(action));
        }

        /// <summary>
        /// Adds an asynchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The asynchronous delegate</param>
        public static EventActivityBinder<TInstance> ThenAsync<TInstance>(
            this EventActivityBinder<TInstance> binder, Func<BehaviorContext<TInstance>, Task> action)
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
        public static EventActivityBinder<TInstance, TData> Then<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> binder, Action<BehaviorContext<TInstance, TData>> action)
            where TInstance : class
        {
            return binder.Add(new ActionActivity<TInstance, TData>(action));
        }

        /// <summary>
        /// Adds an asynchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The asynchronous delegate</param>
        public static EventActivityBinder<TInstance, TData> ThenAsync<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> binder, Func<BehaviorContext<TInstance, TData>, Task> action)
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
        public static EventActivityBinder<TInstance> Execute<TInstance>(
            this EventActivityBinder<TInstance> binder, Func<BehaviorContext<TInstance>, Activity<TInstance>> activityFactory)
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
        /// <param name="activityFactory">The factory method which returns the activity to execute</param>
        public static EventActivityBinder<TInstance> ExecuteAsync<TInstance>(
            this EventActivityBinder<TInstance> binder, Func<BehaviorContext<TInstance>, Task<Activity<TInstance>>> activityFactory)
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
        public static EventActivityBinder<TInstance, TData> Execute<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> binder,
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
        public static EventActivityBinder<TInstance, TData> Execute<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> binder, Func<BehaviorContext<TInstance, TData>, Activity<TInstance>> activityFactory)
            where TInstance : class
        {
            var activity = new FactoryActivity<TInstance, TData>(context =>
            {
                Activity<TInstance> newActivity = activityFactory(context);

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
                Activity<TInstance> newActivity = await activityFactory(context);

                return new SlimActivity<TInstance, TData>(newActivity);
            });

            return binder.Add(activity);
        }
    }
}