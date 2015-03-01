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


    public class ExecuteOnCompensateActivity<TInstance> :
        Activity<TInstance>
    {
        readonly Activity<TInstance> _activity;

        public ExecuteOnCompensateActivity(Activity<TInstance> activity)
        {
            _activity = activity;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            _activity.Accept(visitor);
        }

        public Task Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return next.Execute(context);
        }

        public Task Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            return next.Execute(context);
        }

        public Task Compensate<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
            where TException : Exception
        {
            var nextBehavior = new ExceptionBehavior<TInstance, TException>(next, context);

            return _activity.Execute(context, nextBehavior);
        }

        public Task Compensate<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next)
            where TException : Exception
        {
            var nextBehavior = new ExceptionBehavior<TInstance, T, TException>(next, context);

            return _activity.Execute(context, nextBehavior);
        }
    }
}