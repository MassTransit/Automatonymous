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
namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using Behaviors;


    public class AsyncFactoryActivity<TInstance, TData> :
        Activity<TInstance, TData>
    {
        readonly Func<BehaviorContext<TInstance, TData>, Task<Activity<TInstance, TData>>> _activityFactory;

        public AsyncFactoryActivity(Func<BehaviorContext<TInstance, TData>, Task<Activity<TInstance, TData>>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            Activity<TInstance, TData> activity = await _activityFactory(context).ConfigureAwait(false);

            await activity.Execute(context, next).ConfigureAwait(false);
        }

        Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context,
            Behavior<TInstance, TData> next)
        {
            return next.Faulted(context);
        }
    }


    public class AsyncFactoryActivity<TInstance> :
        Activity<TInstance>
    {
        readonly Func<BehaviorContext<TInstance>, Task<Activity<TInstance>>> _activityFactory;

        public AsyncFactoryActivity(Func<BehaviorContext<TInstance>, Task<Activity<TInstance>>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            Activity<TInstance> activity = await _activityFactory(context).ConfigureAwait(false);

            await activity.Execute(context, next).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            Activity<TInstance> activity = await _activityFactory(context).ConfigureAwait(false);

            var upconvert = new WidenBehavior<TInstance, T>(next, context);

            await activity.Execute(context, upconvert).ConfigureAwait(false);
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            return next.Faulted(context);
        }

        Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context,
            Behavior<TInstance, T> next)
        {
            return next.Faulted(context);
        }
    }
}