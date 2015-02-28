// Copyright 2011-2015 Chris Patterson, Dru Sellers
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


    public interface CompensateActivityBinder<TInstance, TException> :
        EventActivities<TInstance>
        where TInstance : class
        where TException : Exception
    {
        StateMachine<TInstance> StateMachine { get; }

        Event Event { get; }

        CompensateActivityBinder<TInstance, TException> Add(Activity<TInstance> activity);

        CompensateActivityBinder<TInstance, T> Catch<T>(
            Func<CompensateActivityBinder<TInstance, T>, CompensateActivityBinder<TInstance, T>> activityCallback)
            where T : Exception;
    }


    public interface CompensateActivityBinder<TInstance, TData, TException> :
        EventActivities<TInstance>
        where TInstance : class
        where TException : Exception
    {
        StateMachine<TInstance> StateMachine { get; }

        Event<TData> Event { get; }

        CompensateActivityBinder<TInstance, TData, TException> Add(Activity<TInstance> activity);

        CompensateActivityBinder<TInstance, TData, TException> Add(Activity<TInstance, TData> activity);

        CompensateActivityBinder<TInstance, TData, T> Catch<T>(
            Func<CompensateActivityBinder<TInstance,TData,T>, CompensateActivityBinder<TInstance,TData,T>> activityCallback)
            where T : Exception;
    }
}