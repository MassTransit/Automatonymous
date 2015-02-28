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
    using Contexts;


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
            Exception faultException = null;
            try
            {
                await _activity.Execute(context, _next);
            }
            catch (Exception exception)
            {
                faultException = exception;
            }

            if(faultException != null)
            {
                var exceptionContext = new ExceptionBehaviorContextProxy<TInstance, Exception>(context, faultException);

                await _next.Compensate(exceptionContext);
            }
        }

        Task Behavior<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context)
        {
            var behavior = new SplitBehavior<TInstance, T>(_next);

            return _activity.Execute(context, behavior);
        }

        Task Behavior<TInstance>.Compensate<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context)
        {
            var behavior = new SplitBehavior<TInstance, T>(_next);

            return _activity.Compensate(context, behavior);
        }

        Task Behavior<TInstance>.Compensate<TException>(BehaviorExceptionContext<TInstance, TException> context)
        {
            return _activity.Compensate(context, _next);
        }
    }
}