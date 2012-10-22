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
namespace Automatonymous
{
    using System;
    using System.Linq.Expressions;
    using Activities;
    using Binders;

    // TODO: call never used in tests
    public static class CallExtensions
    {
        public static EventActivityBinder<TInstance> Call<TInstance>(
            this EventActivityBinder<TInstance> source,
            Expression<Action<TInstance>> expression)
            where TInstance : class
        {
            return source.Add(new CallActivity<TInstance>(expression));
        }

        public static EventActivityBinder<TInstance, TData> Call<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source,
            Expression<Action<TInstance, TData>> expression)
            where TInstance : class
        {
            return source.Add(new CallActivity<TInstance, TData>(expression));
        }
    }
}