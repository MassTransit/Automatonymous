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


//    public class TryActivity<TInstance>
//        where TInstance : class
//    {
//        readonly Behavior<TInstance> _behavior;
//        readonly Dictionary<Type, List<ExceptionActivity<TInstance>>> _exceptionHandlers;
//
//        public TryActivity(EventActivities<TInstance> activities, IEnumerable<ExceptionActivity<TInstance>> exceptionHandlers)
//        {
////            _behavior = CreateBehavior(activities.GetStateActivityBinders().Select(x => x.Activity).ToArray());
//
//
//            _exceptionHandlers = new Dictionary<Type, List<ExceptionActivity<TInstance>>>(
//                exceptionHandlers.GroupBy(x => x.ExceptionType).ToDictionary(x => x.Key, x => x.ToList()));
//        }
//
////        public void Accept(StateMachineVisitor visitor)
////        {
////            visitor.Visit(this, _ =>
////            {
////                _behavior.Accept(visitor);
////
////                foreach (var handler in _exceptionHandlers.Values)
////                    handler.ForEach(x => x.Accept(visitor));
////            });
////        }
//
//        async Task Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
//        {
//            await Execute(context, () => _behavior.Execute(context));
//
//            await next.Execute(context);
//        }
//
//        async Task Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
//        {
//            await Execute(context, () => _behavior.Execute(context));
//
//            await next.Execute(context);
//        }
//
//        Behavior<TInstance> CreateBehavior(Activity<TInstance>[] activities)
//        {
//            if (activities.Length == 0)
//                return Behavior.Empty<TInstance>();
//
//            Behavior<TInstance> current = new LastBehavior<TInstance>(activities[activities.Length - 1]);
//
//            for (int i = activities.Length - 2; i >= 0; i--)
//                current = new ActivityBehavior<TInstance>(activities[i], current);
//
//            return current;
//        }
//
//        async Task Execute(BehaviorContext<TInstance> context, Func<Task> invokeBehavior)
//        {
//            Exception exception = null;
//
//            try
//            {
//                await invokeBehavior();
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//            }
//
//            if (exception != null)
//            {
//                Type exceptionType = exception.GetType();
//                while (exceptionType != typeof(Exception).BaseType && exceptionType != null)
//                {
//                    List<ExceptionActivity<TInstance>> handlers;
//                    if (_exceptionHandlers.TryGetValue(exceptionType, out handlers))
//                    {
//                        foreach (var handler in handlers)
//                        {
//                            BehaviorContext<TInstance, Exception> contextProxy = handler.GetExceptionContext(context, exception);
//
//                            Behavior<TInstance, Exception> behavior = new LastBehavior<TInstance, Exception>(handler);
//
//                            await behavior.Execute(contextProxy);
//                        }
//
//                        return;
//                    }
//                    exceptionType = exceptionType.BaseType;
//                }
//
//                throw new AutomatonymousException("The activity threw an exception", exception);
//            }
//        }
//    }
//
//
//    public class TryActivity<TInstance, TData> 
//        where TInstance : class
//    {
//        readonly Behavior<TInstance> _behavior;
//        readonly Dictionary<Type, List<ExceptionActivity<TInstance, TData>>> _exceptionHandlers;
//
//        public TryActivity(Event @event, EventActivities<TInstance> activities,
//            IEnumerable<ExceptionActivity<TInstance, TData>> exceptionBinder)
//        {
////            _behavior = CreateBehavior(activities.GetStateActivityBinders().Select(x => x.Activity).ToArray());
//
//            _exceptionHandlers = new Dictionary<Type, List<ExceptionActivity<TInstance, TData>>>(
//                exceptionBinder.GroupBy(x => x.ExceptionType).ToDictionary(x => x.Key, x => x.ToList()));
//        }
//
//        public void Accept(StateMachineVisitor visitor)
//        {
//            _behavior.Accept(visitor);
//
//            foreach (var handler in _exceptionHandlers.Values)
//                handler.ForEach(x => x.Accept(visitor));
//        }
//
//        async Task Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
//        {
//            Exception exception = null;
//            try
//            {
//                await _behavior.Execute(context);
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//            }
//
//            if (exception != null)
//            {
//                Type exceptionType = exception.GetType();
//                while (exceptionType != typeof(Exception).BaseType && exceptionType != null)
//                {
//                    List<ExceptionActivity<TInstance, TData>> handlers;
//                    if (_exceptionHandlers.TryGetValue(exceptionType, out handlers))
//                    {
//                        foreach (var handler in handlers)
//                        {
//                            BehaviorContext<TInstance, Tuple<TData, Exception>> contextProxy = handler.GetExceptionContext(context,
//                                exception);
//
//                            Behavior<TInstance,Tuple<TData,Exception>> behavior = new LastBehavior<TInstance, Tuple<TData, Exception>>(handler);
//
//                            await behavior.Execute(contextProxy);
//                        }
//
//                        return;
//                    }
//                    exceptionType = exceptionType.BaseType;
//                }
//
//                throw new AutomatonymousException("The activity threw an exception", exception);
//            }
//        }
//
//        Behavior<TInstance> CreateBehavior(Activity<TInstance>[] activities)
//        {
//            if (activities.Length == 0)
//                return Behavior.Empty<TInstance>();
//
//            Behavior<TInstance> current = new LastBehavior<TInstance>(activities[activities.Length - 1]);
//
//            for (int i = activities.Length - 2; i >= 0; i--)
//                current = new ActivityBehavior<TInstance>(activities[i], current);
//
//            return current;
//        }
//
//        Task Compensate<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next) where TException : Exception
//        {
//            return next.Compensate(context);
//        }
//    }
}