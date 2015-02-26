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
namespace Automatonymous.Behaviors
{
    using System.Threading.Tasks;


    public class WidenBehavior<TInstance, TData> :
        Behavior<TInstance>
    {
        readonly TData _data;
        readonly Event<TData> _event;
        readonly Behavior<TInstance, TData> _next;

        public WidenBehavior(Behavior<TInstance, TData> next, EventContext<TInstance, TData> context)
        {
            _next = next;
            _data = context.Data;
            _event = context.Event;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        Task Behavior<TInstance>.Execute(BehaviorContext<TInstance> context)
        {
            BehaviorContext<TInstance, TData> nextContext = context.GetProxy(_event, _data);

            return _next.Execute(nextContext);
        }

        Task Behavior<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context)
        {
            var nextContext = context as BehaviorContext<TInstance, TData> ?? context.GetProxy(_event, _data);

            return _next.Execute(nextContext);
        }
    }
}