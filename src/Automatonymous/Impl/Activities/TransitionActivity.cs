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
    public class TransitionActivity<TInstance> :
        Activity<TInstance>
        where TInstance : StateMachineInstance
    {
        readonly State<TInstance> _toState;

        public TransitionActivity(State toState)
        {
            _toState = toState.For<TInstance>();
        }

        public TransitionActivity(State<TInstance> toState)
        {
            _toState = toState;
        }

        public State ToState
        {
            get { return _toState; }
        }

        public void Execute(TInstance instance)
        {
            if (instance.CurrentState == _toState)
                return;

            instance.CurrentState.WithState<TInstance>(x => x.Raise(instance, x.Leave));

            instance.CurrentState = ToState;
            _toState.Raise(instance, ToState.Enter);
        }

        public void Execute<TData>(TInstance instance, TData value)
        {
            Execute(instance);
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}