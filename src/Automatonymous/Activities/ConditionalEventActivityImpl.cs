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
namespace Automatonymous.Activities
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;


    public class ConditionalEventActivityImpl<TInstance, TData> :
        EventActivity<TInstance>
    {
        readonly Activity<TInstance> _activity;
        readonly Event _event;
        readonly Func<TData, bool> _filterExpression;

        public ConditionalEventActivityImpl(Event @event, Activity<TInstance> activity,
            Expression<Func<TData, bool>> filterExpression)
        {
            _event = @event;
            _activity = activity;
            _filterExpression = filterExpression.Compile();
        }

        public async Task Execute(TInstance instance)
        {
            await _activity.Execute(instance);
        }

        public async Task Execute<TEventData>(TInstance instance, TEventData value)
        {
            if(!(value is TData))
            {
                string message = "Expected: " + typeof(TData).FullName + ", Received: " + value.GetType().FullName;
                throw new ArgumentException(message, "value");
            }

            object data = value;

            if (data == null)
                throw new ArgumentNullException("value", "The data argument cannot be null");

            if (_filterExpression((TData)data) == false)
                return;

            await _activity.Execute(instance, (TData)data);
        }

        public void Accept(StateMachineInspector inspector)
        {
            _activity.Accept(inspector);
        }

        public Event Event
        {
            get { return _event; }
        }
    }
}