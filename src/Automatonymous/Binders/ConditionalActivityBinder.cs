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
namespace Automatonymous.Binders
{
    using Activities;
    using Behaviors;


    public class ConditionalActivityBinder<TInstance> :
        ActivityBinder<TInstance>
        where TInstance : class
    {
        readonly EventActivities<TInstance> _activities;
        readonly StateMachineCondition<TInstance> _condition;
        readonly Event _event;

        public ConditionalActivityBinder(Event @event, StateMachineCondition<TInstance> condition, EventActivities<TInstance> activities)
        {
            _activities = activities;
            _condition = condition;
            _event = @event;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(_event, state.Enter) || Equals(_event, state.BeforeEnter)
                   || Equals(_event, state.AfterLeave) || Equals(_event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            var builder = new ActivityBehaviorBuilder<TInstance>();

            foreach (var activity in _activities.GetStateActivityBinders())
            {
                activity.Bind(builder);
            }

            var conditionActivity = new ConditionActivity<TInstance>(_condition, builder.Behavior);

            state.Bind(_event, conditionActivity);
        }

        public void Bind(BehaviorBuilder<TInstance> builder)
        {
            var stateBuilder = new ActivityBehaviorBuilder<TInstance>();

            foreach (var activity in _activities.GetStateActivityBinders())
            {
                activity.Bind(stateBuilder);
            }

            var conditionActivity = new ConditionActivity<TInstance>(_condition, stateBuilder.Behavior);

            builder.Add(conditionActivity);
        }
    }


    public class ConditionalActivityBinder<TInstance, TData> :
        ActivityBinder<TInstance>
        where TInstance : class
    {
        readonly EventActivities<TInstance> _activities;
        readonly StateMachineCondition<TInstance, TData> _condition;
        readonly Event _event;

        public ConditionalActivityBinder(Event @event, StateMachineCondition<TInstance, TData> condition,
            EventActivities<TInstance> activities)
        {
            _activities = activities;
            _condition = condition;
            _event = @event;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(_event, state.Enter) || Equals(_event, state.BeforeEnter)
                   || Equals(_event, state.AfterLeave) || Equals(_event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            var builder = new ActivityBehaviorBuilder<TInstance>();

            foreach (var activity in _activities.GetStateActivityBinders())
            {
                activity.Bind(builder);
            }

            var conditionActivity = new ConditionActivity<TInstance, TData>(_condition, builder.Behavior);

            state.Bind(_event, conditionActivity);
        }

        public void Bind(BehaviorBuilder<TInstance> builder)
        {
            var stateBuilder = new ActivityBehaviorBuilder<TInstance>();

            foreach (var activity in _activities.GetStateActivityBinders())
            {
                activity.Bind(stateBuilder);
            }

            var conditionActivity = new ConditionActivity<TInstance, TData>(_condition, stateBuilder.Behavior);

            builder.Add(conditionActivity);
        }
    }
}