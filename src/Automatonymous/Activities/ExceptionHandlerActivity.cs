// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    public class ExceptionHandlerActivity<TInstance, TException> :
        ExceptionActivity<TInstance>
        where TInstance : class
        where TException : Exception
    {
        readonly Behavior<TInstance> _behavior;
        readonly Event<TException> _event;
        readonly Type _exceptionType;

        public ExceptionHandlerActivity(IEnumerable<EventActivity<TInstance>> activities, Type exceptionType,
            Event<TException> @event)
        {
            _exceptionType = exceptionType;
            _event = @event;
            _behavior = CreateBehavior(activities.Cast<Activity<TInstance>>().ToArray());
        }

        public Event Event
        {
            get { return _event; }
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, _ => _behavior.Accept(inspector));
        }

        public Type ExceptionType
        {
            get { return _exceptionType; }
        }

        public BehaviorContext<TInstance> GetExceptionContext(BehaviorContext<TInstance> context, Exception exception)
        {
            return context.GetProxy(_event, exception as TException);
        }

        public BehaviorContext<TInstance> GetExceptionContext<TData>(BehaviorContext<TInstance, TData> context, Exception exception)
        {
            return context.GetProxy(_event, exception as TException);
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            await _behavior.Execute(context);

            await next.Execute(context);
        }

        async Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            await _behavior.Execute(context);

            await next.Execute(context);
        }

        Behavior<TInstance> CreateBehavior(Activity<TInstance>[] activities)
        {
            if (activities.Length == 0)
                return Behavior.Empty<TInstance>();

            Behavior<TInstance> current = new LastBehavior<TInstance>(activities[activities.Length - 1]);

            for (int i = activities.Length - 2; i >= 0; i--)
                current = new ActivityBehavior<TInstance>(activities[i], current);

            return current;
        }
    }


    public class ExceptionHandlerActivity<TInstance, TData, TException> :
        ExceptionActivity<TInstance>
        where TInstance : class
        where TException : Exception
    {
        readonly Behavior<TInstance> _behavior;
        readonly Event<Tuple<TData, TException>> _event;
        readonly Type _exceptionType;

        public ExceptionHandlerActivity(IEnumerable<EventActivity<TInstance>> activities, Type exceptionType,
            Event<Tuple<TData, TException>> @event)
        {
            _exceptionType = exceptionType;
            _event = @event;
            _behavior = CreateBehavior(activities.Cast<Activity<TInstance>>().ToArray());
        }

        public Event Event
        {
            get { return _event; }
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, _ => _behavior.Accept(inspector));
        }

        public Type ExceptionType
        {
            get { return _exceptionType; }
        }

        public BehaviorContext<TInstance> GetExceptionContext(BehaviorContext<TInstance> context, Exception exception)
        {
            return context.GetProxy(_event, Tuple.Create(default(TData), exception as TException));
        }

        public BehaviorContext<TInstance> GetExceptionContext<T>(BehaviorContext<TInstance, T> context, Exception exception)
        {
            var self = this as ExceptionHandlerActivity<TInstance, T, TException>;
            if (self == null)
                throw new AutomatonymousException("Well that was unexpected.");

            return context.GetProxy(self._event, Tuple.Create(context.Data, exception as TException));
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            await _behavior.Execute(context);

            await next.Execute(context);
        }

        async Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            await _behavior.Execute(context);

            await next.Execute(context);
        }

        Behavior<TInstance> CreateBehavior(Activity<TInstance>[] activities)
        {
            if (activities.Length == 0)
                return Behavior.Empty<TInstance>();

            Behavior<TInstance> current = new LastBehavior<TInstance>(activities[activities.Length - 1]);

            for (int i = activities.Length - 2; i >= 0; i--)
                current = new ActivityBehavior<TInstance>(activities[i], current);

            return current;
        }
    }
}