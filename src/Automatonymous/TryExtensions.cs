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
namespace Automatonymous
{
    using System;
    using Activities;
    using Binders;


    public static class TryExtensions
    {
        public static EventActivityBinder<TInstance> Try<TInstance>(
            this EventActivityBinder<TInstance> source,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> context,
            Func<ExceptionActivityBinder<TInstance>, ExceptionActivityBinder<TInstance>> handlers)
            where TInstance : class
        {
            EventActivityBinder<TInstance> contextBinder = new SimpleEventActivityBinder<TInstance>(
                source.StateMachine, source.Event);

            contextBinder = context(contextBinder);

            ExceptionActivityBinder<TInstance> exceptionBinder =
                new ExceptionOnlyActivityBinder<TInstance>(source.StateMachine, source.Event);

            exceptionBinder = handlers(exceptionBinder);

            return source.Add(new TryActivity<TInstance>(contextBinder, exceptionBinder.GetActivities()));
        }

        public static EventActivityBinder<TInstance, TData> Try<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> context,
            Func<ExceptionActivityBinder<TInstance, TData>, ExceptionActivityBinder<TInstance, TData>> handlers)
            where TInstance : class
        {
            EventActivityBinder<TInstance, TData> contextBinder = new DataEventActivityBinder<TInstance, TData>(
                source.StateMachine, source.Event);

            contextBinder = context(contextBinder);

            ExceptionActivityBinder<TInstance, TData> exceptionBinder =
                new ExceptionDataActivityBinder<TInstance, TData>(source.StateMachine, source.Event);

            exceptionBinder = handlers(exceptionBinder);

            return source.Add(new TryActivity<TInstance, TData>(source.Event, contextBinder, exceptionBinder.GetActivities()));
        }
    }
}