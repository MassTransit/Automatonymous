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


    public class CompensateActionActivity<TInstance, TException> :
        Activity<TInstance>
        where TException : Exception
    {
        readonly Action<BehaviorExceptionContext<TInstance, TException>> _action;

        public CompensateActionActivity(Action<BehaviorExceptionContext<TInstance, TException>> action)
        {
            _action = action;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance>.Execute<TData>(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance>.Compensate<T>(BehaviorExceptionContext<TInstance, T> context, Behavior<TInstance> next)
        {
            var exceptionContext = context as BehaviorExceptionContext<TInstance, TException>;
            if (exceptionContext != null)
                _action(exceptionContext);

            await next.Compensate(context);
        }

        async Task Activity<TInstance>.Compensate<TData, T>(BehaviorExceptionContext<TInstance, TData, T> context,
            Behavior<TInstance, TData> next)
        {
            var exceptionContext = context as BehaviorExceptionContext<TInstance, TData, TException>;
            if (exceptionContext != null)
                _action(exceptionContext);

            await next.Compensate(context);
        }
    }


    public class CompensateActionActivity<TInstance, TData, TException> :
        Activity<TInstance, TData>
        where TInstance : class
        where TException : Exception
    {
        readonly Action<BehaviorExceptionContext<TInstance, TData, TException>> _action;

        public CompensateActionActivity(Action<BehaviorExceptionContext<TInstance, TData, TException>> action)
        {
            _action = action;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance, TData>.Compensate<T>(BehaviorExceptionContext<TInstance, TData, T> context,
            Behavior<TInstance, TData> next)
        {
            var exceptionContext = context as BehaviorExceptionContext<TInstance, TData, TException>;
            if (exceptionContext != null)
                _action(exceptionContext);

            await next.Compensate(context);
        }
    }
}