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
    using System.Threading.Tasks;


    public class TransitionActivity<TInstance> :
        Activity<TInstance>
        where TInstance : class
    {
        readonly StateAccessor<TInstance> _currentStateAccessor;
        readonly State<TInstance> _toState;

        public TransitionActivity(State<TInstance> toState, StateAccessor<TInstance> currentStateAccessor)
        {
            _toState = toState;
            _currentStateAccessor = currentStateAccessor;
        }

        public State ToState
        {
            get { return _toState; }
        }

        public async Task Execute(TInstance instance)
        {
            State lastState = _currentStateAccessor.Get(instance);
            if (lastState == _toState)
                return;

            await lastState.WithState<TInstance>(async x => await x.Raise(instance, x.Leave));
            await _toState.WithState<TInstance>(async x => await x.Raise(instance, x.BeforeEnter, lastState));

            _currentStateAccessor.Set(instance, _toState);

            await lastState.WithState<TInstance>(async x => await x.Raise(instance, x.AfterLeave, _toState));
            await _toState.WithState<TInstance>(async x => await x.Raise(instance, x.Enter));
        }

        public async Task Execute<TData>(TInstance instance, TData value)
        {
            await Execute(instance);
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}