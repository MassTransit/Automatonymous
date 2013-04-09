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
namespace Automatonymous.Activities
{
    using Taskell;


    public class EventActivityImpl<TInstance> :
        EventActivity<TInstance>
    {
        readonly Activity<TInstance> _activity;
        readonly Event _event;

        public EventActivityImpl(Event @event, Activity<TInstance> activity)
        {
            _event = @event;
            _activity = activity;
        }

        void AcceptStateMachineInspector.Accept(StateMachineInspector inspector)
        {
            _activity.Accept(inspector);
        }

        Event EventActivity<TInstance>.Event
        {
            get { return _event; }
        }

        void Activity<TInstance>.Execute(Composer composer, TInstance instance)
        {
            _activity.Execute(composer, instance);
        }

        void Activity<TInstance>.Execute<T>(Composer composer, TInstance instance, T value)
        {
            _activity.Execute(composer, instance, value);
        }
    }
}