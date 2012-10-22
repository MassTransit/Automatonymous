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
    using System.Linq.Expressions;
    using System.Threading.Tasks;


    public class CallActivity<TInstance> :
        Activity<TInstance>
    {
        readonly Task _task = Task.Factory.StartNew(() => { });
        readonly Action<TInstance> _action;
        readonly Expression<Action<TInstance>> _expression;

        public CallActivity(Expression<Action<TInstance>> expression)
        {
            _expression = expression;
            _action = expression.Compile();
        }

        public Expression<Action<TInstance>> Expression
        {
            get { return _expression; }
        }

        public Task Execute(TInstance instance)
        {
            _action(instance);
            return _task;
        }

        public Task Execute<TData>(TInstance instance, TData value)
        {
            _action(instance);
            return _task;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }


    public class CallActivity<TInstance, TData> :
        Activity<TInstance, TData>
    {
        readonly Task _task = Task.Factory.StartNew(() => { });
        readonly Action<TInstance, TData> _action;
        readonly Expression<Action<TInstance, TData>> _expression;

        public CallActivity(Expression<Action<TInstance, TData>> expression)
        {
            _expression = expression;
            _action = expression.Compile();
        }

        public Expression<Action<TInstance, TData>> Expression
        {
            get { return _expression; }
        }

        public Task Execute(TInstance instance, TData data)
        {
            _action(instance, data);
            return _task;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}