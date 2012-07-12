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
    using MassTransit;
    using MassTransit.RequestResponse.Configurators;


    public class PublishRequestActivity<TInstance, TData, TMessage> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly Func<TInstance, TData, TMessage> _messageFactory;
        readonly Action<TInstance, TData, RequestConfigurator<TMessage>> _configurator;

        public PublishRequestActivity(Func<TInstance, TData, TMessage> messageFactory,
            Action<TInstance, TData, RequestConfigurator<TMessage>> configurator)
        {
            _messageFactory = messageFactory;
            _configurator = configurator;
        }

        public void Execute(TInstance instance, TData data)
        {
            TMessage message = _messageFactory(instance, data);
            instance.Bus.PublishRequest(message, configurator => _configurator(instance, data, configurator));
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}