// Copyright 2011-2019 Chris Patterson, Dru Sellers
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
    using System.Threading.Tasks;
    using GreenPipes;


    public class StateMachineEventContextProxy<TInstance, TData> :
        ProxyPipeContext,
        EventContext<TInstance, TData>
        where TInstance : class
    {
        readonly TData _data;
        readonly Event<TData> _event;
        readonly TInstance _instance;
        readonly StateMachine<TInstance> _machine;

        public StateMachineEventContextProxy(PipeContext context, StateMachine<TInstance> machine, TInstance instance, Event<TData> @event,
            TData data)
            : base(context)
        {
            _machine = machine;
            _instance = instance;
            _event = @event;
            _data = data;
        }

        public Task Raise(Event @event)
        {
            var eventContext = new EventContextProxy<TInstance>(this, @event);

            return _machine.RaiseEvent(eventContext);
        }

        public Task Raise<T>(Event<T> @event, T data)
        {
            var eventContext = new EventContextProxy<TInstance, T>(this, @event, data);

            return _machine.RaiseEvent(eventContext);
        }

        Event EventContext<TInstance>.Event => _event;
        TInstance InstanceContext<TInstance>.Instance => _instance;

        TData EventContext<TInstance, TData>.Data => _data;
        Event<TData> EventContext<TInstance, TData>.Event => _event;
    }
}
