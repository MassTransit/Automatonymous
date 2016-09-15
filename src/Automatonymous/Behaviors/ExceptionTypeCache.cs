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
namespace Automatonymous.Behaviors
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Contexts;


    public static class ExceptionTypeCache
    {
        static CachedConfigurator GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ =>
                (CachedConfigurator)Activator.CreateInstance(typeof(CachedConfigurator<>).MakeGenericType(type)));
        }

        public static Task Faulted<TInstance>(Behavior<TInstance> behavior, BehaviorContext<TInstance> context, Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return GetOrAdd(exception.GetType()).Faulted(behavior, context, exception);
        }

        public static Task Faulted<TInstance, TData>(Behavior<TInstance, TData> behavior, BehaviorContext<TInstance, TData> context,
            Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return GetOrAdd(exception.GetType()).Faulted(behavior, context, exception);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedConfigurator> Instance =
                new ConcurrentDictionary<Type, CachedConfigurator>();
        }


        interface CachedConfigurator
        {
            Task Faulted<TInstance>(Behavior<TInstance> behavior, BehaviorContext<TInstance> context, Exception exception);

            Task Faulted<TInstance, TData>(Behavior<TInstance, TData> behavior, BehaviorContext<TInstance, TData> context,
                Exception exception);
        }


        class CachedConfigurator<TException> :
            CachedConfigurator
            where TException : Exception
        {
            Task CachedConfigurator.Faulted<TInstance>(Behavior<TInstance> behavior, BehaviorContext<TInstance> context,
                Exception exception)
            {
                var typedException = exception as TException;
                if (typedException == null)
                {
                    throw new ArgumentException(
                        $"The exception type {exception.GetType().Name} did not match the expected type {typeof(TException).Name}");
                }

                var exceptionContext = new BehaviorExceptionContextProxy<TInstance, TException>(context, typedException);

                return behavior.Faulted(exceptionContext);
            }

            Task CachedConfigurator.Faulted<TInstance, TData>(Behavior<TInstance, TData> behavior,
                BehaviorContext<TInstance, TData> context,
                Exception exception)
            {
                var typedException = exception as TException;
                if (typedException == null)
                {
                    throw new ArgumentException(
                        $"The exception type {exception.GetType().Name} did not match the expected type {typeof(TException).Name}");
                }

                var exceptionContext = new BehaviorExceptionContextProxy<TInstance, TData, TException>(context, typedException);

                return behavior.Faulted(exceptionContext);
            }
        }
    }
}