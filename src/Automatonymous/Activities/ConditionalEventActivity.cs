// Copyright 2011-2014 Chris Patterson, Dru Sellers
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
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Filters an activity to only execute if the condition is met
    /// </summary>
    /// <typeparam name="TInstance">The instance type</typeparam>
    /// <typeparam name="TData">The event data type</typeparam>
    public class ConditionalEventActivity<TInstance, TData> :
        Activity<TInstance, TData>
    {
        readonly Activity<TInstance> _activity;
        readonly Func<BehaviorContext<TInstance, TData>, bool> _filter;

        public ConditionalEventActivity(Activity<TInstance> activity, Func<BehaviorContext<TInstance, TData>, bool> filter)
        {
            _activity = activity;
            _filter = filter;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => _activity.Accept(inspector));
        }

        Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            if (_filter(context))
                return _activity.Execute(context, next);

            return next.Execute(context);
        }
    }
}