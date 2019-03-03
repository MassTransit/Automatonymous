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
namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class ConditionExceptionActivity<TInstance, TConditionException> :
        Activity<TInstance>
        where TInstance : class
        where TConditionException : Exception
    {
        readonly Behavior<TInstance> _behavior;
        readonly StateMachineExceptionCondition<TInstance, TConditionException> _condition;

        public ConditionExceptionActivity(StateMachineExceptionCondition<TInstance, TConditionException> condition, Behavior<TInstance> behavior)
        {
            _condition = condition;
            _behavior = behavior;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("condition");

            _behavior.Probe(scope);
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

        async Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            var behaviorContext = context as BehaviorExceptionContext<TInstance, TConditionException>;
            if (behaviorContext != null)
            {
                if (_condition(behaviorContext))
                {
                    await _behavior.Faulted(context).ConfigureAwait(false);
                }
            }

            await next.Faulted(context).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context,
            Behavior<TInstance, T> next)
        {
            var behaviorContext = context as BehaviorExceptionContext<TInstance, T, TConditionException>;
            if (behaviorContext != null)
            {
                if (_condition(behaviorContext))
                {
                    await _behavior.Faulted(context).ConfigureAwait(false);
                }
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }


    public class ConditionExceptionActivity<TInstance, TData, TConditionException> :
        Activity<TInstance>
        where TInstance : class
        where TConditionException : Exception
    {
        readonly Behavior<TInstance> _behavior;
        readonly StateMachineExceptionCondition<TInstance, TData, TConditionException> _condition;

        public ConditionExceptionActivity(StateMachineExceptionCondition<TInstance, TData, TConditionException> condition, Behavior<TInstance> behavior)
        {
            _condition = condition;
            _behavior = behavior;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("condition");

            _behavior.Probe(scope);
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x => _behavior.Accept(visitor));
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            throw new AutomatonymousException("This activity requires a body with the event, but no body was specified.");
        }

        Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            throw new AutomatonymousException("This activity requires a body with the event, but no body was specified.");
        }

        async Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context,
            Behavior<TInstance, T> next)
        {
            var behaviorContext = context as BehaviorExceptionContext<TInstance, TData, TConditionException>;
            if (behaviorContext != null)
            {
                if (_condition(behaviorContext))
                {
                    await _behavior.Faulted(context).ConfigureAwait(false);
                }
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }
}