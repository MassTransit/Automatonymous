// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using Behaviors;


    /// <summary>
    /// Adapts an Activity to a Data Activity context
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class SlimActivity<TInstance, TData> :
        Activity<TInstance, TData>
    {
        readonly Activity<TInstance> _activity;

        public SlimActivity(Activity<TInstance> activity)
        {
            _activity = activity;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            _activity.Accept(visitor);
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> behavior)
        {
            var upconvert = new WidenBehavior<TInstance, TData>(behavior, context);

            await _activity.Execute(context, upconvert);
        }

        Task Activity<TInstance, TData>.Compensate<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
        {
            return next.Compensate(context);
        }
    }
}