// Copyright 2011-2016 Chris Patterson, Dru Sellers
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


    public interface ExceptionActivityBinder<TInstance, TException> :
        EventActivities<TInstance>
        where TInstance : class
        where TException : Exception
    {
        StateMachine<TInstance> StateMachine { get; }

        Event Event { get; }

        ExceptionActivityBinder<TInstance, TException> Add(Activity<TInstance> activity);

        /// <summary>
        /// Catch an exception and execute the compensating activities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TInstance, TException> Catch<T>(
            Func<ExceptionActivityBinder<TInstance, T>, ExceptionActivityBinder<TInstance, T>> activityCallback)
            where T : Exception;

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TInstance, TException> If(StateMachineExceptionCondition<TInstance, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> activityCallback);
    }


    public interface ExceptionActivityBinder<TInstance, TData, TException> :
        EventActivities<TInstance>
        where TInstance : class
        where TException : Exception
    {
        StateMachine<TInstance> StateMachine { get; }

        Event<TData> Event { get; }

        ExceptionActivityBinder<TInstance, TData, TException> Add(Activity<TInstance> activity);

        ExceptionActivityBinder<TInstance, TData, TException> Add(Activity<TInstance, TData> activity);

        /// <summary>
        /// Catch an exception and execute the compensating activities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TInstance, TData, TException> Catch<T>(
            Func<ExceptionActivityBinder<TInstance, TData, T>, ExceptionActivityBinder<TInstance, TData, T>> activityCallback)
            where T : Exception;

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TInstance, TData, TException> If(StateMachineExceptionCondition<TInstance, TData, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                activityCallback);
    }
}