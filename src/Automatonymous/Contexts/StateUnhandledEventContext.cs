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
    using GreenPipes.Util;


    public class StateUnhandledEventContext<TInstance> :
        BasePipeContext,
        UnhandledEventContext<TInstance>
        where TInstance : class
    {
        readonly EventContext<TInstance> _context;
        readonly StateMachine<TInstance> _machine;
        readonly State _state;

        public StateUnhandledEventContext(EventContext<TInstance> context, State state, StateMachine<TInstance> machine)
            : base(context)
        {
            _context = context;
            _state = state;
            _machine = machine;
        }

        public State CurrentState => _state;
        public TInstance Instance => _context.Instance;
        public Event Event => _context.Event;

        public Task Raise(Event @event, CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.Raise(@event, cancellationToken);
        }

        public Task Raise<TData>(Event<TData> @event, TData data, CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.Raise(@event, data, cancellationToken);
        }

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