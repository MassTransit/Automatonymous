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
    using System.Threading.Tasks;
    using Internals.Caching;
    using System.Reflection;


    static class TryActivity
    {
        /// <summary>
        /// Execute all found exception handlers' Execute method.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="exceptionType"></param>
        /// <param name="instance"></param>
        /// <param name="exceptionHandlers"></param>
        /// <param name="dataFactory">Factory for creating the argument to Execture</param>
        /// <returns>if there was *some* exception handler that was found</returns>
        public static async Task<bool> ExecuteOnException<TInstance, TData>(
            Exception ex, Type exceptionType, TInstance instance,
            Cache<Type, List<ExceptionActivity<TInstance>>> exceptionHandlers,
            Func<Exception, TData> dataFactory = null)
        {
            if (exceptionType == null)
                return false;

            if (exceptionType == typeof(Exception).GetTypeInfo().BaseType)
                return false;

            var typeInfo = exceptionType.GetTypeInfo();

            List<ExceptionActivity<TInstance>> foundHandlers;
            if (exceptionHandlers.TryGetValue(exceptionType, out foundHandlers))
            {
                foreach (var activity in foundHandlers)
                    if (dataFactory == null)
                        await activity.Execute(instance, ex);
                    else
                        await activity.Execute(instance, dataFactory(ex));

                return true;
            }

            return await ExecuteOnException(ex, typeInfo.BaseType, instance, exceptionHandlers, dataFactory);
        }
    }

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

        public async Task Execute(TInstance instance)
        {
            Exception caught;

            try
            {
                foreach (var activity in _activities)
                    await activity.Execute(instance);

                return;
            }
            catch (Exception ex)
            {
                caught = ex;
            }

            if (!await TryActivity.ExecuteOnException<TInstance, Exception>(caught, caught.GetType(), instance, _exceptionHandlers))
                throw caught;
        }

        public async Task Execute<TData>(TInstance instance, TData value)
        {
            Exception caught;

            try
            {
                foreach (var activity in _activities)
                    await activity.Execute(instance, value);

                return;
            }
            catch (Exception ex)
            {
                caught = ex;
            }

            if (!await TryActivity.ExecuteOnException<TInstance, Exception>(caught, caught.GetType(), instance, _exceptionHandlers))
                throw caught;
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

        public async Task Execute(TInstance instance, TData data)
        {
            Exception caught;

            try
            {
                foreach (var activity in _activities)
                    await activity.Execute(instance, data);

                return;
            }
            catch (Exception ex)
            {
                caught = ex;
            }

            var exceptionType = caught.GetType();
            if (!await TryActivity.ExecuteOnException(caught, exceptionType, instance, _exceptionHandlers,
                ex =>
                    {
                        var tupleType = typeof(Tuple<,>).MakeGenericType(typeof(TData), exceptionType);
                        return Activator.CreateInstance(tupleType, data, ex);
                    }))
                throw caught;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, _ => { _activities.ForEach(activity => activity.Accept(inspector)); });
        }
    }
}