// Copyright 2011 Chris Patterson, Dru Sellers, Henrik Feldt
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

    /// <summary>
    /// Extensions for chaining actions onto <see cref="EventActivity{TInstance}"/>.
    /// </summary>
    public static class ThenExtensions
    {
        public static EventActivityBinder<TInstance> Then<TInstance>(
            this EventActivityBinder<TInstance> source, Action<TInstance> action)
            where TInstance : class
        {
            return source.Add(new ActionActivity<TInstance>(action));
        }

        /// <summary>
        /// After the specified event, run the asynchronous func passed as a parameter.
        /// </summary>
        /// <typeparam name="TInstance">Instance of state for the state machine</typeparam>
        /// <param name="source">The activity binder that is fed from 
        /// <see cref="AutomatonymousStateMachine{TInstance}.When"/>.</param>
        /// <param name="func">An async lambda taking the state instance</param>
        /// <example>
        /// ...
        /// When(Initial.AfterLeave)
        ///     .ThenAsync(async (instance) => await Task.Delay(10)),
        /// ...
        /// </example>
        /// <returns>A chainable activity binder</returns>
        public static EventActivityBinder<TInstance> ThenAsync<TInstance>(
            this EventActivityBinder<TInstance> source, Func<TInstance, Task> func)
            where TInstance : class
        {
            return source.Add(new AsyncActionActivity<TInstance>(func));
        }

        public static EventActivityBinder<TInstance, TData> Then<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Action<TInstance> action)
            where TInstance : class
        {
            return source.Add(new ActionActivity<TInstance>(action));
        }

        /// <summary>
        /// After the specified event, run the asynchronous func passed as a parameter.
        /// </summary>
        /// <typeparam name="TInstance">The type of the state instance</typeparam>
        /// <typeparam name="TData">The type of the data passed to the RaiseEvent function</typeparam>
        /// <param name="source">The activity binder that is fed from 
        /// <see cref="AutomatonymousStateMachine{TInstance}.When"/>.</param>
        /// <param name="func">An async lambda taking the state instance</param>        
        /// <example>
        /// When(Initial.AfterLeave)
        ///     .ThenAsync(async (instance, state) =>
        ///         {
        ///             instance.LeftState = state;
        ///             await Task.Delay(10);
        ///         }),
        /// ...
        /// </example>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> ThenAsync<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Func<TInstance, TData, Task> func)
            where TInstance : class
        {
            return source.Add(new AsyncActionActivity<TInstance, TData>(func));
        }

        public static EventActivityBinder<TInstance, TData> Then<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Action<TInstance, TData> action)
            where TInstance : class
        {
            var activity = new ActionActivity<TInstance, TData>(action);
            var adapter = new DataConverterActivity<TInstance, TData>(activity);
            return source.Add(adapter);
        }

        public static EventActivityBinder<TInstance> Then<TInstance>(
            this EventActivityBinder<TInstance> source, Func<Activity<TInstance>> activityFactory)
            where TInstance : class
        {
            var activity = new FactoryEventActivity<TInstance>(activityFactory);
            return source.Add(activity);
        }

        public static EventActivityBinder<TInstance, TData> Then<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Func<Activity<TInstance, TData>> activityFactory)
            where TInstance : class
        {
            var activity = new FactoryEventActivity<TInstance, TData>(activityFactory);
            return source.Add(activity);
        }

        public static EventActivityBinder<TInstance, TData> Then<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Func<Activity<TInstance>> activityFactory)
            where TInstance : class
        {
            var activity = new FactoryEventActivity<TInstance>(activityFactory);
            return source.Add(activity);
        }
    }
}