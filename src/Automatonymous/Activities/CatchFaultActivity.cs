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


    /// <summary>
    /// Catches an exception of a specific type and compenstates using the behavior
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    public class CatchFaultActivity<TInstance, TException> :
        Activity<TInstance>
        where TInstance : class
        where TException : Exception
    {
        readonly Behavior<TInstance> _behavior;

        public CatchFaultActivity(Behavior<TInstance> behavior)
        {
            _behavior = behavior;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x => _behavior.Accept(visitor));
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance>.Faulted<T>(BehaviorExceptionContext<TInstance, T> context, Behavior<TInstance> next)
        {
            var exceptionContext = context as BehaviorExceptionContext<TInstance, TException>;
            if (exceptionContext != null)
            {
                await _behavior.Faulted(exceptionContext);

                // if the compensate returns, we should go forward normally
                await next.Execute(context);
            }
            else
                await next.Faulted(context);
        }

        async Task Activity<TInstance>.Faulted<TData, T>(BehaviorExceptionContext<TInstance, TData, T> context,
            Behavior<TInstance, TData> next)
        {
            var exceptionContext = context as BehaviorExceptionContext<TInstance, TData, TException>;
            if (exceptionContext != null)
            {
                await _behavior.Faulted(exceptionContext);

                // if the compensate returns, we should go forward normally
                await next.Execute(context);
            }
            else
                await next.Faulted(context);
        }
    }
}