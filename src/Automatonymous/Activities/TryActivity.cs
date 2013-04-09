// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
    using Internals.Caching;
    using Taskell;


    public class TryActivity<TInstance> :
        Activity<TInstance>
    {
        readonly List<Activity<TInstance>> _activities;
        readonly Cache<Type, List<ExceptionActivity<TInstance>>> _exceptionHandlers;

        public TryActivity(Event @event, IEnumerable<EventActivity<TInstance>> activities,
            IEnumerable<ExceptionActivity<TInstance>> exceptionBinder)
        {
            _activities = new List<Activity<TInstance>>(activities
                .Select(x => new EventActivityImpl<TInstance>(@event, x)));

            _exceptionHandlers =
                new DictionaryCache<Type, List<ExceptionActivity<TInstance>>>(
                    x => new List<ExceptionActivity<TInstance>>());

            foreach (var exceptionActivity in exceptionBinder)
                _exceptionHandlers[exceptionActivity.ExceptionType].Add(exceptionActivity);
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, _ =>
                {
                    _activities.ForEach(activity =>
                        {
                            activity.Accept(inspector);

                            _exceptionHandlers.Each((type, handler) => handler.ForEach(x => x.Accept(inspector)));
                        });
                });
        }

        void Activity<TInstance>.Execute(Composer composer, TInstance instance)
        {
            composer.Execute(() =>
                {
                    var taskComposer = new TaskComposer<TInstance>(composer.CancellationToken);

                    _activities.ForEach(activity => activity.Execute(taskComposer, instance));

                    ((Composer)taskComposer).Compensate(compensation =>
                        {
                            Type exceptionType = compensation.Exception.GetType();
#if !NETFX_CORE
                            while (exceptionType != typeof(Exception).BaseType && exceptionType != null)
#else
                            while (exceptionType != typeof(Exception).GetTypeInfo().BaseType && exceptionType != null)
#endif
                            {
                                List<ExceptionActivity<TInstance>> handlers;
                                if (_exceptionHandlers.TryGetValue(exceptionType, out handlers))
                                {
                                    var exceptionComposer = new TaskComposer<TInstance>(composer.CancellationToken);

                                    handlers.ForEach(handler => handler.Execute(exceptionComposer, instance, compensation.Exception));

                                    return compensation.Task(exceptionComposer.Finish());
                                }
#if !NETFX_CORE
                                exceptionType = exceptionType.BaseType;
#else
                                exceptionType = exceptionType.GetTypeInfo().BaseType;
#endif
                            }

                            return compensation.Throw();
                        });
                });
        }

        void Activity<TInstance>.Execute<T>(Composer composer, TInstance instance, T value)
        {
            composer.Execute(() =>
                {
                    var taskComposer = new TaskComposer<TInstance>(composer.CancellationToken);

                    _activities.ForEach(activity => activity.Execute(taskComposer, instance, value));

                    ((Composer)taskComposer).Compensate(compensation =>
                        {
                            Type exceptionType = compensation.Exception.GetType();
#if !NETFX_CORE
                            while (exceptionType != typeof(Exception).BaseType && exceptionType != null)
#else
                            while (exceptionType != typeof(Exception).GetTypeInfo().BaseType && exceptionType != null)
#endif
                            {
                                List<ExceptionActivity<TInstance>> handlers;
                                if (_exceptionHandlers.TryGetValue(exceptionType, out handlers))
                                {
                                    var exceptionComposer = new TaskComposer<TInstance>(composer.CancellationToken);

                                    handlers.ForEach(handler => handler.Execute(exceptionComposer, instance, compensation.Exception));

                                    return compensation.Task(exceptionComposer.Finish());
                                }
#if !NETFX_CORE
                                exceptionType = exceptionType.BaseType;
#else
                                exceptionType = exceptionType.GetTypeInfo().BaseType;
#endif
                            }

                            return compensation.Throw();
                        });
                });
        }
    }


    public class TryActivity<TInstance, TData> :
        Activity<TInstance, TData>
    {
        readonly List<Activity<TInstance>> _activities;
        readonly Cache<Type, List<ExceptionActivity<TInstance>>> _exceptionHandlers;

        public TryActivity(Event @event, IEnumerable<EventActivity<TInstance>> activities,
            IEnumerable<ExceptionActivity<TInstance>> exceptionBinder)
        {
            _activities = new List<Activity<TInstance>>(activities
                .Select(x => new EventActivityImpl<TInstance>(@event, x)));

            _exceptionHandlers = new DictionaryCache<Type, List<ExceptionActivity<TInstance>>>(
                x => new List<ExceptionActivity<TInstance>>());

            foreach (var exceptionActivity in exceptionBinder)
                _exceptionHandlers[exceptionActivity.ExceptionType].Add(exceptionActivity);
        }


        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, _ => { _activities.ForEach(activity => activity.Accept(inspector)); });
        }

        void Activity<TInstance, TData>.Execute(Composer composer, TInstance instance, TData value)
        {
            composer.Execute(() =>
                {
                    var taskComposer = new TaskComposer<TInstance>(composer.CancellationToken);

                    _activities.ForEach(activity => activity.Execute(taskComposer, instance, value));

                    ((Composer)taskComposer).Compensate(compensation =>
                        {
                            Type exceptionType = compensation.Exception.GetType();
#if !NETFX_CORE
                            while (exceptionType != typeof(Exception).BaseType && exceptionType != null)
#else
                            while (exceptionType != typeof(Exception).GetTypeInfo().BaseType && exceptionType != null)
#endif
                            {
                                List<ExceptionActivity<TInstance>> handlers;
                                if (_exceptionHandlers.TryGetValue(exceptionType, out handlers))
                                {
                                    var exceptionComposer = new TaskComposer<TInstance>(composer.CancellationToken);

                                    Type tupleType = typeof(Tuple<,>).MakeGenericType(typeof(TData), exceptionType);
                                    object exceptionData = Activator.CreateInstance(tupleType, value, compensation.Exception);

                                    handlers.ForEach(handler => handler.Execute(exceptionComposer, instance, exceptionData));

                                    return compensation.Task(exceptionComposer.Finish());
                                }
#if !NETFX_CORE
                                exceptionType = exceptionType.BaseType;
#else
                                exceptionType = exceptionType.GetTypeInfo().BaseType;
#endif
                            }

                            return compensation.Throw();
                        });
                });
        }
    }
}