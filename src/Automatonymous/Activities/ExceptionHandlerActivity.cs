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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Behaviors;
    using Binders;
    using Events;


    public class ExceptionHandlerActivity<TInstance, TException> :
        ExceptionActivity<TInstance>
        where TInstance : class
        where TException : Exception
    {
        readonly Behavior<TInstance> _behavior;
        readonly Event<TException> _event;
        readonly Type _exceptionType;

        public ExceptionHandlerActivity(IEnumerable<StateActivityBinder<TInstance>> activities, Type exceptionType,
            Event<TException> @event)
        {
            _exceptionType = exceptionType;
            _event = @event;
            _behavior = CreateBehavior(activities.ToArray());
        }

        public Event Event
        {
            get { return _event; }
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, _ => _behavior.Accept(visitor));
        }

        public Type ExceptionType
        {
            get { return _exceptionType; }
        }

        BehaviorContext<TInstance, Exception> ExceptionActivity<TInstance>.GetExceptionContext(BehaviorContext<TInstance> context,
            Exception exception)
        {
            return context.GetProxy(_event, exception as TException);
        }

        public async Task Execute(BehaviorContext<TInstance, Exception> context, Behavior<TInstance, Exception> next)
        {
            BehaviorContext<TInstance, TException> contextProxy = context.GetProxy(_event, context.Data as TException);

            await _behavior.Execute(contextProxy);

            await next.Execute(context);
        }

        Behavior<TInstance> CreateBehavior(StateActivityBinder<TInstance>[] activities)
        {
            if (activities.Length == 0)
                return Behavior.Empty<TInstance>();

            var builder = new ActivityBehaviorBuilder<TInstance>();
            foreach (var activity in activities)
                activity.Bind(builder);

            return builder.Behavior;
        }
    }


    public class ExceptionHandlerActivity<TInstance, TData, TException> :
        ExceptionActivity<TInstance, TData>
        where TInstance : class
        where TException : Exception
    {
        readonly Behavior<TInstance> _behavior;
        readonly Event<Tuple<TData, Exception>> _event;
        readonly Type _exceptionType;
        readonly Event<Tuple<TData, TException>> _typedEvent;

        public ExceptionHandlerActivity(IEnumerable<StateActivityBinder<TInstance>> activities, Type exceptionType)
        {
            _exceptionType = exceptionType;
            _event = new DataEvent<Tuple<TData, Exception>>(typeof(TData).Name + "." + typeof(TException).Name);
            _typedEvent = new DataEvent<Tuple<TData, TException>>(typeof(TData).Name + "." + typeof(TException).Name);
            _behavior = CreateBehavior(activities.ToArray());
        }

        public Event<Tuple<TData, Exception>> Event
        {
            get { return _event; }
        }

        public BehaviorContext<TInstance, Tuple<TData, Exception>> GetExceptionContext(BehaviorContext<TInstance, TData> context,
            Exception exception)
        {
            return context.GetProxy(_event, Tuple.Create(context.Data, exception));
        }

        public async Task Execute(BehaviorContext<TInstance, Tuple<TData, Exception>> context,
            Behavior<TInstance, Tuple<TData, Exception>> next)
        {
            BehaviorContext<TInstance, Tuple<TData, TException>> behaviorContext = context.GetProxy(_typedEvent,
                Tuple.Create(context.Data.Item1, context.Data.Item2 as TException));

            await _behavior.Execute(behaviorContext);

            await next.Execute(context);
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, _ => _behavior.Accept(visitor));
        }

        public Type ExceptionType
        {
            get { return _exceptionType; }
        }

        Behavior<TInstance> CreateBehavior(StateActivityBinder<TInstance>[] activities)
        {
            if (activities.Length == 0)
                return Behavior.Empty<TInstance>();

            var builder = new ActivityBehaviorBuilder<TInstance>();
            foreach (var activity in activities)
                activity.Bind(builder);

            return builder.Behavior;
        }
    }
}