// Copyright 2011-2014 Chris Patterson, Dru Sellers
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
    using System.Threading;
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

        Task Activity<TInstance>.Execute(TInstance instance, CancellationToken cancellationToken)
        {
            return _activity.Execute(instance, cancellationToken);
        }

        Task Activity<TInstance>.Execute<T>(TInstance instance, T value, CancellationToken cancellationToken)
        {
            return _activity.Execute(instance, value, cancellationToken);
        }
    }
}