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
    using Accessors;


    public class CompositeEventActivity<TInstance> :
        Activity<TInstance>
        where TInstance : class
    {
        readonly CompositeEventStatusAccessor<TInstance> _accessor;
        readonly CompositeEventStatus _complete;
        readonly Event _event;
        readonly int _flag;
        readonly StateMachine<TInstance> _stateMachine;

        public CompositeEventActivity(CompositeEventStatusAccessor<TInstance> accessor, int flag, CompositeEventStatus complete,
            StateMachine<TInstance> stateMachine, Event @event)
        {
            _accessor = accessor;
            _flag = flag;
            _complete = complete;
            _stateMachine = stateMachine;
            _event = @event;
        }

        public Event Event
        {
            get { return _event; }
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            await Execute(context);

            await next.Execute(context);
        }

        async Task Activity<TInstance>.Execute<TData>(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            await Execute(context);

            await next.Execute(context);
        }

        async Task Execute(BehaviorContext<TInstance> context)
        {
            CompositeEventStatus value = _accessor.Get(context.Instance);
            value.Set(_flag);

            _accessor.Set(context.Instance, value);

            if (!value.Equals(_complete))
                return;

            await RaiseCompositeEvent(context);
        }

        Task RaiseCompositeEvent(BehaviorContext<TInstance> context)
        {
            BehaviorContext<TInstance> compositeEventContext = context.GetProxy(_event);

            return _stateMachine.RaiseEvent(compositeEventContext);
        }
    }
}