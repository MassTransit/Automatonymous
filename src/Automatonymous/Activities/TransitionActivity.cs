// Copyright 2011-2015 Chris Patterson, Dru Sellers
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

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            return next.Faulted(context);
        }

        Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next)
        {
            return next.Faulted(context);
        }

        async Task Transition(BehaviorContext<TInstance> context)
        {
            State<TInstance> currentState = await _currentStateAccessor.Get(context);
            if (_toState.Equals(currentState))
                return; // Homey don't play re-entry, at least not yet.

            if (currentState != null && !currentState.HasState(_toState))
            {
                await RaiseCurrentStateLeaveEvents(context, currentState);
            }

            await RaiseBeforeEnterEvents(context, currentState, _toState);

            await _currentStateAccessor.Set(context, _toState);

            if (currentState != null)
            {
                await RaiseAfterLeaveEvents(context, currentState, _toState);
            }

            if (currentState == null || !_toState.HasState(currentState))
            {
                State<TInstance> superState = _toState.SuperState;
                while (superState != null && (currentState == null || !superState.HasState(currentState)))
                {
                    BehaviorContext<TInstance> superStateEnterContext = context.GetProxy(superState.Enter);
                    await superState.Raise(superStateEnterContext);

                    superState = superState.SuperState;
                }

                BehaviorContext<TInstance> enterContext = context.GetProxy(_toState.Enter);
                await _toState.Raise(enterContext);
            }
        }

        async Task RaiseBeforeEnterEvents(BehaviorContext<TInstance> context, State<TInstance> currentState, State<TInstance> toState)
        {
            State<TInstance> superState = toState.SuperState;
            if (superState != null && (currentState == null || !superState.HasState(currentState)))
            {
                await RaiseBeforeEnterEvents(context, currentState, superState);
            }

            if (currentState != null && toState.HasState(currentState))
                return;

            BehaviorContext<TInstance, State> beforeContext = context.GetProxy(toState.BeforeEnter, currentState);
            await toState.Raise(beforeContext);
        }

        async Task RaiseAfterLeaveEvents(BehaviorContext<TInstance> context, State<TInstance> fromState, State<TInstance> toState)
        {
            if (fromState.HasState(toState))
                return;

            BehaviorContext<TInstance, State> afterContext = context.GetProxy(fromState.AfterLeave, toState);
            await fromState.Raise(afterContext);

            State<TInstance> superState = fromState.SuperState;
            if (superState != null)
            {
                await RaiseAfterLeaveEvents(context, superState, toState);
            }
        }

        async Task RaiseCurrentStateLeaveEvents(BehaviorContext<TInstance> context, State<TInstance> fromState)
        {
            BehaviorContext<TInstance> leaveContext = context.GetProxy(fromState.Leave);
            await fromState.Raise(leaveContext);

            State<TInstance> superState = fromState.SuperState;
            while (superState != null && !superState.HasState(_toState))
            {
                BehaviorContext<TInstance> superStateLeaveContext = context.GetProxy(superState.Leave);
                await superState.Raise(superStateLeaveContext);

                superState = superState.SuperState;
            }
        }
    }
}