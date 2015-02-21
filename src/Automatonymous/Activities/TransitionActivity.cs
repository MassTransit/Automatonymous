// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            await Transition(context);

            await next.Execute(context);
        }

        async Task Activity<TInstance>.Execute<TData>(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            await Transition(context);

            await next.Execute(context);
        }

        async Task Transition(BehaviorContext<TInstance> context)
        {
            State<TInstance> currentState = await _currentStateAccessor.Get(context);
            if (_toState.Equals(currentState))
                return;

            if (currentState != null)
            {
                BehaviorContext<TInstance> leaveContext = context.GetProxy(currentState.Leave);
                await currentState.Raise(leaveContext);
            }

            BehaviorContext<TInstance, State> beforeContext = context.GetProxy(_toState.BeforeEnter, currentState);
            await _toState.Raise(beforeContext);

            await _currentStateAccessor.Set(context, _toState);

            if (currentState != null)
            {
                BehaviorContext<TInstance, State> leaveContext = context.GetProxy(currentState.AfterLeave, _toState);
                await currentState.Raise(leaveContext);
            }

            BehaviorContext<TInstance> enterContext = context.GetProxy(_toState.Enter);
            await _toState.Raise(enterContext);
        }
    }
}