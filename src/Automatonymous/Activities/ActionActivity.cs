// Copyright 2011-2014 Chris Patterson, Dru Sellers
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


    public class ActionActivity<TInstance> :
        Activity<TInstance>
    {
        readonly Action<BehaviorContext<TInstance>> _action;

        public ActionActivity(Action<BehaviorContext<TInstance>> action)
        {
            _action = action;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            _action(context);

            await next.Execute(context);
        }

        async Task Activity<TInstance>.Execute<TData>(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            _action(context);

            await next.Execute(context);
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            return next.Faulted(context);
        }

        Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next)
        {
            return next.Faulted(context);
        }
    }


    public class ActionActivity<TInstance, TData> :
        Activity<TInstance, TData>
    {
        readonly Action<BehaviorContext<TInstance, TData>> _action;

        public ActionActivity(Action<BehaviorContext<TInstance, TData>> action)
        {
            _action = action;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            _action(context);

            await next.Execute(context);
        }

        Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
        {
            return next.Faulted(context);
        }
    }
}