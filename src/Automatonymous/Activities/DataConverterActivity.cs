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
    using System.Threading.Tasks;


    public class DataConverterActivity<TInstance, TData> :
        Activity<TInstance>
    {
        readonly Activity<TInstance, TData> _activity;

        public DataConverterActivity(Activity<TInstance, TData> activity)
        {
            _activity = activity;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => _activity.Accept(inspector));
        }

        public Task Execute(TInstance instance)
        {
            throw new AutomatonymousException("This activity requires a body with the event, but no body was specified.");
        }

        public async Task Execute<T>(TInstance instance, T value)
        {
            if (!(value is TData))
            {
                string message = "Expected: " + typeof(TData).FullName + ", Received: " + value.GetType().FullName;
                throw new ArgumentException(message, "value");
            }

            object data = value;

            if (data == null)
                throw new ArgumentNullException("value", "The data argument cannot be null");

            await _activity.Execute(instance, (TData)data);
        }
    }
}