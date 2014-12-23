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
    using Events;


    public class ExceptionHandlerActivity<TInstance, TException> :
        ExceptionActivity<TInstance>
        where TInstance : class
        where TException : Exception
    {
        readonly Behavior<TInstance, TException> _behavior;
        readonly Event<TException> _event;
        readonly Type _exceptionType;

        public ExceptionHandlerActivity(IEnumerable<EventActivity<TInstance, TException>> activities, Type exceptionType,
            Event<TException> @event)
        {
            _exceptionType = exceptionType;
            _event = @event;
            _behavior = CreateBehavior(activities.Cast<Activity<TInstance, TException>>().ToArray());
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

        BehaviorContext<TInstance, Exception> ExceptionActivity<TInstance>.GetExceptionContext(BehaviorContext<TInstance> context,
            Exception exception)
        {
            return context.GetProxy(_event, exception as TException);
        }

        BehaviorContext<TInstance, Tuple<TData, Exception>> ExceptionActivity<TInstance>.GetExceptionContext<TData>(
            BehaviorContext<TInstance, TData> context, Exception exception)
        {
            var @event = new DataEvent<Tuple<TData, Exception>>(typeof(TData).Name + "." + typeof(TException).Name);

            return context.GetProxy(@event, Tuple.Create(context.Data, exception));
        }

        public async Task Execute(BehaviorContext<TInstance, Exception> context, Behavior<TInstance, Exception> next)
        {
            var contextProxy = context.GetProxy(_event, context.Data as TException);

            await _behavior.Execute(contextProxy);

            await next.Execute(context);
        }

        Behavior<TInstance, TException> CreateBehavior(Activity<TInstance, TException>[] activities)
        {
            if (activities.Length == 0)
                return Behavior.Empty<TInstance, TException>();

            Behavior<TInstance, TException> current = new LastBehavior<TInstance, TException>(activities[activities.Length - 1]);

            for (int i = activities.Length - 2; i >= 0; i--)
                current = new ActivityBehavior<TInstance,TException>(activities[i], current);

            return current;
        }
    }


    public class ExceptionHandlerActivity<TInstance, TData, TException> :
        ExceptionActivity<TInstance, TData>
        where TInstance : class
        where TException : Exception
    {
        readonly Behavior<TInstance, Tuple<TData,TException>> _behavior;
        readonly Event<Tuple<TData, Exception>> _event;
        readonly Event<Tuple<TData, TException>> _typedEvent;
        readonly Type _exceptionType;

        public ExceptionHandlerActivity(IEnumerable<EventActivity<TInstance, Tuple<TData,TException>>> activities, Type exceptionType)
        {
            _exceptionType = exceptionType;
            _event = new DataEvent<Tuple<TData, Exception>>(typeof(TData).Name + "." + typeof(TException).Name);
            _typedEvent = new DataEvent<Tuple<TData, TException>>(typeof(TData).Name + "." + typeof(TException).Name);
            _behavior = CreateBehavior(activities.Cast<Activity<TInstance, Tuple<TData,TException>>>().ToArray());
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

        public async Task Execute(BehaviorContext<TInstance, Tuple<TData, Exception>> context, Behavior<TInstance, Tuple<TData, Exception>> next)
        {
            var behaviorContext = context.GetProxy(_typedEvent, Tuple.Create(context.Data.Item1, context.Data.Item2 as TException));

            await _behavior.Execute(behaviorContext);

            await next.Execute(context);
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, _ => _behavior.Accept(inspector));
        }

        public Type ExceptionType
        {
            get { return _exceptionType; }
        }

        Behavior<TInstance, Tuple<TData, TException>> CreateBehavior(Activity<TInstance, Tuple<TData,TException>>[] activities)
        {
            if (activities.Length == 0)
                return Behavior.Empty<TInstance, Tuple<TData, TException>>();

            Behavior<TInstance, Tuple<TData, TException>> current = new LastBehavior<TInstance, Tuple<TData,TException>>(activities[activities.Length - 1]);

            for (int i = activities.Length - 2; i >= 0; i--)
                current = new ActivityBehavior<TInstance, Tuple<TData,TException>>(activities[i], current);

            return current;
        }
    }
}