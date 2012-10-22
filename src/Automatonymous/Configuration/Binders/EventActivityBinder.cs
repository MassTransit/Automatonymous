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
namespace Automatonymous.Binders
{
    using System.Collections.Generic;


    public interface EventActivityBinder<TInstance> :
        IEnumerable<EventActivity<TInstance>>
        where TInstance : class
    {
        StateMachine<TInstance> StateMachine { get; }
        Event Event { get; }

        EventActivityBinder<TInstance> Add(Activity<TInstance> activity);

        EventActivityBinder<TInstance> Add(AsyncActivity<TInstance> activity);
    }


    public interface EventActivityBinder<TInstance, TData> :
        IEnumerable<EventActivity<TInstance>>
        where TInstance : class
    {
        StateMachine<TInstance> StateMachine { get; }
        Event<TData> Event { get; }

        EventActivityBinder<TInstance, TData> Add(Activity<TInstance> activity);

        EventActivityBinder<TInstance, TData> Add(AsyncActivity<TInstance> activity);

        EventActivityBinder<TInstance, TData> Add(Activity<TInstance, TData> activity);

        EventActivityBinder<TInstance, TData> Add(AsyncActivity<TInstance, TData> activity);
    }
}