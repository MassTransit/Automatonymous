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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Activities;


    public class ExceptionActivityBinderImpl<TInstance> :
        ExceptionActivityBinder<TInstance>
        where TInstance : class, StateMachineInstance
    {
        readonly IEnumerable<ExceptionActivity<TInstance>> _activities;
        readonly Event _event;
        readonly StateMachine<TInstance> _machine;

        public ExceptionActivityBinderImpl(StateMachine<TInstance> machine, Event @event)
            : this(machine, @event, Enumerable.Empty<ExceptionActivity<TInstance>>())
        {
        }

        public ExceptionActivityBinderImpl(StateMachine<TInstance> machine, Event @event,
                                           IEnumerable<ExceptionActivity<TInstance>> activities)
        {
            _activities = activities;
            _machine = machine;
            _event = @event;
        }

        public ExceptionActivityBinder<TInstance> Handle<TException>(
            Func<EventActivityBinder<TInstance, TException>, EventActivityBinder<TInstance, TException>> context)
            where TException : Exception
        {
            EventActivityBinder<TInstance, TException> contextBinder = new DataEventActivityBinder
                <TInstance, TException>(
                _machine, new DataEvent<TException>(typeof(TException).Name));

            contextBinder = context(contextBinder);

            var handler = new ExceptionHandlerActivity<TInstance, TException>(contextBinder, typeof(TException), contextBinder.Event);

            return new ExceptionActivityBinderImpl<TInstance>(_machine, _event,
                _activities.Concat(Enumerable.Repeat(handler, 1)));
        }

        public IEnumerator<ExceptionActivity<TInstance>> GetEnumerator()
        {
            return _activities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    
    public class ExceptionActivityBinderImpl<TInstance, TData> :
        ExceptionActivityBinder<TInstance, TData>
        where TInstance : class, StateMachineInstance
    {
        readonly IEnumerable<ExceptionActivity<TInstance>> _activities;
        readonly Event _event;
        readonly StateMachine<TInstance> _machine;

        public ExceptionActivityBinderImpl(StateMachine<TInstance> machine, Event @event)
            : this(machine, @event, Enumerable.Empty<ExceptionActivity<TInstance>>())
        {
        }

        public ExceptionActivityBinderImpl(StateMachine<TInstance> machine, Event @event,
                                           IEnumerable<ExceptionActivity<TInstance>> activities)
        {
            _activities = activities;
            _machine = machine;
            _event = @event;
        }

        public ExceptionActivityBinder<TInstance, TData> Handle<TException>(
            Func<EventActivityBinder<TInstance, Tuple<TData, TException>>,
                EventActivityBinder<TInstance, Tuple<TData, TException>>> context)
            where TException : Exception
        {
            EventActivityBinder<TInstance, Tuple<TData, TException>> contextBinder = new DataEventActivityBinder
                <TInstance, Tuple<TData, TException>>(
                _machine,
                new DataEvent<Tuple<TData, TException>>(typeof(TData).Name + "." + typeof(TException).Name));

            contextBinder = context(contextBinder);

            var handler = new ExceptionHandlerActivity<TInstance, Tuple<TData, TException>>(contextBinder,
                typeof(TException), contextBinder.Event);

            return new ExceptionActivityBinderImpl<TInstance, TData>(_machine, _event,
                _activities.Concat(Enumerable.Repeat(handler, 1)));
        }

        public IEnumerator<ExceptionActivity<TInstance>> GetEnumerator()
        {
            return _activities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}