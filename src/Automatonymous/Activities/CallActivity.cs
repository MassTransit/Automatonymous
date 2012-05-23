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


    public class CallActivity<TInstance> :
        Activity<TInstance>
    {
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

        public void Execute(TInstance instance)
        {
            _action(instance);
        }

        public void Execute<TData>(TInstance instance, TData value)
        {
            _action(instance);
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }


    public class CallActivity<TInstance, TData> :
        Activity<TInstance, TData>
        where TData : class
    {
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

        public void Execute(TInstance instance, TData data)
        {
            _action(instance, data);
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}