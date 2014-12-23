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
namespace Automatonymous.Lifts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class InstanceLiftImpl<TStateMachine, TInstance> :
        InstanceLift<TStateMachine>
        where TStateMachine : StateMachine<TInstance>
        where TInstance : class
    {
        readonly TInstance _instance;
        readonly TStateMachine _stateMachine;

        public InstanceLiftImpl(TStateMachine stateMachine, TInstance instance)
        {
            _stateMachine = stateMachine;
            _instance = instance;
        }

        Task InstanceLift<TStateMachine>.Raise(Event @event, CancellationToken cancellationToken)
        {
            return _stateMachine.RaiseEvent(_instance, @event, cancellationToken);
        }

        Task InstanceLift<TStateMachine>.Raise<T>(Event<T> @event, T value, CancellationToken cancellationToken)
        {
            return _stateMachine.RaiseEvent(_instance, @event, value, cancellationToken);
        }

        Task InstanceLift<TStateMachine>.Raise(Func<TStateMachine, Event> eventSelector, CancellationToken cancellationToken)
        {
            return _stateMachine.RaiseEvent(_instance, eventSelector, cancellationToken);
        }

        Task InstanceLift<TStateMachine>.Raise<T>(Func<TStateMachine, Event<T>> eventSelector, T data, CancellationToken cancellationToken)
        {
            return _stateMachine.RaiseEvent(_instance, eventSelector, data, cancellationToken);
        }
    }
}