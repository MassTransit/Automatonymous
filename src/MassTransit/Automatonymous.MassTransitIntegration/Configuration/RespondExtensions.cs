﻿// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
    using MassTransit;


    public static class RespondExtensions
    {
        public static EventActivityBinder<TInstance, TData> Respond<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, TMessage message)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            Func<TInstance, IConsumeContext<TData>, TMessage> factory = (saga, context) => message;

            return source.Add(new RespondActivity<TInstance, TData, TMessage>(factory, x => { }));
        }

        public static EventActivityBinder<TInstance, TData> Respond<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, TMessage message,
            Action<ISendContext<TMessage>> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            Func<TInstance, IConsumeContext<TData>, TMessage> factory = (saga, context) => message;

            return source.Add(new RespondActivity<TInstance, TData, TMessage>(factory, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Respond<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Func<TInstance, TMessage> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            Func<TInstance, IConsumeContext<TData>, TMessage> factory =
                (saga, context) => messageFactory(saga);

            return source.Add(new RespondActivity<TInstance, TData, TMessage>(factory, x => { }));
        }

        public static EventActivityBinder<TInstance, TData> Respond<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Func<TInstance, TMessage> messageFactory,
            Action<ISendContext<TMessage>> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            Func<TInstance, IConsumeContext<TData>, TMessage> factory =
                (saga, context) => messageFactory(saga);

            return source.Add(new RespondActivity<TInstance, TData, TMessage>(factory, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Respond<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Func<TInstance, TData, TMessage> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            Func<TInstance, IConsumeContext<TData>, TMessage> factory =
                (saga, context) => messageFactory(saga, context.Message);

            return source.Add(new RespondActivity<TInstance, TData, TMessage>(factory, x => { }));
        }

        public static EventActivityBinder<TInstance, TData> Respond<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Func<TInstance, TData, TMessage> messageFactory,
            Action<ISendContext<TMessage>> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            Func<TInstance, IConsumeContext<TData>, TMessage> factory =
                (saga, context) => messageFactory(saga, context.Message);

            return source.Add(new RespondActivity<TInstance, TData, TMessage>(factory, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Respond<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source,
            Func<TInstance, IConsumeContext<TData>, TData, TMessage> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            Func<TInstance, IConsumeContext<TData>, TMessage> factory =
                (saga, context) => messageFactory(saga, context, context.Message);

            return source.Add(new RespondActivity<TInstance, TData, TMessage>(factory, x => { }));
        }

        public static EventActivityBinder<TInstance, TData> Respond<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source,
            Func<TInstance, IConsumeContext<TData>, TData, TMessage> messageFactory,
            Action<ISendContext<TMessage>> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            Func<TInstance, IConsumeContext<TData>, TMessage> factory =
                (saga, context) => messageFactory(saga, context, context.Message);

            return source.Add(new RespondActivity<TInstance, TData, TMessage>(factory, contextCallback));
        }
    }
}