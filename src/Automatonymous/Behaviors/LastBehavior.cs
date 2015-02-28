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
namespace Automatonymous.Behaviors
{
    using System.Threading.Tasks;


    public class LastBehavior<TInstance> :
        Behavior<TInstance>
    {
        readonly Activity<TInstance> _activity;

        public LastBehavior(Activity<TInstance> activity)
        {
            _activity = activity;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            _activity.Accept(visitor);
        }

        Task Behavior<TInstance>.Execute(BehaviorContext<TInstance> context)
        {
            return _activity.Execute(context, Behavior.Empty<TInstance>());
        }

        Task Behavior<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context)
        {
            return _activity.Execute(context, Behavior.Empty<TInstance, T>());
        }

        Task Behavior<TInstance>.Compensate<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context)
        {
            return _activity.Compensate(context, Behavior.Empty<TInstance, T>());
        }

        Task Behavior<TInstance>.Compensate<TException>(BehaviorExceptionContext<TInstance, TException> context)
        {
            return _activity.Compensate(context, Behavior.Empty<TInstance>());
        }
    }


    public class LastBehavior<TInstance, TData> :
        Behavior<TInstance, TData>
    {
        readonly Activity<TInstance, TData> _activity;

        public LastBehavior(Activity<TInstance, TData> activity)
        {
            _activity = activity;
        }

        Task Behavior<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context)
        {
            return _activity.Execute(context, Behavior.Empty<TInstance, TData>());
        }

        Task Behavior<TInstance, TData>.Compensate<TException>(BehaviorExceptionContext<TInstance, TData, TException> context)
        {
            return _activity.Compensate(context, Behavior.Empty<TInstance, TData>());
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            _activity.Accept(visitor);
        }
    }
}