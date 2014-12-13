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


    public class ActivityBehavior<TInstance> :
        Behavior<TInstance>
    {
        readonly Activity<TInstance> _filter;
        readonly Behavior<TInstance> _next;

        public ActivityBehavior(Activity<TInstance> filter, Behavior<TInstance> next)
        {
            _filter = filter;
            _next = next;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x =>
            {
                _filter.Accept(inspector);
                _next.Accept(inspector);
            });
        }

        public Task Execute(BehaviorContext<TInstance> context)
        {
            return _filter.Execute(context, _next);
        }

        public Task Execute<T>(BehaviorContext<TInstance, T> context)
        {
            return _filter.Execute(context, _next);
        }
    }
}