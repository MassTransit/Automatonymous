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
    using Activities;


    public class ExceptionActivityImpl<TInstance, TException> :
        ExceptionActivity<TInstance>
        where TInstance : StateMachineInstance
        where TException : Exception
    {
        readonly Activity<TInstance> _activity;

        public ExceptionActivityImpl(Activity<TInstance> activity)
        {
            _activity = activity;
        }

        public void Execute(TInstance instance, object value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "The exception argument cannot be null");

            var data = value as TException;
            if (data == null)
            {
                throw new ArgumentException("The exception was not a compatible type: " + value.GetType().Name,
                    "value");
            }

            _activity.Execute(instance, data);
        }

        public void Inspect(StateMachineInspector inspector)
        {
            _activity.Inspect(inspector);
        }

        public Type ExceptionType
        {
            get { return typeof(TException); }
        }
    }
}