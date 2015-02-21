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
namespace Automatonymous
{
    using System.Threading.Tasks;
    using Activities;


    public class ActivityBehavior<TInstance> :
        Behavior<TInstance>
    {
        readonly Activity<TInstance> _activity;
        readonly Behavior<TInstance> _next;

        public ActivityBehavior(Activity<TInstance> activity, Behavior<TInstance> next)
        {
            _activity = activity;
            _next = next;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x =>
            {
                _activity.Accept(visitor);
                _next.Accept(visitor);
            });
        }

        public Task Execute(BehaviorContext<TInstance> context)
        {
            return _activity.Execute(context, _next);
        }

        public Task Execute<T>(BehaviorContext<TInstance, T> context)
        {
            var behavior = new SplitBehavior<TInstance, T>(_next);

            return _activity.Execute(context, behavior);
        }
    }

    public class ActivityBehavior<TInstance, TData> :
        Behavior<TInstance, TData>
    {
        readonly Activity<TInstance, TData> _activity;
        readonly Behavior<TInstance, TData> _next;

        public ActivityBehavior(Activity<TInstance, TData> activity, Behavior<TInstance, TData> next)
        {
            _activity = activity;
            _next = next;
        }

        public Task Execute(BehaviorContext<TInstance, TData> context)
        {
            return _activity.Execute(context, _next);
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x =>
            {
                _activity.Accept(visitor);
                _next.Accept(visitor);
            });
        }
    }
}