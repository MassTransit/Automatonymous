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


    public interface ExceptionActivityBinder<TInstance> :
        ExceptionBinder<TInstance>
        where TInstance : StateMachineInstance
    {
        ExceptionActivityBinder<TInstance> Handle<TException>(Func<EventActivityBinder<TInstance, TException>,
                                                                  EventActivityBinder<TInstance, TException>> context)
            where TException : Exception;
    }


    public interface ExceptionActivityBinder<TInstance, TData> :
        ExceptionBinder<TInstance>
        where TInstance : StateMachineInstance
    {
        ExceptionActivityBinder<TInstance, TData> Handle<TException>(
            Func<EventActivityBinder<TInstance, Tuple<TData, TException>>,
                EventActivityBinder<TInstance, Tuple<TData, TException>>> context)
            where TException : Exception;
    }
}