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
namespace Automatonymous
{
    using System;
    using Impl;
    using Impl.Activities;


    public static class ExceptionHandlingExtensions
    {
        public static EventActivityBinder<TInstance> Try<TInstance>(
            this EventActivityBinder<TInstance> source,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> context,
            Func<ExceptionActivityBinder<TInstance>, ExceptionActivityBinder<TInstance>> handlers)
            where TInstance : StateMachineInstance
        {
            EventActivityBinder<TInstance> contextBinder = new SimpleEventActivityBinder<TInstance>(
                source.StateMachine, source.Event);

            contextBinder = context(contextBinder);

            ExceptionActivityBinder<TInstance> exceptionBinder =
                new ExceptionActivityBinderImpl<TInstance>(source.StateMachine, source.Event);

            exceptionBinder = handlers(exceptionBinder);

            return source.Add(new TryActivity<TInstance>(source.Event, contextBinder, exceptionBinder));
        }

        public static EventActivityBinder<TInstance> Handle<TInstance>(
            this EventActivityBinder<TInstance> source,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> context,
            Func<ExceptionActivityBinder<TInstance>, ExceptionActivityBinder<TInstance>> handlers)
            where TInstance : StateMachineInstance
        {
            EventActivityBinder<TInstance> contextBinder = new SimpleEventActivityBinder<TInstance>(
                source.StateMachine, source.Event);

            contextBinder = context(contextBinder);

            ExceptionActivityBinder<TInstance> exceptionBinder =
                new ExceptionActivityBinderImpl<TInstance>(source.StateMachine, source.Event);

            exceptionBinder = handlers(exceptionBinder);

            return source.Add(new TryActivity<TInstance>(source.Event, contextBinder, exceptionBinder));
        }
    }
}