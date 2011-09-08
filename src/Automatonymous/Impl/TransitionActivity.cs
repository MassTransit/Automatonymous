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
namespace Automatonymous.Impl
{
    using System;


    public class TransitionActivity<TInstance> :
        Activity<TInstance>
        where TInstance : StateMachineInstance
    {
        readonly StateImpl<TInstance> _toState;

        public TransitionActivity(StateImpl<TInstance> toState)
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

            var currentState = instance.CurrentState as StateImpl<TInstance>;
            if (currentState != null)
                currentState.Raise(instance, currentState.Leave, null);

            instance.CurrentState = ToState;
            _toState.Raise(instance, ToState.Enter, null);
        }

        public void Inspect(StateMachineInspector inspector)
        {
            throw new NotImplementedException();
        }

        public Event Event
        {
            get { throw new NotImplementedException(); }
        }
    }
}

