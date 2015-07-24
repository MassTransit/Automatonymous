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
namespace Automatonymous.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;


    public class StateUnhandledEventContext<TInstance> :
        UnhandledEventContext<TInstance>
        where TInstance : class
    {
        readonly EventContext<TInstance> _context;
        readonly StateMachine<TInstance> _machine;
        readonly State _state;

        public StateUnhandledEventContext(EventContext<TInstance> context, State state, StateMachine<TInstance> machine)
        {
            _context = context;
            _state = state;
            _machine = machine;
        }

        public CancellationToken CancellationToken => _context.CancellationToken;

        public State CurrentState => _state;

        public TInstance Instance => _context.Instance;

        public bool HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload payload) where TPayload : class
        {
            return _context.TryGetPayload(out payload);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory) where TPayload : class
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        public Event Event => _context.Event;

        public Task Ignore()
        {
            return TaskUtil.Completed;
        }

        public Task Throw()
        {
            throw new UnhandledEventException(_machine.Name, _context.Event.Name, _state.Name);
        }
    }
}