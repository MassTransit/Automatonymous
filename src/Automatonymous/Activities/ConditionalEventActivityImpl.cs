// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
    using Taskell;


    public class ConditionalEventActivityImpl<TInstance, TData> :
        Activity<TInstance, TData>
    {
        readonly Activity<TInstance> _activity;
        readonly Func<TData, bool> _filterExpression;

        public ConditionalEventActivityImpl(Activity<TInstance> activity, Expression<Func<TData, bool>> filterExpression)
        {
            _activity = activity;
            _filterExpression = filterExpression.Compile();
        }

        public void Accept(StateMachineInspector inspector)
        {
            _activity.Accept(inspector);
        }

        void Activity<TInstance, TData>.Execute(Composer composer, TInstance instance, TData value)
        {
            composer.Execute(() =>
                {
                    if (_filterExpression(value) == false)
                        return composer.ComposeCompleted();

                    return composer.ComposeActivity(_activity, instance, value);
                });
        }
    }
}