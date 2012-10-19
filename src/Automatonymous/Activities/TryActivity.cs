// Copyright 2011 Chris Patterson, Dru Sellers
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
    using System.Reflection;


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

        public void Execute(TInstance instance)
        {
            try
            {
                _activities.ForEach(activity => activity.Execute(instance));
            }
            catch (Exception ex)
            {
                Type exceptionType = ex.GetType();
#if !NETFX_CORE
                while (exceptionType != typeof(Exception).BaseType && exceptionType != null)
#else
                while (exceptionType != typeof(Exception).GetTypeInfo().BaseType && exceptionType != null)
#endif
                {
                    if (_exceptionHandlers.WithValue(exceptionType,
                        x => x.ForEach(activity => activity.Execute(instance, ex))))
                        return;

#if !NETFX_CORE
                    exceptionType = exceptionType.BaseType;
#else
                    exceptionType = exceptionType.GetTypeInfo().BaseType;
#endif
                }

                throw;
            }
        }

        public void Execute<TData>(TInstance instance, TData value)
        {
            try
            {
                _activities.ForEach(activity => activity.Execute(instance, value));
            }
            catch (Exception ex)
            {
                Type exceptionType = ex.GetType();
#if !NETFX_CORE
                while (exceptionType != typeof(Exception).BaseType && exceptionType != null)
#else
                while (exceptionType != typeof(Exception).GetTypeInfo().BaseType && exceptionType != null)
#endif
                {
                    if (_exceptionHandlers.WithValue(exceptionType,
                        x => x.ForEach(activity => activity.Execute(instance, ex))))
                        return;

#if !NETFX_CORE
                    exceptionType = exceptionType.BaseType;
#else
                    exceptionType = exceptionType.GetTypeInfo().BaseType;
#endif
                }

                throw;
            }
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
    }


    public class TryActivity<TInstance, TData> :
        Activity<TInstance, TData>
    {
        readonly List<Activity<TInstance>> _activities;
        readonly Cache<Type, List<Activity<TInstance>>> _exceptionHandlers;

        public TryActivity(Event @event, IEnumerable<EventActivity<TInstance>> activities,
                           IEnumerable<ExceptionActivity<TInstance>> exceptionBinder)
        {
            _activities = new List<Activity<TInstance>>(activities
                .Select(x => new EventActivityImpl<TInstance>(@event, x)));

            _exceptionHandlers =
                new DictionaryCache<Type, List<Activity<TInstance>>>(x => new List<Activity<TInstance>>());

            foreach (var exceptionActivity in exceptionBinder)
                _exceptionHandlers[exceptionActivity.ExceptionType].Add(exceptionActivity);
        }

        public void Execute(TInstance instance, TData data)
        {
            try
            {
                _activities.ForEach(activity => activity.Execute(instance, data));
            }
            catch (Exception ex)
            {
                Type exceptionType = ex.GetType();
#if !NETFX_CORE
                while (exceptionType != typeof(Exception).BaseType && exceptionType != null)
#else
                while (exceptionType != typeof(Exception).GetTypeInfo().BaseType && exceptionType != null)
#endif
                {
                    if (_exceptionHandlers.WithValue(exceptionType, x =>
                        {
                            Type tupleType = typeof(Tuple<,>).MakeGenericType(typeof(TData), exceptionType);
                            object arg = Activator.CreateInstance(tupleType, data, ex);

                            x.ForEach(activity => activity.Execute(instance, arg));
                        }))
                        return;

#if !NETFX_CORE
                    exceptionType = exceptionType.BaseType;
#else
                    exceptionType = exceptionType.GetTypeInfo().BaseType;
#endif
                }

                throw;
            }
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, _ => { _activities.ForEach(activity => activity.Accept(inspector)); });
        }
    }
}