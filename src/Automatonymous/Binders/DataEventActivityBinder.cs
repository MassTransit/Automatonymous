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
namespace Automatonymous.Binders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Activities;


    public class DataEventActivityBinder<TInstance, TData> :
        EventActivityBinder<TInstance, TData>
        where TInstance : class
    {
        readonly IEnumerable<Activity<TInstance, TData>> _activities;
        readonly Event<TData> _event;
        readonly Func<BehaviorContext<TInstance, TData>, bool> _filter;
        readonly StateMachine<TInstance> _machine;

        public DataEventActivityBinder(StateMachine<TInstance> machine, Event<TData> @event)
            : this(machine, @event, null, Enumerable.Empty<Activity<TInstance, TData>>())
        {
        }

        public DataEventActivityBinder(StateMachine<TInstance> machine, Event<TData> @event,
            Func<BehaviorContext<TInstance, TData>, bool> filter)
            : this(machine, @event, filter, Enumerable.Empty<Activity<TInstance, TData>>())
        {
        }

        public DataEventActivityBinder(StateMachine<TInstance> machine, Event<TData> @event,
            Func<BehaviorContext<TInstance, TData>, bool> filter,
            IEnumerable<Activity<TInstance, TData>> activities)
        {
            _event = @event;
            _activities = activities;
            _machine = machine;
            _filter = filter;
        }

        public Func<BehaviorContext<TInstance, TData>, bool> Filter
        {
            get { return _filter; }
        }

        Event<TData> EventActivityBinder<TInstance, TData>.Event
        {
            get { return _event; }
        }

        EventActivityBinder<TInstance, TData> EventActivityBinder<TInstance, TData>.Add(Activity<TInstance> activity)
        {
            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter,
                _activities.Concat(Enumerable.Repeat(new SlimActivity<TInstance, TData>(activity), 1)));
        }

        EventActivityBinder<TInstance, TData> EventActivityBinder<TInstance, TData>.Add(Activity<TInstance, TData> activity)
        {
            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter,
                _activities.Concat(Enumerable.Repeat(activity, 1)));
        }

        public IEnumerable<EventActivity<TInstance, TData>> GetActivities()
        {
            if (_filter == null)
                return _activities.Select(x => new EventActivityShim<TInstance, TData>(_event, x));

            return _activities.Select(x => new ConditionalEventActivity<TInstance, TData>(x, _filter))
                .Select(x => new EventActivityShim<TInstance, TData>(_event, x));
        }

        StateMachine<TInstance> EventActivityBinder<TInstance, TData>.StateMachine
        {
            get { return _machine; }
        }

        IEnumerator<EventActivity<TInstance>> IEnumerable<EventActivity<TInstance>>.GetEnumerator()
        {
            if (_filter == null)
                return _activities.Select(CreateEventActivity).GetEnumerator();

            return _activities.Select(CreateConditionalEventActivity).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<EventActivity<TInstance>>)this).GetEnumerator();
        }

        EventActivity<TInstance> CreateConditionalEventActivity(Activity<TInstance, TData> activity)
        {
            var conditionalEventActivity = new ConditionalEventActivity<TInstance, TData>(activity, _filter);

            var converterActivity = new DataConverterActivity<TInstance, TData>(conditionalEventActivity);

            return new EventActivityShim<TInstance>(_event, converterActivity);
        }

        EventActivity<TInstance> CreateEventActivity(Activity<TInstance, TData> activity)
        {
            var converterActivity = new DataConverterActivity<TInstance, TData>(activity);

            return new EventActivityShim<TInstance>(_event, converterActivity);
        }
    }
}