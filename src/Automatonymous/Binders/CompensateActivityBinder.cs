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
    using System;
    using Activities;
    using Behaviors;


    /// <summary>
    /// Creates a compensation activity with the compensation behavior
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    public class CompensateActivityBinder<TInstance, TException> :
        ActivityBinder<TInstance>
        where TInstance : class
        where TException : Exception
    {
        readonly EventActivities<TInstance> _activities;
        readonly Event _event;

        public CompensateActivityBinder(Event @event, EventActivities<TInstance> activities)
        {
            _event = @event;
            _activities = activities;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(_event, state.Enter) || Equals(_event, state.BeforeEnter)
                   || Equals(_event, state.AfterLeave) || Equals(_event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            var builder = new CompensateBehaviorBuilder<TInstance>();
            foreach (var activity in _activities.GetStateActivityBinders())
            {
                activity.Bind(builder);
            }

            var compensateActivity = new CompensateActivity<TInstance, TException>(builder.Behavior);

            state.Bind(_event, compensateActivity);
        }

        public void Bind(BehaviorBuilder<TInstance> builder)
        {
            foreach (var activity in _activities.GetStateActivityBinders())
            {
                activity.Bind(builder);
            }
        }
    }
}