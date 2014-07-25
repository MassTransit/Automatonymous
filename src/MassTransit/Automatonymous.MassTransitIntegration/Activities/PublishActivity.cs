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
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;


    public class PublishActivity<TInstance, TMessage> :
        Activity<TInstance>
        where TInstance : SagaStateMachineInstance
        where TMessage : class
    {
        readonly Action<IPublishContext<TMessage>> _contextCallback;
        readonly Func<TInstance, TMessage> _messageFactory;

        public PublishActivity(Func<TInstance, TMessage> messageFactory,
            Action<IPublishContext<TMessage>> contextCallback)
        {
            _messageFactory = messageFactory;
            _contextCallback = contextCallback;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }

        async Task Activity<TInstance>.Execute(TInstance instance, CancellationToken cancellationToken)
        {
            TMessage message = _messageFactory(instance);
            instance.Bus.Publish(message, _contextCallback);
        }

        async Task Activity<TInstance>.Execute<T>(TInstance instance, T value, CancellationToken cancellationToken)
        {
            TMessage message = _messageFactory(instance);
            instance.Bus.Publish(message, _contextCallback);
        }
    }


    public class PublishActivity<TInstance, TData, TMessage> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly Action<IPublishContext<TMessage>> _contextCallback;
        readonly Func<TInstance, TData, TMessage> _messageFactory;

        public PublishActivity(Func<TInstance, TData, TMessage> messageFactory,
            Action<IPublishContext<TMessage>> contextCallback)
        {
            _messageFactory = messageFactory;
            _contextCallback = contextCallback;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }

        async Task Activity<TInstance, TData>.Execute(TInstance instance, TData value, CancellationToken cancellationToken)
        {
            TMessage message = _messageFactory(instance, value);
            instance.Bus.Publish(message, _contextCallback);
        }
    }
}