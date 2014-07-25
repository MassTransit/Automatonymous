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
    using System.Threading;
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

        void AcceptStateMachineInspector.Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }

        Task Activity<TInstance>.Execute(TInstance instance, CancellationToken cancellationToken)
        {
            return Transition(instance, cancellationToken);
        }

        Task Activity<TInstance>.Execute<T>(TInstance instance, T value, CancellationToken cancellationToken)
        {
            return Transition(instance, cancellationToken);
        }

        async Task Transition(TInstance instance, CancellationToken cancellationToken)
        {
            State<TInstance> currentState = _currentStateAccessor.Get(instance);
            if (_toState.Equals(currentState))
                return;

            if (currentState != null)
                await currentState.Raise(instance, currentState.Leave, cancellationToken);

            await _toState.Raise(instance, _toState.BeforeEnter, currentState, cancellationToken);

            _currentStateAccessor.Set(instance, _toState);

            if (currentState != null)
                await currentState.Raise(instance, currentState.AfterLeave, _toState, cancellationToken);

            await _toState.Raise(instance, _toState.Enter, cancellationToken);
        }
    }
}