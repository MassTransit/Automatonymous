// Copyright 2011 Chris Patterson, Dru Sellers
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
        public static EventActivityBinder<TInstance> Then<TInstance>(
            this EventActivityBinder<TInstance> source, Action<TInstance> action)
            where TInstance : class
        {
            return source.Add(new ActionActivity<TInstance>(action));
        }

        public static EventActivityBinder<TInstance> ThenAsync<TInstance>(
            this EventActivityBinder<TInstance> source, Func<TInstance, Task<TInstance>> action)
            where TInstance : class
        {
            return source.Add(new TaskAsyncActivity<TInstance>(action));
        }

        public static EventActivityBinder<TInstance> ThenAsync<TInstance>(
            this EventActivityBinder<TInstance> source, Func<TInstance, Task> action)
            where TInstance : class
        {
            return source.Add(new TaskAsyncActivity<TInstance>(action));
        }

        public static EventActivityBinder<TInstance, TData> Then<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Action<TInstance> action)
            where TInstance : class
        {
            return source.Add(new ActionActivity<TInstance>(action));
        }

        public static EventActivityBinder<TInstance, TData> ThenAsync<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Func<TInstance, Task<TInstance>> action)
            where TInstance : class
        {
            return source.Add(new TaskAsyncActivity<TInstance>(action));
        }

        public static EventActivityBinder<TInstance, TData> ThenAsync<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Func<TInstance, Task> action)
            where TInstance : class
        {
            return source.Add(new TaskAsyncActivity<TInstance>(action));
        }

        public static EventActivityBinder<TInstance, TData> Then<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Action<TInstance, TData> action)
            where TInstance : class
        {
            var activity = new ActionActivity<TInstance, TData>(action);
            var adapter = new DataConverterActivity<TInstance, TData>(activity);
            return source.Add(adapter);
        }

        public static EventActivityBinder<TInstance, TData> ThenAsync<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Func<TInstance, TData, Task<TInstance>> action)
            where TInstance : class
        {
            var activity = new TaskAsyncActivity<TInstance, TData>(action);
            var adapter = new AsyncDataConverterActivity<TInstance, TData>(activity);
            return source.Add(adapter);
        }

        public static EventActivityBinder<TInstance, TData> ThenAsync<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Func<TInstance, TData, Task> action)
            where TInstance : class
        {
            var activity = new TaskAsyncActivity<TInstance, TData>(action);
            var adapter = new AsyncDataConverterActivity<TInstance, TData>(activity);
            return source.Add(adapter);
        }

        public static EventActivityBinder<TInstance> Then<TInstance>(
            this EventActivityBinder<TInstance> source, Func<Activity<TInstance>> activityFactory)
            where TInstance : class
        {
            var activity = new FactoryEventActivity<TInstance>(activityFactory);
            return source.Add(activity);
        }

        public static EventActivityBinder<TInstance> Then<TInstance>(
            this EventActivityBinder<TInstance> source, Func<AsyncActivity<TInstance>> activityFactory)
            where TInstance : class
        {
            var activity = new AsyncFactoryEventActivity<TInstance>(activityFactory);
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
            this EventActivityBinder<TInstance, TData> source, Func<AsyncActivity<TInstance, TData>> activityFactory)
            where TInstance : class
        {
            var activity = new AsyncFactoryEventActivity<TInstance, TData>(activityFactory);
            return source.Add(activity);
        }

        public static EventActivityBinder<TInstance, TData> Then<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Func<Activity<TInstance>> activityFactory)
            where TInstance : class
        {
            var activity = new FactoryEventActivity<TInstance>(activityFactory);
            return source.Add(activity);
        }

        public static EventActivityBinder<TInstance, TData> Then<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Func<AsyncActivity<TInstance>> activityFactory)
            where TInstance : class
        {
            var activity = new AsyncFactoryEventActivity<TInstance>(activityFactory);
            return source.Add(activity);
        }
    }
}