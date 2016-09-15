// Copyright 2011-2016 Chris Patterson, Dru Sellers
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
namespace Automatonymous.Contexts
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Payloads;


    public class StateMachineEventContext<TInstance> :
        BasePipeContext,
        EventContext<TInstance>
        where TInstance : class
    {
        readonly Event _event;
        readonly TInstance _instance;
        readonly StateMachine<TInstance> _machine;

        public StateMachineEventContext(StateMachine<TInstance> machine, TInstance instance, Event @event,
            CancellationToken cancellationToken)
            : base(new PayloadCache(), cancellationToken)
        {
            _machine = machine;
            _instance = instance;
            _event = @event;
        }

        public StateMachineEventContext(PipeContext context, StateMachine<TInstance> machine, TInstance instance, Event @event)
            : base(context)
        {
            _machine = machine;
            _instance = instance;
            _event = @event;
        }

        public async Task Raise(Event @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventContext = new EventContextProxy<TInstance>(this, @event, cancellationToken);
            using (eventContext)
            {
                await _machine.RaiseEvent(eventContext).ConfigureAwait(false);
            }
        }

        public async Task Raise<TData>(Event<TData> @event, TData data, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventContext = new EventContextProxy<TInstance, TData>(this, @event, data, cancellationToken);
            using (eventContext)
            {
                await _machine.RaiseEvent(eventContext).ConfigureAwait(false);
            }
        }

        Event EventContext<TInstance>.Event => _event;
        TInstance InstanceContext<TInstance>.Instance => _instance;
    }


    public class StateMachineEventContext<TInstance, TData> :
        StateMachineEventContext<TInstance>,
        EventContext<TInstance, TData>
        where TInstance : class
    {
        readonly TData _data;
        readonly Event<TData> _event;

        public StateMachineEventContext(StateMachine<TInstance> machine, TInstance instance, Event<TData> @event, TData data,
            CancellationToken cancellationToken)
            : base(machine, instance, @event, cancellationToken)
        {
            _data = data;
            _event = @event;
        }

        public StateMachineEventContext(PipeContext context, StateMachine<TInstance> machine, TInstance instance, Event<TData> @event,
            TData data)
            : base(context, machine, instance, @event)
        {
            _data = data;
            _event = @event;
        }

        TData EventContext<TInstance, TData>.Data => _data;
        Event<TData> EventContext<TInstance, TData>.Event => _event;
    }
}