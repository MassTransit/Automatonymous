// Copyright 2011-2016 Chris Patterson, Dru Sellers
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
    using System.Threading.Tasks;
    using GreenPipes.Util;


    public class EmptyBehavior<TInstance> :
        Behavior<TInstance>
    {
        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        Task Behavior<TInstance>.Execute(BehaviorContext<TInstance> context)
        {
            return TaskUtil.Completed;
        }

        Task Behavior<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context)
        {
            return TaskUtil.Completed;
        }

        Task Behavior<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context)
        {
            return TaskUtil.Completed;
        }

        Task Behavior<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context)
        {
            return TaskUtil.Completed;
        }
    }


    public class EmptyBehavior<TInstance, TData> :
        Behavior<TInstance, TData>
    {
        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        Task Behavior<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context)
        {
            return TaskUtil.Completed;
        }

        Task Behavior<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context)
        {
            return TaskUtil.Completed;
        }
    }
}