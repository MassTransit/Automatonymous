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
namespace Automatonymous.Activities
{
    using System.Threading.Tasks;


    /// <summary>
    /// Routes event activity to an activity
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public class EventActivityShim<TInstance> :
        EventActivity<TInstance>
    {
        readonly Activity<TInstance> _activity;
        readonly Event _event;

        public EventActivityShim(Event @event, Activity<TInstance> activity)
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

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return _activity.Execute(context, next);
        }

        public Task Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            return _activity.Execute(context, next);
        }
    }


    /// <summary>
    /// Routes event activity to an activity
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class EventActivityShim<TInstance, TData> :
        EventActivity<TInstance>
    {
        readonly Activity<TInstance, TData> _activity;
        readonly Event _event;

        public EventActivityShim(Event @event, Activity<TInstance, TData> activity)
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

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            throw new AutomatonymousException("This activity requires a body with the event, but no body was specified.");
        }

        public Task Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            var activity = _activity as Activity<TInstance, T>;
            if (activity == null)
                throw new AutomatonymousException("Expected Type " + typeof(TData).Name + " but was " + typeof(T).Name);

            return activity.Execute(context, next);
        }
    }
}