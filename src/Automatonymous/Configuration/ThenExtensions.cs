// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
            this EventActivityBinder<TInstance> source, Action<BehaviorContext<TInstance>> action)
            where TInstance : class
        {
            return source.Add(new ActionActivity<TInstance>(action));
        }

        public static EventActivityBinder<TInstance> ThenAsync<TInstance>(
            this EventActivityBinder<TInstance> source, Func<BehaviorContext<TInstance>, Task> action)
            where TInstance : class
        {
            return source.Add(new AsyncActivity<TInstance>(action));
        }

        public static EventActivityBinder<TInstance, TData> Then<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Action<BehaviorContext<TInstance, TData>> action)
            where TInstance : class
        {
            return source.Add(new ActionActivity<TInstance, TData>(action));
        }

        public static EventActivityBinder<TInstance, TData> ThenAsync<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Func<BehaviorContext<TInstance, TData>, Task> action)
            where TInstance : class
        {
            return source.Add(new AsyncActivity<TInstance, TData>(action));
        }

        public static EventActivityBinder<TInstance> Execute<TInstance>(
            this EventActivityBinder<TInstance> source, Func<BehaviorContext<TInstance>, Activity<TInstance>> activityFactory)
            where TInstance : class
        {
            var activity = new FactoryActivity<TInstance>(activityFactory);
            return source.Add(activity);
        }

        public static EventActivityBinder<TInstance, TData> Execute<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source,
            Func<BehaviorContext<TInstance, TData>, Activity<TInstance, TData>> activityFactory)
            where TInstance : class
        {
            var activity = new FactoryActivity<TInstance, TData>(activityFactory);
            return source.Add(activity);
        }

        public static EventActivityBinder<TInstance, TData> Execute<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Func<BehaviorContext<TInstance, TData>, Activity<TInstance>> activityFactory)
            where TInstance : class
        {
            var activity = new FactoryActivity<TInstance, TData>(context =>
            {
                Activity<TInstance> newActivity = activityFactory(context);

                return new SlimActivity<TInstance, TData>(newActivity);
            });

            return source.Add(activity);
        }
    }
}