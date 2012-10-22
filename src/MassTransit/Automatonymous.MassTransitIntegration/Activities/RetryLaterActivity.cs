﻿// Copyright 2011 Chris Patterson, Dru Sellers
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
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Context;


    public class RetryLaterActivity<TInstance, TData> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
    {
        readonly Task _done = Task.Factory.StartNew(() => { });

        public Task Execute(TInstance instance, TData data)
        {
            IConsumeContext<TData> context = ContextStorage.MessageContext<TData>();

            context.RetryLater();

            return _done;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}