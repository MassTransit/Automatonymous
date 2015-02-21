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
    using Behaviors;


    /// <summary>
    /// Routes event activity to an activity
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public class EventStateActivityBinder<TInstance> :
        StateActivityBinder<TInstance>
    {
        readonly Activity<TInstance> _activity;
        readonly Event _event;

        public EventStateActivityBinder(Event @event, Activity<TInstance> activity)
        {
            _event = @event;
            _activity = activity;
        }

        public Activity<TInstance> Activity
        {
            get { return _activity; }
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(_event, state.Enter) || Equals(_event, state.BeforeEnter)
                   || Equals(_event, state.AfterLeave) || Equals(_event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            state.Bind(_event, _activity);
        }

        public void Bind(BehaviorBuilder<TInstance> builder)
        {
            builder.Add(_activity);
        }
    }
}