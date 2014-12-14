// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Activities;


    public class SimpleEventActivityBinder<TInstance> :
        EventActivityBinder<TInstance>
        where TInstance : class
    {
        readonly IEnumerable<Activity<TInstance>> _activities;
        readonly Event _event;
        readonly StateMachine<TInstance> _machine;

        public SimpleEventActivityBinder(StateMachine<TInstance> machine, Event @event)
            : this(machine, @event, Enumerable.Empty<Activity<TInstance>>())
        {
        }

        SimpleEventActivityBinder(StateMachine<TInstance> machine, Event @event,
            IEnumerable<Activity<TInstance>> activities)
        {
            _event = @event;
            _activities = activities;
            _machine = machine;
        }

        Event EventActivityBinder<TInstance>.Event
        {
            get { return _event; }
        }

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.Add(Activity<TInstance> activity)
        {
            return new SimpleEventActivityBinder<TInstance>(_machine, _event,
                _activities.Concat(Enumerable.Repeat(activity, 1)));
        }

        StateMachine<TInstance> EventActivityBinder<TInstance>.StateMachine
        {
            get { return _machine; }
        }

        IEnumerator<EventActivity<TInstance>> IEnumerable<EventActivity<TInstance>>.GetEnumerator()
        {
            return _activities.Select(CreateEventActivity).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<EventActivity<TInstance>>)this).GetEnumerator();
        }

        EventActivity<TInstance> CreateEventActivity(Activity<TInstance> activity)
        {
            return new EventActivityShim<TInstance>(_event, activity);
        }
    }
}