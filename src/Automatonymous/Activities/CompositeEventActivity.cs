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
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals;


    public class CompositeEventActivity<TInstance> :
        Activity<TInstance>
    {
        readonly CompositeEventStatus _complete;
        readonly Func<BehaviorContext<TInstance>, Task> _completeCallback;
        readonly int _flag;
        readonly ReadWriteProperty<TInstance, CompositeEventStatus> _property;

        public CompositeEventActivity(PropertyInfo propertyInfo, int flag, CompositeEventStatus complete,
            Func<BehaviorContext<TInstance>, Task> completeCallback)
        {
            _property = new ReadWriteProperty<TInstance, CompositeEventStatus>(propertyInfo);
            _flag = flag;
            _complete = complete;
            _completeCallback = completeCallback;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            await Execute(context);

            await next.Execute(context);
        }

        async Task Activity<TInstance>.Execute<TData>(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            await Execute(context);

            await next.Execute(context);
        }

        async Task Execute(BehaviorContext<TInstance> context)
        {
            CompositeEventStatus value = _property.Get(context.Instance);
            value.Set(_flag);

            _property.Set(context.Instance, value);

            if (!value.Equals(_complete))
                return;

            await _completeCallback(context);
        }
    }
}