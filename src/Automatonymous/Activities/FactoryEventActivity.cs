// Copyright 2011-2014 Chris Patterson, Dru Sellers
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
    using System.Threading;
    using System.Threading.Tasks;


    public class FactoryEventActivity<TInstance> :
        Activity<TInstance>
    {
        readonly Func<Activity<TInstance>> _activityFactory;

        public FactoryEventActivity(Func<Activity<TInstance>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }

        Task Activity<TInstance>.Execute(TInstance instance, CancellationToken cancellationToken)
        {
            return Execute(instance, cancellationToken);
        }

        Task Activity<TInstance>.Execute<T>(TInstance instance, T value, CancellationToken cancellationToken)
        {
            return Execute(instance, cancellationToken);
        }

        Task Execute(TInstance instance, CancellationToken cancellationToken)
        {
            Activity<TInstance> activity = _activityFactory();

            return activity.Execute(instance, cancellationToken);
        }
    }


    public class FactoryEventActivity<TInstance, TData> :
        Activity<TInstance, TData>
    {
        readonly Func<Activity<TInstance, TData>> _activityFactory;

        public FactoryEventActivity(Func<Activity<TInstance, TData>> activityFactory)
        {
            _activityFactory = activityFactory;
        }

        Task Activity<TInstance, TData>.Execute(TInstance instance, TData value, CancellationToken cancellationToken)
        {
            Activity<TInstance, TData> activity = _activityFactory();

            return activity.Execute(instance, value, cancellationToken);
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}