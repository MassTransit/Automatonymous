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
    using System.Threading.Tasks;
    using MassTransit;


    public class PublishActivity<TInstance, TMessage> :
        Activity<TInstance>
        where TInstance : SagaStateMachineInstance
        where TMessage : class
    {
        readonly Task _finishedTask = Task.Factory.StartNew(() => { });
        readonly Action<IPublishContext<TMessage>> _contextCallback;
        readonly Func<TInstance, TMessage> _messageFactory;

        public PublishActivity(Func<TInstance, TMessage> messageFactory,
                               Action<IPublishContext<TMessage>> contextCallback)
        {
            _messageFactory = messageFactory;
            _contextCallback = contextCallback;
        }

        public Task Execute(TInstance instance)
        {
            TMessage message = _messageFactory(instance);
            instance.Bus.Publish(message, _contextCallback);
            return _finishedTask;
        }

        public Task Execute<TData>(TInstance instance, TData value)
        {
            TMessage message = _messageFactory(instance);
            instance.Bus.Publish(message, _contextCallback);
            return _finishedTask;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }


    public class PublishActivity<TInstance, TData, TMessage> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly Task _finishedTask = Task.Factory.StartNew(() => { });
        readonly Action<IPublishContext<TMessage>> _contextCallback;
        readonly Func<TInstance, TData, TMessage> _messageFactory;

        public PublishActivity(Func<TInstance, TData, TMessage> messageFactory,
                               Action<IPublishContext<TMessage>> contextCallback)
        {
            _messageFactory = messageFactory;
            _contextCallback = contextCallback;
        }

        public Task Execute(TInstance instance, TData data)
        {
            TMessage message = _messageFactory(instance, data);
            instance.Bus.Publish(message, _contextCallback);
            return _finishedTask;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}