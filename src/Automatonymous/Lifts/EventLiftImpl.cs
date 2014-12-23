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
namespace Automatonymous.Lifts
{
    using System.Threading;
    using System.Threading.Tasks;
    using Events;


    public class EventLiftImpl<TInstance> :
        EventLift<TInstance>
        where TInstance : class
    {
        readonly Event _event;
        readonly StateMachine<TInstance> _machine;

        public EventLiftImpl(StateMachine<TInstance> machine, Event @event)
        {
            _machine = machine;
            _event = @event;
        }

        Task EventLift<TInstance>.Raise(TInstance instance, CancellationToken cancellationToken)
        {
            var context = new StateMachineEventContext<TInstance>(_machine, instance, _event, cancellationToken);

            return _machine.RaiseEvent(context);
        }
    }


    public class EventLiftImpl<TInstance, TData> :
        EventLift<TInstance, TData>
        where TInstance : class
    {
        readonly Event<TData> _event;
        readonly StateMachine<TInstance> _machine;

        public EventLiftImpl(StateMachine<TInstance> machine, Event<TData> @event)
        {
            _machine = machine;
            _event = @event;
        }

        Task EventLift<TInstance, TData>.Raise(TInstance instance, TData data, CancellationToken cancellationToken)
        {
            var context = new StateMachineEventContext<TInstance, TData>(_machine, instance, _event, data, cancellationToken);

            return _machine.RaiseEvent(context);
        }
    }
}