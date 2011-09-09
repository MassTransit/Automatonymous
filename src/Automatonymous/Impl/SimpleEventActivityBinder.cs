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
namespace Automatonymous.Impl
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    public class SimpleEventActivityBinder<TInstance> :
        EventActivityBinder<TInstance>
        where TInstance : StateMachineInstance
    {
        readonly IEnumerable<Activity<TInstance>> _activities;
        readonly Event _event;
        readonly StateMachine<TInstance> _machine;

        public SimpleEventActivityBinder(StateMachine<TInstance> machine, Event @event)
            : this(machine, @event, Enumerable.Empty<Activity<TInstance>>())
        {
        }

        public SimpleEventActivityBinder(StateMachine<TInstance> machine, Event @event,
                                         IEnumerable<Activity<TInstance>> activities)
        {
            _event = @event;
            _activities = activities;
            _machine = machine;
        }

        public Event Event
        {
            get { return _event; }
        }

        public EventActivityBinder<TInstance> Add(Activity<TInstance> activity,
                                                  params ExceptionBinder<TInstance>[] exceptions)
        {
            return new SimpleEventActivityBinder<TInstance>(_machine, _event,
                _activities.Concat(Enumerable.Repeat(activity, 1)));
        }

        public StateMachine<TInstance> StateMachine
        {
            get { return _machine; }
        }

        public IEnumerator<EventActivity<TInstance>> GetEnumerator()
        {
            return _activities.Select(x => new EventActivityImpl<TInstance>(_event, x)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}