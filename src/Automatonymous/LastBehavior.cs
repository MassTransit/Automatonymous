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
namespace Automatonymous
{
    using System.Threading.Tasks;


    public class LastBehavior<TInstance> :
        Behavior<TInstance>
        where TInstance : class
    {
        readonly Activity<TInstance> _activity;

        public LastBehavior(Activity<TInstance> activity)
        {
            _activity = activity;
        }

        public void Accept(StateMachineInspector inspector)
        {
            _activity.Accept(inspector);
        }

        public Task Execute(BehaviorContext<TInstance> context)
        {
            return _activity.Execute(context, Behavior.Empty<TInstance>());
        }

        public Task Execute<T>(BehaviorContext<TInstance, T> context)
        {
            return _activity.Execute(context, Behavior.Empty<TInstance>());
        }
    }
}