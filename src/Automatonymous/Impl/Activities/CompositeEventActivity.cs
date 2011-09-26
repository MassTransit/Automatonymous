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
namespace Automatonymous.Impl.Activities
{
    using System;
    using Internal;


    public class CompositeEventActivity<TInstance> :
        Activity<TInstance>
        where TInstance : StateMachineInstance
    {
        readonly CompositeEventStatus _complete;
        readonly Action<TInstance> _completeCallback;
        readonly int _flag;
        readonly FastProperty<TInstance, CompositeEventStatus> _property;

        public CompositeEventActivity(FastProperty<TInstance, CompositeEventStatus> property, int flag,
                                      CompositeEventStatus complete, Action<TInstance> completeCallback)
        {
            _property = property;
            _flag = flag;
            _complete = complete;
            _completeCallback = completeCallback;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }

        public void Execute(TInstance instance)
        {
            CompositeEventStatus value = _property.Get(instance);
            value.Set(_flag);

            _property.Set(instance, value);

            if (value.Equals(_complete))
                _completeCallback(instance);
        }

        public void Execute<TData>(TInstance instance, TData ignored)
        {
            CompositeEventStatus value = _property.Get(instance);
            value.Set(_flag);

            _property.Set(instance, value);

            if (value.Equals(_complete))
                _completeCallback(instance);
        }
    }
}