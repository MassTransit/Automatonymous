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
namespace Automatonymous.Accessors
{
    using System;
    using System.Linq;


    public class StateAccessorIndex<TInstance>
        where TInstance : class
    {
        readonly State<TInstance>[] _assignedStates;
        readonly StateMachine<TInstance> _stateMachine;
        readonly Lazy<State<TInstance>[]> _states;

        public StateAccessorIndex(StateMachine<TInstance> stateMachine, State<TInstance> initial, State<TInstance> final, State[] states)
        {
            _stateMachine = stateMachine;

            _assignedStates = new[] {null, initial, final}.Concat(states.Cast<State<TInstance>>()).ToArray();

            _states = new Lazy<State<TInstance>[]>(CreateStateArray);
        }

        public int this[string name]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name));

                for (int i = 1; i < _states.Value.Length; i++)
                {
                    if (_states.Value[i].Name.Equals(name))
                        return i;
                }

                throw new ArgumentException("Unknown state specified: " + name);
            }
        }

        public State<TInstance> this[int index]
        {
            get
            {
                if (index < 0 || index >= _states.Value.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _states.Value[index];
            }
        }

        State<TInstance>[] CreateStateArray()
        {
            return _assignedStates.Concat(_stateMachine.States.Cast<State<TInstance>>()).Distinct().ToArray();
        }
    }
}