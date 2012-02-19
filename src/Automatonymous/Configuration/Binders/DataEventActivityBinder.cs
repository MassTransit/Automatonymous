// Copyright 2011 Chris Patterson, Dru Sellers
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
    using System.Linq.Expressions;
    using Activities;


    public class DataEventActivityBinder<TInstance, TData> :
        EventActivityBinder<TInstance, TData>
        where TInstance : class, StateMachineInstance
        where TData : class
    {
        readonly IEnumerable<Activity<TInstance>> _activities;
        readonly Event<TData> _event;
        readonly Expression<Func<TData, bool>> _filterExpression;
        readonly AutomatonymousStateMachine<TInstance> _machine;

        public DataEventActivityBinder(AutomatonymousStateMachine<TInstance> machine, Event<TData> @event)
            : this(machine, @event, x => true, Enumerable.Empty<Activity<TInstance>>())
        {
        }

        public DataEventActivityBinder(AutomatonymousStateMachine<TInstance> machine, Event<TData> @event,
                                       Expression<Func<TData, bool>> filterExpression)
            : this(machine, @event, filterExpression, Enumerable.Empty<Activity<TInstance>>())
        {
        }

        public DataEventActivityBinder(AutomatonymousStateMachine<TInstance> machine, Event<TData> @event,
                                       Expression<Func<TData, bool>> filterExpression,
                                       IEnumerable<Activity<TInstance>> activities)
        {
            _event = @event;
            _activities = activities;
            _machine = machine;
            _filterExpression = filterExpression;
        }

        public Expression<Func<TData, bool>> FilterExpression
        {
            get { return _filterExpression; }
        }

        public Event<TData> Event
        {
            get { return _event; }
        }

        public EventActivityBinder<TInstance, TData> Add(Activity<TInstance> activity)
        {
            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filterExpression,
                _activities.Concat(Enumerable.Repeat(activity, 1)));
        }

        public EventActivityBinder<TInstance, TData> Add(Activity<TInstance, TData> activity)
        {
            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filterExpression,
                _activities.Concat(Enumerable.Repeat(new DataConverterActivity<TInstance, TData>(activity), 1)));
        }

        public AutomatonymousStateMachine<TInstance> StateMachine
        {
            get { return _machine; }
        }

        public IEnumerator<EventActivity<TInstance>> GetEnumerator()
        {
            return _activities.Select(x => new ConditionalEventActivityImpl<TInstance, TData>(_event, x, _filterExpression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}