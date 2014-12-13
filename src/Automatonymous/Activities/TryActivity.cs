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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals.Caching;


    public class TryActivity<TInstance> :
        Activity<TInstance>
    {
        readonly List<Activity<TInstance>> _activities;
        readonly Cache<Type, List<ExceptionActivity<TInstance>>> _exceptionHandlers;

        public TryActivity(Event @event, IEnumerable<EventActivity<TInstance>> activities,
            IEnumerable<ExceptionActivity<TInstance>> exceptionBinder)
        {
            _activities = new List<Activity<TInstance>>(activities
                .Select(x => new EventActivityShim<TInstance>(@event, x)));

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

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return Execute(instance, cancellationToken);
        }

        Task Activity<TInstance>.Execute<T>(TInstance instance, T value, CancellationToken cancellationToken)
        {
            return Execute(instance, cancellationToken);
        }

        async Task Execute(TInstance instance, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var activity in _activities)
                    await activity.Execute(instance, cancellationToken);
            }
            catch (Exception ex)
            {
                Type exceptionType = ex.GetType();
                while (exceptionType != typeof(Exception).GetTypeInfo().BaseType && exceptionType != null)
                {
                    List<ExceptionActivity<TInstance>> handlers;
                    if (_exceptionHandlers.TryGetValue(exceptionType, out handlers))
                    {
                        foreach (var handler in handlers)
                            handler.Execute(instance, ex, cancellationToken).Wait(cancellationToken);

                        return;
                    }
                    exceptionType = exceptionType.GetTypeInfo().BaseType;
                }

                throw;
            }
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
                .Select(x => new EventActivityShim<TInstance>(@event, x)));

            _exceptionHandlers = new DictionaryCache<Type, List<ExceptionActivity<TInstance>>>(
                x => new List<ExceptionActivity<TInstance>>());

            foreach (var exceptionActivity in exceptionBinder)
                _exceptionHandlers[exceptionActivity.ExceptionType].Add(exceptionActivity);
        }


        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, _ => { _activities.ForEach(activity => activity.Accept(inspector)); });
        }

        async Task Activity<TInstance, TData>.Execute(TInstance instance, TData value, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var activity in _activities)
                    await activity.Execute(instance, value, cancellationToken);
            }
            catch (Exception ex)
            {
                Type exceptionType = ex.GetType();
                while (exceptionType != typeof(Exception).GetTypeInfo().BaseType && exceptionType != null)
                {
                    List<ExceptionActivity<TInstance>> handlers;
                    if (_exceptionHandlers.TryGetValue(exceptionType, out handlers))
                    {
                        Type tupleType = typeof(Tuple<,>).MakeGenericType(typeof(TData), exceptionType);
                        object exceptionData = Activator.CreateInstance(tupleType, value, ex);

                        foreach (var handler in handlers)
                            handler.Execute(instance, exceptionData, cancellationToken).Wait(cancellationToken);

                        return;
                    }
                    exceptionType = exceptionType.GetTypeInfo().BaseType;
                }

                throw;
            }
        }
    }
}