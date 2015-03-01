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
namespace Automatonymous.Binders
{
    using System.Threading.Tasks;
    using Behaviors;


    public class IgnoreEventActivityBinder<TInstance> :
        ActivityBinder<TInstance>
    {
        readonly Event _event;

        public IgnoreEventActivityBinder(Event @event)
        {
            _event = @event;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(_event, state.Enter) || Equals(_event, state.BeforeEnter)
                   || Equals(_event, state.AfterLeave) || Equals(_event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            state.Ignore(_event);
        }

        public void Bind(BehaviorBuilder<TInstance> builder)
        {
        }
    }


    public class IgnoreEventActivityBinder<TInstance, TData> :
        ActivityBinder<TInstance>
    {
        readonly Event<TData> _event;
        readonly StateMachineEventFilter<TInstance, TData> _filter;

        public IgnoreEventActivityBinder(Event<TData> @event, StateMachineEventFilter<TInstance, TData> filter)
        {
            _event = @event;
            _filter = filter;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(_event, state.Enter) || Equals(_event, state.BeforeEnter)
                   || Equals(_event, state.AfterLeave) || Equals(_event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            state.Ignore(_event, _filter);
        }

        public void Bind(BehaviorBuilder<TInstance> builder)
        {
        }
    }
}