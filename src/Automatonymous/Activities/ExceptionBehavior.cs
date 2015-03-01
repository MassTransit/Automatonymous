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


    public class ExceptionBehavior<TInstance, TException> :
        Behavior<TInstance>
        where TException : Exception
    {
        readonly BehaviorExceptionContext<TInstance, TException> _context;
        readonly Behavior<TInstance> _next;

        public ExceptionBehavior(Behavior<TInstance> next, BehaviorExceptionContext<TInstance, TException> context)
        {
            _next = next;
            _context = context;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            _next.Accept(visitor);
        }

        Task Behavior<TInstance>.Execute(BehaviorContext<TInstance> context)
        {
            return _next.Compensate(_context);
        }

        Task Behavior<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context)
        {
            return _next.Compensate(_context);
        }

        Task Behavior<TInstance>.Compensate<TData, T>(BehaviorExceptionContext<TInstance, TData, T> context)
        {
            throw new AutomatonymousException("This should not ever be called.");
        }

        Task Behavior<TInstance>.Compensate<T>(BehaviorExceptionContext<TInstance, T> context)
        {
            throw new AutomatonymousException("This should not ever be called.");
        }
    }


    public class ExceptionBehavior<TInstance, TData, TException> :
        Behavior<TInstance>
        where TException : Exception
    {
        readonly BehaviorExceptionContext<TInstance, TData, TException> _context;
        readonly Behavior<TInstance, TData> _next;

        public ExceptionBehavior(Behavior<TInstance, TData> next, BehaviorExceptionContext<TInstance, TData, TException> context)
        {
            _next = next;
            _context = context;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            _next.Accept(visitor);
        }

        Task Behavior<TInstance>.Execute(BehaviorContext<TInstance> context)
        {
            return _next.Compensate(_context);
        }

        Task Behavior<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context)
        {
            return _next.Compensate(_context);
        }

        Task Behavior<TInstance>.Compensate<TData, T>(BehaviorExceptionContext<TInstance, TData, T> context)
        {
            throw new AutomatonymousException("This should not ever be called.");
        }

        Task Behavior<TInstance>.Compensate<T>(BehaviorExceptionContext<TInstance, T> context)
        {
            throw new AutomatonymousException("This should not ever be called.");
        }
    }
}