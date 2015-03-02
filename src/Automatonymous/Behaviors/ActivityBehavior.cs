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
namespace Automatonymous.Behaviors
{
    using System;
    using System.Threading.Tasks;


    public class ActivityBehavior<TInstance> :
        Behavior<TInstance>
    {
        readonly Activity<TInstance> _activity;
        readonly Behavior<TInstance> _next;

        public ActivityBehavior(Activity<TInstance> activity, Behavior<TInstance> next)
        {
            _activity = activity;
            _next = next;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x =>
            {
                _activity.Accept(visitor);
                _next.Accept(visitor);
            });
        }

        async Task Behavior<TInstance>.Execute(BehaviorContext<TInstance> context)
        {
            Exception activityException = null;
            try
            {
                await _activity.Execute(context, _next);
            }
            catch (Exception exception)
            {
                activityException = exception;
            }

            // once we can await in a catch, we're golden
            if (activityException != null)
            {
                await ExceptionTypeCache.Faulted(_next, context, activityException);
            }
        }

        async Task Behavior<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context)
        {
            var behavior = new DataBehavior<TInstance, T>(_next);

            Exception activityException = null;
            try
            {
                await _activity.Execute(context, behavior);
            }
            catch (Exception exception)
            {
                activityException = exception;
            }

            // once we can await in catch we're golden
            if (activityException != null)
            {
                await ExceptionTypeCache.Faulted(behavior, context, activityException);
            }
        }

        Task Behavior<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context)
        {
            var behavior = new DataBehavior<TInstance, T>(_next);

            return _activity.Faulted(context, behavior);
        }

        Task Behavior<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context)
        {
            return _activity.Faulted(context, _next);
        }
    }
}