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
    using Taskell;


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

        void Activity<TInstance>.Execute(Composer composer, TInstance instance)
        {
            composer.Execute(() =>
                {
                    Activity<TInstance> activity = _activityFactory();

                    return composer.ComposeActivity(activity, instance);
                });
        }

        void Activity<TInstance>.Execute<T>(Composer composer, TInstance instance, T value)
        {
            composer.Execute(() =>
                {
                    Activity<TInstance> activity = _activityFactory();

                    return composer.ComposeActivity(activity, instance);
                });
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

        void Activity<TInstance, TData>.Execute(Composer composer, TInstance instance, TData value)
        {
            composer.Execute(() =>
                {
                    Activity<TInstance, TData> activity = _activityFactory();

                    return composer.ComposeActivity(activity, instance, value);
                });
        }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }
    }
}