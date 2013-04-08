// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
    using Internals.Reflection;
    using TaskComposition;


    public class CompositeEventActivity<TInstance> :
        Activity<TInstance>
    {
        readonly CompositeEventStatus _complete;
        readonly Action<Composer, TInstance> _completeCallback;
        readonly int _flag;
        readonly ReadWriteProperty<TInstance, CompositeEventStatus> _property;

        public CompositeEventActivity(PropertyInfo propertyInfo, int flag,
            CompositeEventStatus complete, Action<Composer, TInstance> completeCallback)
        {
            _property = new ReadWriteProperty<TInstance, CompositeEventStatus>(propertyInfo);
            _flag = flag;
            _complete = complete;
            _completeCallback = completeCallback;
        }

        void AcceptStateMachineInspector.Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }


        void Activity<TInstance>.Execute(Composer composer, TInstance instance)
        {
            Execute(composer, instance);
        }

        void Activity<TInstance>.Execute<T>(Composer composer, TInstance instance, T ignored)
        {
            Execute(composer, instance);
        }

        void Execute(Composer composer, TInstance instance)
        {
            composer.Execute(() =>
                {
                    CompositeEventStatus value = _property.Get(instance);
                    value.Set(_flag);

                    _property.Set(instance, value);

                    if (!value.Equals(_complete))
                        return composer.ComposeCompleted();

                    var taskComposer = new TaskComposer<TInstance>(composer.CancellationToken);

                    _completeCallback(taskComposer, instance);

                    return taskComposer.Finish();
                });
        }
    }
}